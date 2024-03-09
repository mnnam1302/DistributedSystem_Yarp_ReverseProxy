namespace Authorization.Domain.Exceptions
{
    public static class AppUserException
    {
        public class UserNotFoundByFieldException : NotFoundException
        {
            public UserNotFoundByFieldException(string userField)
                : base($"User with field {userField} is not found")
            {
            }
        }
        public class UserExistingException : BadRequestException
        {
            public UserExistingException(string userField, string value) 
                : base($"The user with field {userField}: {value} is already existing.")
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