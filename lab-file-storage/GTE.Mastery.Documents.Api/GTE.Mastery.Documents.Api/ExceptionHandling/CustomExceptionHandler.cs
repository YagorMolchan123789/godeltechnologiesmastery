using GTE.Mastery.Documents.Api.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace GTE.Mastery.Documents.Api.ExceptionHandling
{
    /// <summary>
    /// Handles exceptions that occur within the application.
    /// </summary>
    public sealed class CustomExceptionHandler
    {
        /// <summary>
        /// Invoked when an exception occurs.
        /// </summary>
        /// <param name="httpContext">The current HTTP context.</param>
        public async Task Invoke(HttpContext httpContext)
        {
            // Get the details of the exception.
            IExceptionHandlerFeature? exceptionDetails = httpContext.Features.Get<IExceptionHandlerFeature>();
            Exception? exception = exceptionDetails?.Error;

            // If there is an exception and it is a subclass of DocumentApiBaseException, handle it.
            if (exception != null && exception.GetType().IsSubclassOf(typeof(DocumentApiBaseException)))
            {
                await HandleDocumentApiCustomException(httpContext, exception);
            }
        }

        /// <summary>
        /// Handles exceptions that are subclasses of DocumentApiBaseException.
        /// </summary>
        /// <param name="context">The current HTTP context.</param>
        /// <param name="ex">The exception that occurred.</param>
        private async Task HandleDocumentApiCustomException(HttpContext context, Exception ex)
        {
            var typeName = ex.GetType().Name.ToLowerInvariant();

            switch (typeName)
            {
                case "documentapientitynotfoundexception":
                    await HandleEntityNotFoundException(context, ex);
                    break;
                case "documentapivalidationexception":
                    await HandleValidationException(context, ex);
                    break;
                default:
                    await HandleGeneralException(context, ex);
                    break;
            }
        }

        /// <summary>
        /// Handles exceptions of type DocumentApiEntityNotFoundException.
        /// Sets the HTTP status code to 404 (Not Found).
        /// </summary>
        /// <param name="context">The current HTTP context.</param>
        /// <param name="ex">The exception that occurred.</param>
        private static async Task HandleEntityNotFoundException(HttpContext context, Exception ex)
        {
            IProblemDetailsService problemDetailsService = context.RequestServices.GetRequiredService<IProblemDetailsService>();
            IExceptionHandlerFeature? exceptionDetails = context.Features.Get<IExceptionHandlerFeature>();

            // Set the HTTP status code to 404.
            context.Response.StatusCode = 404;
            // Write the problem details to the response.
            await problemDetailsService.WriteAsync(new ProblemDetailsContext
            {
                HttpContext = context,
                AdditionalMetadata = exceptionDetails?.Endpoint?.Metadata,
                ProblemDetails = new ProblemDetails
                {
                    Status = 404,
                    Detail = ex.Message,
                    Title = "The specified entity has not been found."
                }
            });
        }

        /// <summary>
        /// Handles exceptions of type DocumentApiValidationException.
        /// Sets the HTTP status code to 400 (Bad Request).
        /// </summary>
        /// <param name="context">The current HTTP context.</param>
        /// <param name="ex">The exception that occurred.</param>
        private static async Task HandleValidationException(HttpContext context, Exception ex)
        {
            IProblemDetailsService problemDetailsService = context.RequestServices.GetRequiredService<IProblemDetailsService>();
            IExceptionHandlerFeature? exceptionDetails = context.Features.Get<IExceptionHandlerFeature>();

            // Set the HTTP status code to 400.
            context.Response.StatusCode = 400;
            // Write the problem details to the response.
            await problemDetailsService.WriteAsync(new ProblemDetailsContext
            {
                HttpContext = context,
                AdditionalMetadata = exceptionDetails?.Endpoint?.Metadata,
                ProblemDetails = new ProblemDetails
                {
                    Status = 400,
                    Detail = ex.Message,
                    Title = "The provided entity is invalid."
                }
            });
        }

        /// <summary>
        /// Handles all other exceptions that are subclasses of DocumentApiBaseException.
        /// Sets the HTTP status code to 500 (Internal Server Error).
        /// </summary>
        /// <param name="context">The current HTTP context.</param>
        /// <param name="ex">The exception that occurred.</param>
        private static async Task HandleGeneralException(HttpContext context, Exception ex)
        {
            IProblemDetailsService problemDetailsService = context.RequestServices.GetRequiredService<IProblemDetailsService>();
            IExceptionHandlerFeature? exceptionDetails = context.Features.Get<IExceptionHandlerFeature>();

            // Set the HTTP status code to 500.
            context.Response.StatusCode = 500;
            // Write the problem details to the response.
            await problemDetailsService.WriteAsync(new ProblemDetailsContext
            {
                HttpContext = context,
                AdditionalMetadata = exceptionDetails?.Endpoint?.Metadata,
                ProblemDetails = new ProblemDetails
                {
                    Status = 500,
                    Detail = ex.Message,
                    Title = "The unknown document api custom error has occured."
                }
            });
        }
    }
}
