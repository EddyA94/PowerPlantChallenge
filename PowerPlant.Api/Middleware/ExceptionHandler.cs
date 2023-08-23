using Newtonsoft.Json;
using System.Net;

namespace PowerPlant.Api.Middleware
{
    public class ExceptionHandler : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                // Log the exception
                // Create a custom error response
                var errorResponse = new ErrorResponse
                {
                    ErrorCode = "500",
                    Message = "An error occurred while processing your request.",
                    InnerMessage = ex.Message
                };

                var json = JsonConvert.SerializeObject(errorResponse);

                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                context.Response.ContentType = "application/json";

                await context.Response.WriteAsync(json);
            }
        }
    }

    public class ErrorResponse
    {
        public string? ErrorCode { get; set; }
        public string? Message { get; set; }
        public string? InnerMessage { get; set; }
    }
}
