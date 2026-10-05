public sealed record UpdateCustomerRequest(
    string FirstName,
    string LastName,
    string Email,
    string? PhoneNumber,
    DateTime DateOfBirth);