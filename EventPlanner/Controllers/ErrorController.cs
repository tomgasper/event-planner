using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Diagnostics;
using System;
using EventPlanner.Models.Error;
using Microsoft.AspNetCore.Authorization;
using EventPlanner.Exceptions;

namespace EventPlanner.Controllers
{
    [AllowAnonymous]
	public class ErrorController : Controller
	{
        private readonly ILogger _logger;
        public ErrorController (ILogger<ErrorController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            var exceptionHandlerPathFeature = HttpContext.Features.Get<IExceptionHandlerFeature>();

            Exception? exception = exceptionHandlerPathFeature?.Error;

            string? message;
            string? detail;

            switch (exception)
            {
                case null:
                    message = "Undefined error";
                    detail = String.Empty;
                    _logger.LogWarning("Hit the error page with no exception information.");
                    break;
                case UserManagementException:
                    message = "Invalid user management operation";
                    detail = exception.Message;
                    break;
                case InvalidInputException:
                    message = "Invalid input provided";
                    detail = exception.Message;
                    _logger.LogWarning("Invalid input exception: {ExceptionMessage}", exception.Message);
                    break;
                case NotFoundException:
                    message = "Not found";
                    detail = exception.Message;
                    _logger.LogWarning("Not found exception: {ExceptionMessage}", exception.Message);
                    break;
                case Exception:
                    message = "An unexpected error occurred";
                    detail = String.Empty;
                    _logger.LogError(exception, "An unexpected exception occurred.");
                    break;
            };

            var model = new ErrorViewModel()
            {
                Message = message,
                Detail = detail
            };

            return View(model);
		}
	}
}
