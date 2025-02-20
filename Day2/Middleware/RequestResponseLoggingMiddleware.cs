using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

public class RequestResponseLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestResponseLoggingMiddleware> _logger;

    public RequestResponseLoggingMiddleware(RequestDelegate next, ILogger<RequestResponseLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        Console.WriteLine($"[Middleware] Incoming Request: {context.Request.Method} {context.Request.Path}");
        _logger.LogInformation($"Incoming Request: {context.Request.Method} {context.Request.Path}");

        var originalResponseBody = context.Response.Body;
        using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;

        await _next(context);

        responseBody.Seek(0, SeekOrigin.Begin);
        var responseText = await new StreamReader(responseBody).ReadToEndAsync();

        Console.WriteLine($"[Middleware] Response: {context.Response.StatusCode} - {responseText}");
        _logger.LogInformation($"Response: {context.Response.StatusCode} {responseText}");

        responseBody.Seek(0, SeekOrigin.Begin);
        await responseBody.CopyToAsync(originalResponseBody);
        context.Response.Body = originalResponseBody;
    }

}
