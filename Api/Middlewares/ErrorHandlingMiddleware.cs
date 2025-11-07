using Api.DTO;
using System.Net;
using System.Text.Json;

namespace Api.Middlewares
{
	public class ErrorHandlingMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly ILogger<ErrorHandlingMiddleware> _logger;

		public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
		{
			_next = next;
			_logger = logger;
		}

		public async Task InvokeAsync(HttpContext context)
		{
			try
			{
				await _next(context);
			}
			catch (Exception ex)
			{
				await HandleExceptionAsync(context, ex);
			}
		}

		private async Task HandleExceptionAsync(HttpContext context, Exception ex)
		{
			_logger.LogError(ex, "Unhandled exception caught in middleware");

			var statusCode = ex switch
			{
				ArgumentException => HttpStatusCode.BadRequest,
				KeyNotFoundException => HttpStatusCode.NotFound,
				UnauthorizedAccessException => HttpStatusCode.Unauthorized,
				_ => HttpStatusCode.InternalServerError
			};

			var response = new ApiResponse<string>(
				Success: false,
				Message: ex.Message
			);

			context.Response.ContentType = "application/json";
			context.Response.StatusCode = (int)statusCode;

			var options = new JsonSerializerOptions
			{
				PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
				WriteIndented = true
			};

			await context.Response.WriteAsync(JsonSerializer.Serialize(response, options));
		}
	}
}
