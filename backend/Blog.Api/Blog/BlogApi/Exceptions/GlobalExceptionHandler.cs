using Microsoft.AspNetCore.Diagnostics;

namespace BlogApi.Exceptions
{
    public class GlobalExceptionHandler : IExceptionHandler//apideki hataları yakalayacak bir exception handler.
    {
        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            if (exception is NotFoundException)
            {
                httpContext.Response.StatusCode = StatusCodes.Status404NotFound;

                await httpContext.Response.WriteAsJsonAsync(
                    new
                    {
                        message = exception.Message
                    },
                    cancellationToken
                );

                return true;
            }

            if (exception is BadRequestException)
            {
                httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;

                await httpContext.Response.WriteAsJsonAsync(
                    new
                    {
                        message = exception.Message
                    },
                    cancellationToken
                );

                return true;
            }

            if (exception is UnauthorizedException)
            {
                httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;

                await httpContext.Response.WriteAsJsonAsync(
                    new
                    {
                        message = exception.Message
                    },
                    cancellationToken
                );

                return true;
            }


            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

            await httpContext.Response.WriteAsJsonAsync(
                new
                {
                    message = "Beklenmeyen bir sunucu hatası oluştu."
                },
                cancellationToken
            );

            return true;
        }
    }
}