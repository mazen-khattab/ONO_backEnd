using System.Text.Json;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace ONO.API.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var errorResponse = new
            {
                context.Response.StatusCode,
                exception.Message,
                InnerMessage = exception.InnerException?.Message,
                exception.StackTrace
            };

            var errorJson = JsonSerializer.Serialize(errorResponse);

            return context.Response.WriteAsync(errorJson);
        }
    }
}
