using EnterprisePlatform.Application.Abstractions.ExternalServices;

namespace EnterprisePlatform.Infrastructure.ExternalServices;

public sealed class FakeExternalApiClient : IExternalApiClient
{
    public async Task<string> GetCustomerProfileAsync(
        Guid customerId,
        CancellationToken cancellationToken)
    {
        await Task.Delay(1000, cancellationToken);

        return "Customer profile loaded";
    }

    public async Task<string> GetPaymentSummaryAsync(
        Guid customerId,
        CancellationToken cancellationToken)
    {
        await Task.Delay(1000, cancellationToken);

        return "Payment summary loaded";
    }

    public async Task<string> GetNotificationStatusAsync(
        Guid customerId,
        CancellationToken cancellationToken)
    {
        await Task.Delay(1000, cancellationToken);

        return "Notification status loaded";
    }
}