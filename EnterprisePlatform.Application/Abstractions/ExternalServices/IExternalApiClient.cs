namespace EnterprisePlatform.Application.Abstractions.ExternalServices;

public interface IExternalApiClient
{
    Task<string> GetCustomerProfileAsync(
        Guid customerId,
        CancellationToken cancellationToken);

    Task<string> GetPaymentSummaryAsync(
        Guid customerId,
        CancellationToken cancellationToken);

    Task<string> GetNotificationStatusAsync(
        Guid customerId,
        CancellationToken cancellationToken);
}