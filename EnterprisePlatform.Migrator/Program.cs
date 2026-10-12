using EnterprisePlatform.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

var configuration = new ConfigurationBuilder()
    .AddEnvironmentVariables()
    .Build();

var connectionString =
    configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        "ConnectionStrings:DefaultConnection is not configured.");
}

var options = new DbContextOptionsBuilder<AppDbContext>()
    .UseNpgsql(connectionString)
    .Options;

await using var dbContext = new AppDbContext(options);

Console.WriteLine("Applying EF Core migrations...");
await dbContext.Database.MigrateAsync();
Console.WriteLine("Migrations completed successfully.");