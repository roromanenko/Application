namespace Api.DTO
{
	public record CreateEventRequest(
		string Title,
		string? Description,
		DateTime StartDate,
		DateTime EndDate,
		string? Location,
		int? Capacity,
		bool IsPublic,
		IEnumerable<string>? Tags
	);

	public record UpdateEventRequest(
		string Title,
		string? Description,
		DateTime StartDate,
		DateTime EndDate,
		string? Location,
		int? Capacity,
		bool IsPublic,
		IEnumerable<string>? Tags
	);
}
