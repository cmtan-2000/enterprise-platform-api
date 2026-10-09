using EnterprisePlatform.Application.Abstractions.ExternalServices;
using EnterprisePlatform.Application.Abstractions.Pagination;
using EnterprisePlatform.Application.Abstractions.Persistence;
using EnterprisePlatform.Infrastructure.ExternalServices;
using EnterprisePlatform.Infrastructure.Pagination;
using EnterprisePlatform.Infrastructure.Persistence;
using EnterprisePlatform.Infrastructure.Persistence.Interceptors;
using EnterprisePlatform.Infrastructure.Persistence.Queries;
using EnterprisePlatform.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using Npgsql;
using Polly;
using Microsoft.Extensions.Http.Resilience;

namespace EnterprisePlatform.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        var builder = new NpgsqlConnectionStringBuilder(connectionString);

        Console.WriteLine(
            $"Npgsql Maximum Pool Size = {builder.MaxPoolSize}");

        services.AddDbContext<AppDbContext>((serviceProvider, options) =>
        {
            options.UseNpgsql(connectionString);

            options.AddInterceptors(serviceProvider.GetRequiredService<ConcurrencyInterceptor>());

            if (configuration.GetValue<bool>("Database:EnableDetailedLogging"))
            {
                options.EnableDetailedErrors();
                options.EnableSensitiveDataLogging();

                options.LogTo(
                    Console.WriteLine,
                    LogLevel.Information);
            }
        });


        services.AddScoped<ConcurrencyInterceptor>();
        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<AppDbContext>());

        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IIdempotencyRepository, IdempotencyRepository>();

        services.AddScoped<ICustomerQueries, CustomerQueries>();
        services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<AppDbContext>());
        services.AddScoped<IExternalApiClient, FakeExternalApiClient>();
        services.AddHttpClient<ICustomerApiClient, CustomerApiClient>(
            client =>
            {
                client.BaseAddress =
                    new Uri(configuration["ExternalApis:CustomerApi"]!);

                client.Timeout = TimeSpan.FromSeconds(5);
            }).AddResilienceHandler("CustomerApiRetry", builder =>
            {
                builder.AddRetry(new HttpRetryStrategyOptions
                {
                    MaxRetryAttempts = 3,
                    BackoffType = DelayBackoffType.Exponential,
                    Delay = TimeSpan.FromMilliseconds(200),
                    UseJitter = true
                });

                builder.AddCircuitBreaker(new HttpCircuitBreakerStrategyOptions
                {
                    FailureRatio = 0.5,
                    MinimumThroughput = 5,
                    SamplingDuration = TimeSpan.FromSeconds(10),
                    BreakDuration = TimeSpan.FromSeconds(10)
                });
            });

        services.AddSingleton<ICursorSerializer, CursorSerializer>();

        return services;
    }
}