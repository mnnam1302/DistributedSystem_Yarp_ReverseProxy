using Authorization.Domain.Abstractions.Entities;
using Authorization.Domain.Exceptions;
using Microsoft.AspNetCore.Identity;

namespace Authorization.Domain.Entities;

public class AppUser : IdentityUser<Guid>, IEntity<Guid>
{
    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string FullName { get; set; }

    public DateTime? DateOfBirth { get; set; }

    public bool IsDirector { get; set; }

    public bool IsHeadOfDepartment { get; set; }

    public Guid? ManagerId { get; set; }

    public Guid PositionId { get; set; }

    public int IsReceipient { get; set; }

    public string PasswordSalt { get; set; }

    public virtual ICollection<IdentityUserClaim<Guid>> Claims { get; set; }

    public virtual ICollection<IdentityUserLogin<Guid>> Logins { get; set; }

    public virtual ICollection<IdentityUserToken<Guid>> Tokens { get; set; }

    public virtual ICollection<IdentityUserRole<Guid>> UserRoles { get; set; }

    protected AppUser(Guid id, string firstName, string lastName, string fullName, DateTime? dateOfBirth, string? phoneNumber, string email, string passwordHash, string passwordSalt)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        FullName = fullName;
        DateOfBirth = dateOfBirth;
        PhoneNumber = phoneNumber;
        Email = email;
        PasswordHash = passwordHash;
        PasswordSalt = passwordSalt;
    }

    public static AppUser Create(Guid id, string firstName, string lastName, DateTime? dateOfBirth, string? PhoneNumber, string email, string passwordHash, string passwordSalt)
    {
        if (firstName.Length > 10)
            throw new AppUserException.UserFieldException(nameof(FirstName));

        if (lastName.Length > 10)
            throw new AppUserException.UserFieldException(nameof(LastName));

        if (dateOfBirth.HasValue)
        {
            if (dateOfBirth.Value > DateTime.Now)
                throw new AppUserException.UserFieldException(nameof(DateOfBirth));
        }

        string fullName = $"{firstName} {lastName}";

        var user = new AppUser(id, firstName, lastName, fullName, dateOfBirth, PhoneNumber, email, passwordHash, passwordSalt);

        return user;
    }
}