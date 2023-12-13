using ApiApplication.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace ApiApplication.Middlewares
{
    public class ExceptionHandlingMiddleware : IMiddleware
    {
        private readonly ILogger<ExceptionHandlingMiddleware> logger;

        public ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger)
        {
            this.logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            async Task WriteError(object errorModel, int statusCode, Exception ex)
            {
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                logger.LogError(ex, ex.Message);
                var json = JsonSerializer.Serialize(errorModel);
                await context.Response.WriteAsync(json);
            }

            object errorModel = null;

            try
            {
                await next(context);
            }
            catch (ReservationException ex)
            {
                errorModel = new { ExceptionType = nameof(ReservationException), Message = ex.Message };
                await WriteError(errorModel, StatusCodes.Status400BadRequest, ex);
            }
            catch (ContigiousSeatsException ex)
            {
                errorModel = new { ExceptionType = nameof(ContigiousSeatsException), Message = ex.Message };
                await WriteError(errorModel, StatusCodes.Status500InternalServerError, ex);
            }
            catch (Exception ex)
            {
                errorModel = new { ExceptionType = nameof(Exception), Message = ex.Message };
                await WriteError(errorModel, StatusCodes.Status500InternalServerError, ex);
            }
        }
    }
}