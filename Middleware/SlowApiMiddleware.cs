using System.Diagnostics;

namespace EmployeeManagementApi.Middleware
{
    public class SlowApiMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<SlowApiMiddleware> _logger;

        private const double Threshold = 0.000001;

        public SlowApiMiddleware(RequestDelegate next, ILogger<SlowApiMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var watch = Stopwatch.StartNew();

            await _next(context);

            watch.Stop();

            var elapsed = watch.ElapsedMilliseconds;

            if (elapsed >= Threshold)
            {
                _logger.LogWarning("Slow API | Method: {Method} | Path: {Path} | Status: {StatusCode} | Time: {Elapsed} ms",    
                                   context.Request.Method, context.Request.Path, context.Response.StatusCode, elapsed);
            }
        }
    }
}