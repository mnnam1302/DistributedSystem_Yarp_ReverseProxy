namespace Authorization.Domain.Exceptions
{
    public static class AppUserException
    {
        public class UserNotFoundByEmailException : NotFoundException
        {
            public UserNotFoundByEmailException(string email)
                : base($"User with email: {email} not found")
            {
            }
        }

        public class UserFieldException : NotFoundException
        {
            public UserFieldException(string userField)
                : base($"The user with field {userField} is not correct.")
            {
            }
        }
    }
}