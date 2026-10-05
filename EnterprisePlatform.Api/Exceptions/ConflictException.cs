namespace EnterprisePlatform.Application.Exceptions;

public sealed class ConflictException : AppException
{
    public ConflictException(
        string message,
        string code)
        : base(message, code)
    {
    }
}