var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Enable API Explorer & Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Read AllowedOrigins from appsettings.json
var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() ?? new string[] { };

Console.WriteLine("Configured CORS Origins:");
if (allowedOrigins.Length > 0)
{
    foreach (var origin in allowedOrigins)
    {
        Console.WriteLine(origin);
    }
}
else
{
    Console.WriteLine("No CORS origins found in configuration.");
}

// Configure CORS dynamically from appsettings.json
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
        policy.WithOrigins(allowedOrigins)
              .AllowAnyMethod()
              .AllowAnyHeader()
    );
});

var app = builder.Build();

Console.WriteLine("Application starting...");
app.UseMiddleware<ErrorHandlingMiddleware>(); // First, handle exceptions
app.UseMiddleware<RequestResponseLoggingMiddleware>(); // Then, log requests/responses

// Enable Swagger in Development
if (app.Environment.IsDevelopment())
{
    Console.WriteLine("Enabling Swagger UI...");
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Apply CORS Policy
app.UseCors("CorsPolicy");
Console.WriteLine("CORS settings applied successfully.");


// Middleware to confirm request-response logging
app.Use(async (context, next) =>
{
    Console.WriteLine("Request-Response Middleware executed.");
    await context.Response.WriteAsync("Request Response Middleware working fine\n");
    await next();
});

// Test Route to Trigger Error Middleware
app.MapGet("/error", (HttpContext context) =>
{
    Console.WriteLine("Exception Middleware triggered.");
    throw new Exception("Test Exception");
});

// Map controllers
app.UseAuthorization();
app.MapControllers();

Console.WriteLine("Application is now running...");
app.Run();
