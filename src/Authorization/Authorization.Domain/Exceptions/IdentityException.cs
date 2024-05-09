namespace Authorization.Domain.Exceptions;

public static class IdentityException
{
    public class TokenException : DomainException
    {
        public TokenException(string message)
            : base("Token Exception", message)
        {
        }
    }

    public class AuthenticatedException : BadRequestException
    {
        public AuthenticatedException()
            : base("The email or password are incorrect.")
        {
        }
    }
}
