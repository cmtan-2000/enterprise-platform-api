using System.Text;
using System.Text.Json;
using EnterprisePlatform.Application.Abstractions.Pagination;

namespace EnterprisePlatform.Infrastructure.Pagination;

public sealed class CursorSerializer : ICursorSerializer
{
    private static readonly JsonSerializerOptions JsonOptions = new(
        JsonSerializerDefaults.Web);

    public string Encode<T>(T value)
    {
        var json = JsonSerializer.Serialize(
            value,
            JsonOptions);

        var bytes = Encoding.UTF8.GetBytes(json);

        return Convert.ToBase64String(bytes)
            .Replace("+", "-")
            .Replace("/", "_")
            .TrimEnd('=');
    }

    public T Decode<T>(string cursor)
    {
        var base64 = cursor
            .Replace("-", "+")
            .Replace("_", "/");

        switch (base64.Length % 4)
        {
            case 2:
                base64 += "==";
                break;

            case 3:
                base64 += "=";
                break;
        }

        var bytes = Convert.FromBase64String(base64);

        var json = Encoding.UTF8.GetString(bytes);

        return JsonSerializer.Deserialize<T>(
                   json,
                   JsonOptions)
               ?? throw new InvalidOperationException(
                   "Invalid cursor.");
    }
}