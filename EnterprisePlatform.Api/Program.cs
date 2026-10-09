using EnterprisePlatform.Api.Configurations;
using Microsoft.OpenApi;
using EnterprisePlatform.Api.Exceptions;
using EnterprisePlatform.Infrastructure;
using EnterprisePlatform.Application;
using OpenTelemetry.Trace;
using OpenTelemetry.Resources;
using Npgsql;
using OpenTelemetry.Metrics;

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
builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource =>
        resource.AddService("EnterprisePlatform.Api"))
    .WithTracing(tracing =>
    {
        tracing
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddNpgsql()
            .AddSource("EnterprisePlatform.Application")
            .AddOtlpExporter(options =>
            {
                options.Endpoint = new Uri("http://localhost:4317");
                options.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;
            });
    })
    .WithMetrics(metrics =>
    {
        metrics
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddRuntimeInstrumentation()
            .AddPrometheusExporter();
    });

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
app.MapPrometheusScrapingEndpoint();

app.MapControllers();
app.MapHealthChecks("/health/live");
app.MapHealthChecks("/health/ready");

app.Run();