using Authorization.Application.Abstractions;
using Authorization.Domain.Abstractions;
using Authorization.Domain.Abstractions.Repositories;
using Authorization.Domain.Exceptions;
using DistributedSystem.Contract.Abstractions.Message;
using DistributedSystem.Contract.Abstractions.Shared;
using DistributedSystem.Contract.Services.V1.Identity;
using Microsoft.EntityFrameworkCore.Metadata;


namespace Authorization.Application.UseCases.V1.Commands
{
    public class RegisterCommandHandler : ICommandHandler<Command.RegisterCommand>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHashPasswordService _hashPasswordService;

        public RegisterCommandHandler(
            IUserRepository userRepository,
            IUnitOfWork unitOfWork,
            IHashPasswordService hashPasswordService)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _hashPasswordService = hashPasswordService;
        }

        public async Task<Result> Handle(Command.RegisterCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.FindSingleUserAsync(x => x.Email == request.Email, cancellationToken);

            if (user is not null)
                throw new AppUserException.UserExistingException(nameof(request.Email), request.Email);

            var hashedPassword = _hashPasswordService.HashPassword(request.Password, out string salt);

            var userCreate = Domain.Entities.AppUser.Create(Guid.NewGuid(), request.FirstName, request.LastName, request.DateOfBirth, request.PhoneNumber, request.Email, salt, hashedPassword);

            _userRepository.Add(userCreate);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}