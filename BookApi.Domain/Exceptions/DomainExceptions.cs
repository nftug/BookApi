namespace BookApi.Domain.Exceptions;

public class ValidationErrorException : Exception
{
    public ValidationErrorException(string message) : base(message)
    {
    }
}

public class ForbiddenException : Exception;

public class ItemNotFoundException : Exception;
