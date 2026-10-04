
namespace FileProcessing.Api.Middleware;
public class ApiMiddleware
{
    private const string ApiKeyHeader = "X-API-Key";

    private readonly RequestDelegate _next;
    private readonly IConfiguration _configuration;

    public ApiMiddleware(
        RequestDelegate next,
        IConfiguration configuration)
    {
        _next = next;
        _configuration = configuration;
    }
    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Request.Headers.TryGetValue(ApiKeyHeader, out var providedApiKey))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsJsonAsync(new
            {
                error = "API key is required."
            });
            return;
        }

        var configuredApiKey = _configuration["ApiKey"];
        if (string.IsNullOrWhiteSpace(configuredApiKey) ||
            !string.Equals(
                providedApiKey,
                configuredApiKey,
                StringComparison.Ordinal))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsJsonAsync(new
            {
                error = "Invalid API key."
            });
            return;
        }
        await _next(context);
    }
}