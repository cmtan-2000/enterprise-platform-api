using System.Diagnostics;

namespace EnterprisePlatform.Application.Observability;

public static class ApplicationActivitySource
{
    public const string Name = "EnterprisePlatform.Application";

    public static readonly ActivitySource Source = new(Name);
}