namespace BookApi.Domain.Exceptions;

public class ValidationErrorException(string message) : Exception(message)
{
}

public class ForbiddenException : Exception;

public class ItemNotFoundException : Exception;
