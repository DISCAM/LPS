using LabelPrintingSystemApi_1._0.Exceptions;

namespace LabelPrintingSystemApi_1._0.Middleware
{
    public class ErrorHandlingMiddleware : IMiddleware
    {
        private readonly ILogger<ErrorHandlingMiddleware> logger;

        public ErrorHandlingMiddleware(ILogger<ErrorHandlingMiddleware> logger )
        {
            this.logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                //Console.WriteLine("Przed kontrolerem");
                await next.Invoke(context);
                //Console.WriteLine("Po kontrolerze");
            }
            catch (BadRequestException exception)
            {
                logger.LogError(exception, "Exception occurred - 400 Bad Request");
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                context.Response.ContentType = "text/plain";

                await context.Response.WriteAsync(exception.Message);
            }
            catch (NotFoundException exception)
            {
                logger.LogError(exception, "Exception occurred - 404 Not Found");
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                context.Response.ContentType = "text/plain";
                await context.Response.WriteAsync(exception.Message);
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Unhandled server exception occurred");
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "text/plain";
                await context.Response.WriteAsync("Coś poszło nie tak !!");
            }
        }
    }
}
