using System.Diagnostics;
using Serilog;
using Serilog.Context;
using ILogger = Serilog.ILogger;

namespace StudentManagementAPI.Services.Infrastructure.Logging
{
    public class PerformanceLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public PerformanceLoggingMiddleware(RequestDelegate next, ILogger logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var requestId = Guid.NewGuid().ToString();
            var stopwatch = Stopwatch.StartNew();

            // Add request context to all logs
            using (LogContext.PushProperty("RequestId", requestId))
            using (LogContext.PushProperty("RequestPath", context.Request.Path))
            using (LogContext.PushProperty("RequestMethod", context.Request.Method))
            using (LogContext.PushProperty("UserAgent", context.Request.Headers.UserAgent.ToString()))
            {
                try
                {
                    // Log request start
                    _logger.Information(
                        "HTTP {RequestMethod} {RequestPath} started",
                        context.Request.Method,
                        context.Request.Path);

                    await _next(context);

                    stopwatch.Stop();

                    // Log request completion
                    _logger.Information(
                        "HTTP {RequestMethod} {RequestPath} completed with {StatusCode} in {ElapsedMs}ms",
                        context.Request.Method,
                        context.Request.Path,
                        context.Response.StatusCode,
                        stopwatch.ElapsedMilliseconds);

                    // Log slow requests
                    if (stopwatch.ElapsedMilliseconds > 1000)
                    {
                        _logger.Warning(
                            "SLOW REQUEST: {RequestMethod} {RequestPath} took {ElapsedMs}ms",
                            context.Request.Method,
                            context.Request.Path,
                            stopwatch.ElapsedMilliseconds);
                    }
                }
                catch (Exception ex)
                {
                    stopwatch.Stop();

                    _logger.Error(ex,
                        "HTTP {RequestMethod} {RequestPath} failed after {ElapsedMs}ms",
                        context.Request.Method,
                        context.Request.Path,
                        stopwatch.ElapsedMilliseconds);

                    throw;
                }
            }
        }
    }

    public static class PerformanceLoggingMiddlewareExtensions
    {
        public static IApplicationBuilder UsePerformanceLogging(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<PerformanceLoggingMiddleware>();
        }
    }
}