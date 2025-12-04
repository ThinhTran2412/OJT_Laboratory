using Microsoft.AspNetCore.Http;

namespace SharedLibrary.Middleware
{
    /// <summary>
    /// Middleware used to block all requests that do not go through the API Gateway.
    /// Idea:
    /// - When implementing a microservices system, each backend service should only be accessed through the API Gateway.
    /// - This middleware will check if the request contains a special header attached by the API Gateway.
    /// - If there is no valid header, the middleware will return a 503 (Service Unavailable) error.
    /// </summary>
    public class ListenToOnlyApiGateway(RequestDelegate next)
    {
        /// <summary>
        /// Invokes the asynchronous.
        /// </summary>
        /// <param name="context">The context.</param>
        public async Task InvokeAsync(HttpContext context)
        {
            var signedHeader = context.Request.Headers["Api-Gateway"];

            if(signedHeader.FirstOrDefault() is null)
            {
                context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
                await context.Response.WriteAsync("Service is unvailable");
                return;
            }
            else
            {
                await next(context);
            }
        }
    }
}
