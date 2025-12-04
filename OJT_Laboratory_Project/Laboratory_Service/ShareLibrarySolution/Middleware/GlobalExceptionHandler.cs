using System.Text.Json;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SharedLibrary.Middleware
{
    /// <summary>
    /// Middleware is responsible for catching and handling all exceptions 
    /// occurring in the application pipeline. 
    /// This class helps: 
    /// - Standardize error responses in JSON format. 
    /// - Clearly distinguish error types (Validation, Unauthorized, NotFound, InternalServerError,...) 
    /// - Easily log and debug in real environments.
    /// </summary>
    public class GlobalExceptionHandler
    {
        /// <summary>
        /// The next
        /// </summary>
        private readonly RequestDelegate _next;

        /// <summary>
        /// Initializes a new instance of the <see cref="GlobalExceptionHandler"/> class.
        /// </summary>
        /// <param name="next">The next.</param>
        public GlobalExceptionHandler(RequestDelegate next)
        {
            _next = next;
        }

        /// <summary>
        /// Invokes the asynchronous.
        /// </summary>
        /// <param name="context">The context.</param>
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);

                // Xử lý các status code đã trả từ pipeline
                switch (context.Response.StatusCode)
                {
                    // comment because we handle these in custom exceptions

                    case StatusCodes.Status429TooManyRequests:
                        await WriteProblemDetails(context, "Warning", "Too many requests made!", StatusCodes.Status429TooManyRequests);
                        break;
                    case StatusCodes.Status401Unauthorized:
                        await WriteProblemDetails(context, "Unauthorized", "You are not authorized to access this resource.", StatusCodes.Status401Unauthorized);
                        break;
                }
            }
            catch (ValidationException ex)
            {
                // FluentValidation errors grouped by property
                var errors = ex.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.ErrorMessage).ToArray()
                    );

                await WriteProblemDetails(context,
                    "Bad Request",
                    "One or more validation errors occurred.",
                    StatusCodes.Status400BadRequest,
                    ex,
                    errors);
            }
            catch (UnauthorizedAccessException ex)
            {
                await WriteProblemDetails(context,
                    "Unauthorized",
                    "You are not authorized to access this resource.",
                    StatusCodes.Status401Unauthorized,
                    ex);
            }
            catch (KeyNotFoundException ex)
            {
                await WriteProblemDetails(context,
                    "Not Found",
                    ex.Message,
                    StatusCodes.Status404NotFound,
                    ex);
            }
            catch (Exception ex)
            {
                await WriteProblemDetails(context,
                    "Internal Server Error",
                    "Sorry, internal server error occurred. Try again!",
                    StatusCodes.Status500InternalServerError,
                    ex);
            }
        }

        /// <summary>
        /// Writes the problem details.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="title">The title.</param>
        /// <param name="message">The message.</param>
        /// <param name="statusCode">The status code.</param>
        /// <param name="ex">The ex.</param>
        /// <param name="errors">The errors.</param>
        private async Task WriteProblemDetails(
            HttpContext context,
            string title,
            string message,
            int statusCode,
            Exception? ex = null,
            IDictionary<string, string[]>? errors = null,
            IDictionary<string, object>? extensions = null)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            var problemDetails = new ProblemDetails
            {
                Title = title,
                Detail = message,
                Status = statusCode
            };

            if (ex != null)
                problemDetails.Extensions["ExceptionType"] = ex.GetType().Name;

            if (errors != null)
                problemDetails.Extensions["Errors"] = errors;

            if (extensions != null)
            {
                foreach (var kvp in extensions)
                {
                    // overwrite or add extension entries
                    problemDetails.Extensions[kvp.Key] = kvp.Value;
                }
            }

            await context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails, new JsonSerializerOptions
            {
                WriteIndented = true
            }));
        }
    }
}
