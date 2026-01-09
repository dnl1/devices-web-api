using Devices.Infrastructure;
using System.Text;

namespace DeviceWebApi.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, ApplicationDbContext dbContext)
        {
            string body = string.Empty;

            try
            {
                context.Request.EnableBuffering();

                if (context.Request.ContentLength > 0 &&
                    context.Request.Body.CanSeek)
                {
                    context.Request.Body.Seek(0, SeekOrigin.Begin);
                    using var reader = new StreamReader(
                        context.Request.Body,
                        encoding: Encoding.UTF8,
                        detectEncodingFromByteOrderMarks: false,
                        leaveOpen: true
                    );
                    body = await reader.ReadToEndAsync();
                    context.Request.Body.Seek(0, SeekOrigin.Begin);
                }

                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception occurred");

                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsync("An unexpected error occurred.");
            }
        }
    }
}
