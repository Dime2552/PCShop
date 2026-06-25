using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PCShop.Application.Common.Exceptions;

namespace PCShop.WebApi.Middlewares
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            // Loging only 500 errors as critical, others as warnings
            if (exception is not BadRequestException && exception is not NotFoundException && exception is not ValidationException)
            {
                _logger.LogError(exception, "An unhandled exception occurred: {Message}", exception.Message);
            }
            else
            {
                _logger.LogWarning("Business exception occurred: {Message}", exception.Message);
            }

            var problemDetails = new ProblemDetails
            {
                Instance = httpContext.Request.Path
            };

            // Handle FluentValidation Errors (400)
            if (exception is ValidationException validationException)
            {
                problemDetails.Title = "Validation Failed";
                problemDetails.Status = StatusCodes.Status400BadRequest;
                problemDetails.Detail = "One or more validation errors occurred.";

                // Map validation errors into the ProblemDetails extensions
                var errors = validationException.Errors
                    .GroupBy(x => x.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(x => x.ErrorMessage).ToArray()
                    );

                problemDetails.Extensions.Add("errors", errors);
                httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
            // Handle Business Logic Errors (400)
            else if (exception is BadRequestException badRequestException)
            {
                problemDetails.Title = "Bad Request";
                problemDetails.Status = StatusCodes.Status400BadRequest;
                problemDetails.Detail = badRequestException.Message; // Return business message

                httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
            // Handle Not Found Errors (404)
            else if (exception is NotFoundException notFoundException)
            {
                problemDetails.Title = "Not Found";
                problemDetails.Status = StatusCodes.Status404NotFound;
                problemDetails.Detail = notFoundException.Message;

                httpContext.Response.StatusCode = StatusCodes.Status404NotFound;
            }
            // Handle all other unexpected errors (500)
            else
            {
                problemDetails.Title = "Internal Server Error";
                problemDetails.Status = StatusCodes.Status500InternalServerError;
                problemDetails.Detail = "An unexpected error occurred. Please try again later.";

                httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            }

            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

            return true;
        }
    }
}
