namespace EnterprisePlatform.Application.Exceptions;

public sealed class DownstreamTimeoutException : Exception
{
    public DownstreamTimeoutException(string message)
        : base(message)
    {
    }

    public DownstreamTimeoutException(
        string message,
        Exception innerException)
        : base(message, innerException)
    {
    }
}