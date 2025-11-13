using Core.Domain;
using Core.Interfaces;
using Core.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Reflection;
using System.Text.Json;

namespace Infrastructure.AI
{
	public class GroqAiService : IAiAssistantService
	{
		private readonly HttpClient _httpClient;
		private readonly GroqOptions _options;
		private readonly string _sqlPromptTemplate;
		private readonly string _humanResponsePromptTemplate;
		private readonly ILogger<GroqAiService> _logger;

		public GroqAiService(
			HttpClient httpClient,
			IOptions<GroqOptions> options,
			ILogger<GroqAiService> logger)
		{
			_httpClient = httpClient;
			_options = options.Value;
			_logger = logger;

			_sqlPromptTemplate = LoadEmbeddedPrompt("sql_generator_prompt.txt");
			_humanResponsePromptTemplate = LoadEmbeddedPrompt("human_response_prompt.txt");

			ValidateConfiguration();
		}

		public async Task<AiQueryResult> ProcessUserQueryAsync(
			string userQuery,
			Guid userId,
			CancellationToken cancellationToken = default)
		{
			try
			{
				var prompt = BuildPromptWithUserQuery(userQuery, userId);
				var response = await CallGroqApiAsync(prompt, cancellationToken);
				return ParseAiResponse(response);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error processing AI query");
				throw;
			}
		}

		public async Task<string> GenerateHumanResponseAsync(
			string userQuery,
			string sqlQuery,
			List<Dictionary<string, object>> data,
			CancellationToken cancellationToken = default)
		{
			try
			{
				var dataJson = JsonSerializer.Serialize(data, new JsonSerializerOptions
				{
					WriteIndented = true,
					Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
				});

				var prompt = _humanResponsePromptTemplate
					.Replace("{USER_QUERY}", userQuery)
					.Replace("{SQL_QUERY}", sqlQuery)
					.Replace("{RESULT_COUNT}", data.Count.ToString())
					.Replace("{DATA_JSON}", dataJson);

				_logger.LogDebug("Generating human response for query: {Query}", userQuery);

				var response = await CallGroqApiAsync(prompt, cancellationToken, isHumanResponse: true);

				_logger.LogInformation("Human response generated successfully");

				return response.Trim();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error generating human response");
				throw;
			}
		}

		private void ValidateConfiguration()
		{
			if (string.IsNullOrWhiteSpace(_options.ApiKey))
			{
				throw new InvalidOperationException(
					"Groq API key is not configured. Please set it in appsettings.json");
			}
		}

		private string LoadEmbeddedPrompt(string fileName)
		{
			var assembly = Assembly.GetExecutingAssembly();
			var resourceName = $"Infrastructure.Prompts.{fileName}";

			using var stream = assembly.GetManifestResourceStream(resourceName);

			if (stream == null)
			{
				var availableResources = assembly.GetManifestResourceNames();
				_logger.LogError(
					"Prompt resource '{FileName}' not found. Available resources: {Resources}",
					fileName,
					string.Join(", ", availableResources));

				throw new FileNotFoundException(
					$"Embedded resource '{resourceName}' not found. " +
					$"Available resources: {string.Join(", ", availableResources)}");
			}

			using var reader = new StreamReader(stream);
			return reader.ReadToEnd();
		}

		private string BuildPromptWithUserQuery(string userQuery, Guid userId)
		{
			return _sqlPromptTemplate.Replace(
				"[=== USER REQUEST ===]",
				$"[=== USER REQUEST ===]\n{userQuery}\nCurrent user ID: {userId}");
		}

		private async Task<string> CallGroqApiAsync(
			string prompt,
			CancellationToken cancellationToken,
			bool isHumanResponse = false)
		{
			var messages = isHumanResponse
				? new[] { new { role = "user", content = prompt } }
				: new[]
				{
					new { role = "system", content = prompt },
					new { role = "user", content = "Generate SQL query for this request." }
				};

			var request = new
			{
				model = _options.Model,
				messages = messages,
				temperature = isHumanResponse ? 0.7 : _options.Temperature,
				max_tokens = _options.MaxTokens
			};

			var httpRequest = new HttpRequestMessage(
				HttpMethod.Post,
				$"{_options.BaseUrl}")
			{
				Content = JsonContent.Create(request),
				Headers =
				{
					Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(
						"Bearer",
						_options.ApiKey)
				}
			};

			var response = await _httpClient.SendAsync(httpRequest, cancellationToken);
			response.EnsureSuccessStatusCode();

			var content = await response.Content.ReadAsStringAsync(cancellationToken);
			var jsonResponse = JsonSerializer.Deserialize<JsonElement>(content);

			return jsonResponse
				.GetProperty("choices")[0]
				.GetProperty("message")
				.GetProperty("content")
				.GetString();
		}

		private AiQueryResult ParseAiResponse(string response)
		{
			var jsonString = response.Trim();

			if (jsonString.StartsWith("```json", StringComparison.OrdinalIgnoreCase))
			{
				jsonString = jsonString.Substring(7).Trim();
			}
			if (jsonString.EndsWith("```"))
			{
				jsonString = jsonString[..^3].Trim();
			}

			int startIndex = jsonString.IndexOf('{');
			if (startIndex > 0)
			{
				jsonString = jsonString[startIndex..].Trim();
			}

			int endIndex = jsonString.LastIndexOf('}');
			if (endIndex > 0 && endIndex < jsonString.Length - 1)
			{
				jsonString = jsonString[..(endIndex + 1)];
			}

			try
			{
				return JsonSerializer.Deserialize<AiQueryResult>(jsonString, new JsonSerializerOptions
				{
					PropertyNameCaseInsensitive = true
				}) ?? throw new JsonException("Deserialization returned null.");
			}
			catch (JsonException ex)
			{
				_logger.LogError(ex, "Failed to parse AI response. Raw content:\n{Raw}", jsonString[..Math.Min(jsonString.Length, 400)]);
				throw;
			}
		}
	}
}
