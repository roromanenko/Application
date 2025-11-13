using Core.Domain;

namespace Core.Interfaces
{
	public interface IAiAssistantService
	{
		Task<AiQueryResult> ProcessUserQueryAsync(string userQuery, Guid userId, CancellationToken cancellationToken = default);

		Task<string> GenerateHumanResponseAsync(
			string userQuery,
			string sqlQuery,
			List<Dictionary<string, object>> data,
			CancellationToken cancellationToken = default);
	}
}
