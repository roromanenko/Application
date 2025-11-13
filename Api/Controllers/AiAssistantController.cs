using Api.DTO;
using Core.Domain;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Api.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	[Authorize]
	public class AiAssistantController : ControllerBase
	{
		private readonly IAiAssistantService _aiService;
		private readonly ISqlExecutor _sqlExecutor;
		private readonly ILogger<AiAssistantController> _logger;

		public AiAssistantController(
			IAiAssistantService aiService,
			ISqlExecutor sqlExecutor,
			ILogger<AiAssistantController> logger)
		{
			_aiService = aiService;
			_sqlExecutor = sqlExecutor;
			_logger = logger;
		}


		[HttpPost("query")]
		[ProducesResponseType(typeof(AiAssistantResponse), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
		[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<AiAssistantResponse>> ProcessQuery(
			[FromBody] AiQueryRequest request,
			CancellationToken cancellationToken)
		{
			if (string.IsNullOrWhiteSpace(request.Query))
			{
				return BadRequest(new ProblemDetails
				{
					Title = "Invalid Query",
					Detail = "Query cannot be empty",
					Status = StatusCodes.Status400BadRequest
				});
			}

			try
			{
				var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

				if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
				{
					_logger.LogWarning("Invalid user ID in token");
					return Unauthorized();
				}

				_logger.LogInformation(
					"Processing AI query for user {UserId}: {Query}",
					userId,
					request.Query);

				var aiResult = await _aiService.ProcessUserQueryAsync(
					request.Query,
					userId,
					cancellationToken);

				if (!aiResult.IsValid)
				{
					_logger.LogWarning(
						"AI could not generate valid SQL for query: {Query}. Notes: {Notes}",
						request.Query,
						aiResult.Notes);

					return Ok(new AiAssistantResponse
					{
						Success = false,
						Answer = aiResult.Notes ?? "I couldn't understand your request. Could you please rephrase it?",
						Query = null,
						Explanation = aiResult.Explanation,
						Data = null,
						ResultCount = 0,
						ErrorMessage = "Unable to generate SQL query"
					});
				}

				_logger.LogInformation("Generated SQL: {Sql}", aiResult.Sql);

				var data = await _sqlExecutor.ExecuteRawQueryAsync(
					aiResult.Sql,
					cancellationToken);

				_logger.LogInformation(
					"Query executed successfully. Returned {Count} rows",
					data.Count);

				var humanResponse = await _aiService.GenerateHumanResponseAsync(
					request.Query,
					aiResult.Sql,
					data,
					cancellationToken);

				_logger.LogInformation("Generated human response: {Response}", humanResponse);

				return Ok(new AiAssistantResponse
				{
					Success = true,
					Answer = humanResponse,
					Query = aiResult.Sql,
					Explanation = aiResult.Explanation,
					ResultCount = data.Count,
					ErrorMessage = null
				});
			}
			catch (InvalidOperationException ex) when (ex.Message.Contains("Only SELECT queries are allowed"))
			{
				_logger.LogWarning(ex, "Attempted to execute non-SELECT query");

				return BadRequest(new ProblemDetails
				{
					Title = "Invalid Query",
					Detail = "Only SELECT queries are allowed for security reasons",
					Status = StatusCodes.Status400BadRequest
				});
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error processing AI assistant query: {Query}", request.Query);

				return StatusCode(
					StatusCodes.Status500InternalServerError,
					new ProblemDetails
					{
						Title = "Internal Server Error",
						Detail = "An error occurred while processing your request. Please try again later.",
						Status = StatusCodes.Status500InternalServerError
					});
			}
		}
	}
}
