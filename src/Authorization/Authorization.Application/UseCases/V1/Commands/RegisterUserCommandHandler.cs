using Authorization.Application.Abstractions;
using Authorization.Domain.Abstractions.Repositories;
using Authorization.Domain.Entities;
using Authorization.Domain.Exceptions;
using DistributedSystem.Contract.Abstractions.Message;
using DistributedSystem.Contract.Abstractions.Shared;
using DistributedSystem.Contract.Services.V1.Identity;

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
        /*
            1. Check if user exists
            2. Hash password
            3. Create user
            4. Add user to database
         */

        // 1.
        var isExistsUser = await _userRepository.FindSingleAsync(x => x.Email == request.Email, cancellationToken);

        if (isExistsUser is not null)
        {
            throw new AppUserException.UserAlreadyExistsException(request.Email);
        }

        // 2.
        var hashPassword = _passwordHasherService.HashPassword(request.Password);

        // 3.
        var user = AppUser.Create(Guid.NewGuid(), request.FirstName, request.LastName, request.DateOfBirth, request.PhoneNumber, request.Email, hashPassword);

        // 4.
        _userRepository.Add(user);

        return Result.Success();
    }
}
