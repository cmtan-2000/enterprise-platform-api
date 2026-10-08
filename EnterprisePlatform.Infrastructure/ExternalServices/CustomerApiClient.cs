using System.Net.Http.Json;
using EnterprisePlatform.Application.Abstractions.ExternalServices;
using EnterprisePlatform.Application.Exceptions;

namespace EnterprisePlatform.Infrastructure.ExternalServices
{
    public sealed class CustomerApiClient
        : ICustomerApiClient
    {
        private readonly HttpClient _httpClient;

        public CustomerApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<CustomerProfileDto?> GetProfileAsync(Guid customerId, CancellationToken cancellationToken)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<CustomerProfileDto>(
                    $"/api/customers/external-simulation/{customerId}",
                    cancellationToken);
            }
            catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
            {
                throw new DownstreamTimeoutException("Customer API request timed out.", ex);
            }
        }
    }
}