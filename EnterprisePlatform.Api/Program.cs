using EnterprisePlatform.Api.Configurations;
using Microsoft.OpenApi;
using EnterprisePlatform.Api.Exceptions;
using EnterprisePlatform.Infrastructure;
using EnterprisePlatform.Application;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Enterprise Platform API",
        Version = "v1",
        Description = "Enterprise-grade ASP.NET Core backend"
    });
});

builder.Services
    .AddOptions<ApplicationSettings>()
    .Bind(builder.Configuration.GetSection("Application"))
    .ValidateOnStart();

builder.Services.AddHealthChecks();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddProblemDetails();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Enterprise Platform API v1");
        options.RoutePrefix = "swagger";
    });
}

// app.UseHttpsRedirection();
app.UseExceptionHandler();

app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health/live");
app.MapHealthChecks("/health/ready");

app.Run();