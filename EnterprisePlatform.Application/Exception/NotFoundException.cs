namespace EnterprisePlatform.Application.Exceptions;

public sealed class NotFoundException : AppException
{
    public NotFoundException(string resource, object key)
    : base($"{resource} with identifier '{key}' was not found.", $"{resource.ToLowerInvariant()}.not_found")
    {
    }
}