namespace Authorization.Domain.Exceptions;

public static class AppUserException
{
    public class UserNotFoundByIdException : NotFoundException
    {
        public UserNotFoundByIdException(string id)
            : base($"The user with id {id} was not found.")
        {
        }
    }

    public class UserNotFoundByEmailException : NotFoundException
    {
        public UserNotFoundByEmailException(string email)
            : base($"The user with email {email} was not found.")
        {
        }
    }

    public class UserFieldException : BadRequestException
    {
        public UserFieldException(string userField)
            : base($"The user with field {userField} is not correct.")
        {
        }
    }

    public class UserAlreadyExistsException : BadRequestException
    {
        public UserAlreadyExistsException(string email)
            : base($"The user with email {email} has already exists.")
        {
        }
    }
}
