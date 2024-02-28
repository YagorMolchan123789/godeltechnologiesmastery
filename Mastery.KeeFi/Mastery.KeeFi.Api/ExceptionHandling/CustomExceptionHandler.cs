using Mastery.KeeFi.Common.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;

namespace Mastery.KeeFi.Api.ExceptionHandling
{
    public sealed class CustomExceptionHandler
    {
        public async Task Invoke(HttpContext httpContext)
        {
            IExceptionHandlerFeature? exceptionDetails = httpContext.Features.Get<IExceptionHandlerFeature>();
            Exception? exception = exceptionDetails?.Error;

            if (exception != null && exception.GetType().IsSubclassOf(typeof(DocumentApiBaseException)))
            {
                await HandleDocumentApiCustomException(httpContext, exception);
            }
        }

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

        private static async Task HandleEntityNotFoundException(HttpContext context, Exception ex)
        {
            IProblemDetailsService problemDetailsService = context.RequestServices.GetRequiredService<IProblemDetailsService>();
            IExceptionHandlerFeature? exceptionDetails = context.Features.Get<IExceptionHandlerFeature>();

            context.Response.StatusCode = 404;

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

        private static async Task HandleValidationException(HttpContext context, Exception ex)
        {
            IProblemDetailsService problemDetailsService = context.RequestServices.GetRequiredService<IProblemDetailsService>();
            IExceptionHandlerFeature? exceptionDetails = context.Features.Get<IExceptionHandlerFeature>();

            context.Response.StatusCode = 400;

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

        private static async Task HandleGeneralException(HttpContext context, Exception ex)
        {
            IProblemDetailsService problemDetailsService = context.RequestServices.GetRequiredService<IProblemDetailsService>();
            IExceptionHandlerFeature? exceptionDetails = context.Features.Get<IExceptionHandlerFeature>();

            context.Response.StatusCode = 500;

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
