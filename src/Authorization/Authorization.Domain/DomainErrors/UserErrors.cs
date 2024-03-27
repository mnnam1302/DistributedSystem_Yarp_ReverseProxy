using DistributedSystem.Contract.Abstractions.Shared;

namespace Authorization.Domain.DomainErrors;

public static class UserErrors
{
    public static Error NotFound(Guid id) =>
        new Error("Users.NotFound", $"The user with identifier '{id}' was not found.");

    public static Error EmailAlreadyInUse(string email) =>
        new Error("Users.Error", $"The use with email {email} has already exists.");
}