using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ApiApplication.Middlewares
{
    public class RequestTimeLoggingMiddleware : IMiddleware
    {
        private readonly ILogger<RequestTimeLoggingMiddleware> logger;

        public RequestTimeLoggingMiddleware(ILogger<RequestTimeLoggingMiddleware> logger)
        {
            this.logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var stopwatch = Stopwatch.StartNew();

            context.Response.OnCompleted(() =>
            {
                stopwatch.Stop();
                logger.LogInformation($"Request {context.Request.Method} {context.Request.Path} executed in {stopwatch.ElapsedMilliseconds}ms");

                return Task.CompletedTask;
            });

            await next(context);
        }
    }
}