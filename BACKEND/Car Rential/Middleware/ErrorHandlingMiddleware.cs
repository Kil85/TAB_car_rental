using Car_Rential.Exceptions;
using Car_Rential.Interfaces;
using Car_Rential.Model.Validators;
using FluentValidation;

namespace Car_Rential.Middleware
{
    public class ErrorHandlingMiddleware : IMiddleware
    {
        private readonly ILogger _logger;

        public ErrorHandlingMiddleware(ILogger<ErrorHandlingMiddleware> logger)
        {
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next.Invoke(context);
            }
            //catch (CarNotFoudException ex)
            //{
            //    context.Response.StatusCode = 404;
            //    await context.Response.WriteAsync(ex.Message);
            //}
            //catch (LoginFailException ex)
            //{
            //    context.Response.StatusCode = 404;
            //    await context.Response.WriteAsync(ex.Message);
            //}
            catch (ValidationException ex)
            {
                context.Response.StatusCode = 401;
                string response = "";
                foreach (var error in ex.Errors)
                {
                    response += error.ErrorMessage;
                }
                await context.Response.WriteAsync(response);
            }
            //catch (CustomerNotFoundException ex)
            //{
            //    context.Response.StatusCode = 404;
            //    await context.Response.WriteAsync(ex.Message);
            //}
            catch (BaseException ex)
            {
                context.Response.StatusCode = ex.statusCode;
                await context.Response.WriteAsync($"Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                context.Response.StatusCode = 500;
                await context.Response.WriteAsync(ex.Message);
            }
        }
    }
}
