using Authorization.Domain.Abstractions.Entities;
using Microsoft.AspNetCore.Identity;
using System.Diagnostics;
using static Authorization.Domain.Exceptions.AppUserException;

namespace Authorization.Domain.Entities
{
    public class AppUser : IdentityUser<Guid>
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string FullName { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public string Salt { get; set; }

        public bool IsDirector { get; set; }

        public bool IsHeadOfDepartment { get; set; }

        public Guid? ManagerId { get; set; }

        public Guid PositionId { get; set; }

        public int IsReceipient { get; set; }

        public virtual ICollection<IdentityUserClaim<Guid>> Claims { get; set; }
        public virtual ICollection<IdentityUserLogin<Guid>> Logins { get; set; }
        public virtual ICollection<IdentityUserToken<Guid>> Tokens { get; set; }
        public virtual ICollection<IdentityUserRole<Guid>> UserRoles { get; set; }


        public AppUser(Guid id, string firstName, string lastName, string fullName, DateTime? dateOfBirth, string phoneNumber, string email, string salt, string passwordHash)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            FullName = fullName;
            DateOfBirth = dateOfBirth;
            PhoneNumber = phoneNumber;
            Email = email;
            Salt = salt;
            PasswordHash = passwordHash;

            // Initialize the collections
            Claims = new List<IdentityUserClaim<Guid>>();
            Logins = new List<IdentityUserLogin<Guid>>();
            Tokens = new List<IdentityUserToken<Guid>>();
            UserRoles = new List<IdentityUserRole<Guid>>();
        }

        public static AppUser Create(Guid id, string firstName, string lastName, DateTime dateOfBirth, string phoneNumber, string email, string salt, string passwordHash)
        {
            var fullName = $"{firstName} {lastName}";

            if (fullName.Length > 100)
                throw new UserFieldException(nameof(FullName));

            if (dateOfBirth > DateTime.Now)
                throw new UserFieldException(nameof(DateOfBirth));

            if (phoneNumber.Length > 10)
                throw new UserFieldException(nameof(PhoneNumber));

            var user = new AppUser(id, firstName, lastName, fullName, dateOfBirth, phoneNumber, email, salt, passwordHash);
            return user;
        }

        //public void Update(string firstName, string lastName, DateTime dateOfBirth, string phoneNumber)
        //{
        //    var fullName = $"{firstName} {lastName}";

        //    if (fullName.Length > 100)
        //        throw new UserFieldException(nameof(FullName));

        //    if (dateOfBirth > DateTime.Now)
        //        throw new UserFieldException(nameof(DateOfBirth));

        //    if (phoneNumber.Length > 10)
        //        throw new UserFieldException(nameof(PhoneNumber));

        //    FirstName = firstName;
        //    LastName = lastName;
        //    FullName = fullName;
        //    DateOfBirth = dateOfBirth;
        //    PhoneNumber = phoneNumber;
        //}

        //public void Delete()
        //{
        //}
    }
}