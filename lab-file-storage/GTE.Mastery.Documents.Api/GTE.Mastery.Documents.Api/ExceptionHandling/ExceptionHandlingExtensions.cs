namespace GTE.Mastery.Documents.Api.ExceptionHandling
{
    public static class ExceptionHandlingExtensions
    {
        public static IApplicationBuilder UseCustomExceptionHandler(this IApplicationBuilder app) =>
            app.UseExceptionHandler(new ExceptionHandlerOptions
            {
                ExceptionHandler = new CustomExceptionHandler().Invoke
            });
    }
}
