using AddressBook.Domain.Exceptions;
using System.Net;
using System.Text.Json;

namespace AddressBook.API.Middleware
{
    /// <summary>
    /// Catches anything that escapes the pipeline, logs it, and returns a consistent
    /// <c>{ "message": "..." }</c> body instead of an HTML error page or an empty response.
    /// </summary>
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        private readonly IHostEnvironment _environment;

        public ExceptionHandlingMiddleware(
            RequestDelegate next,
            ILogger<ExceptionHandlingMiddleware> logger,
            IHostEnvironment environment)
        {
            _next = next;
            _logger = logger;
            _environment = environment;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Unhandled exception for {Method} {Path}",
                    context.Request.Method,
                    context.Request.Path);

                await WriteResponseAsync(context, ex);
            }
        }

        private async Task WriteResponseAsync(HttpContext context, Exception ex)
        {
            if (context.Response.HasStarted)
            {
                // Headers are already on the wire; rewriting the response would corrupt it.
                _logger.LogWarning("Response already started; cannot write the error payload.");
                return;
            }

            var statusCode = MapStatusCode(ex);

            // Only leak exception detail for faults the caller caused, or when running locally.
            var message = statusCode == HttpStatusCode.InternalServerError && !_environment.IsDevelopment()
                ? "An unexpected error occurred."
                : ex.Message;

            context.Response.Clear();
            context.Response.StatusCode = (int)statusCode;
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsync(JsonSerializer.Serialize(new { message }));
        }

        private static HttpStatusCode MapStatusCode(Exception ex) => ex switch
        {
            UnauthorizedAccessException => HttpStatusCode.Unauthorized,
            KeyNotFoundException => HttpStatusCode.NotFound,
            ConflictException => HttpStatusCode.Conflict,
            ArgumentException => HttpStatusCode.BadRequest,
            _ => HttpStatusCode.InternalServerError
        };
    }
}
