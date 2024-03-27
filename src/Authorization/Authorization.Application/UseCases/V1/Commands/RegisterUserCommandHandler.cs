using Authorization.Application.Abstractions;
using Authorization.Domain.Abstractions;
using Authorization.Domain.Abstractions.Repositories;
using Authorization.Domain.DomainErrors;
using Authorization.Domain.Entities;
using Authorization.Domain.Exceptions;
using DistributedSystem.Contract.Abstractions.Message;
using DistributedSystem.Contract.Abstractions.Shared;
using DistributedSystem.Contract.Services.V1.Identity;
using Microsoft.EntityFrameworkCore.Metadata;


namespace Authorization.Application.UseCases.V1.Commands;

public class RegisterUserCommandHandler : ICommandHandler<Command.RegisterUserCommand>
{
    private readonly IRepositoryBase<AppUser, Guid> _userRepository;
    private readonly IPasswordHasherService _passwordHasherService;

    public RegisterUserCommandHandler(
        IRepositoryBase<AppUser, Guid> userRepository,
        IPasswordHasherService passwordHasherService)
    {
        _userRepository = userRepository;
        _passwordHasherService = passwordHasherService;
    }

    public async Task<Result> Handle(Command.RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var isExistsUser = await _userRepository.FindSingleAsync(x => x.Email.Equals(request.Email), cancellationToken);

        // Should be thrown here, or use Result.Failure(Error)
        if (isExistsUser is not null)
            //throw new IdentityException.UserExistsException("The user with email has already exists.");
            return Result.Failure(UserErrors.EmailAlreadyInUse(request.Email));

        var passwordSalt = _passwordHasherService.GenerateSalt();
        var passwordHash = _passwordHasherService.HashPassword(request.Password, passwordSalt);

        var user = AppUser.Create(Guid.NewGuid(), request.FirstName, request.LastName, request.DateOfBirth, request.PhoneNumber, request.Email, passwordHash, passwordSalt);

        _userRepository.Add(user);

        return Result.Success();
    }
}