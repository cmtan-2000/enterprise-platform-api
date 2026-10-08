using EnterprisePlatform.Application.Abstractions.Pagination;
using EnterprisePlatform.Application.Abstractions.Persistence;
using EnterprisePlatform.Infrastructure.Pagination;
using EnterprisePlatform.Infrastructure.Persistence;
using EnterprisePlatform.Infrastructure.Persistence.Interceptors;
using EnterprisePlatform.Infrastructure.Persistence.Queries;
using EnterprisePlatform.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Npgsql;

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

                options.LogTo(
                    Console.WriteLine,
                    LogLevel.Information);
            }
            options.EnableSensitiveDataLogging();
        });


        services.AddScoped<ConcurrencyInterceptor>();
        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<AppDbContext>());
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<ICustomerQueries, CustomerQueries>();
        services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<AppDbContext>());
        services.AddSingleton<ICursorSerializer, CursorSerializer>();

        return services;
    }
}