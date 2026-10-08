namespace EnterprisePlatform.Application.Abstractions.ExternalServices;

public sealed record CustomerProfileDto(
    Guid Id,
    string FirstName,
    string LastName,
    string Email);