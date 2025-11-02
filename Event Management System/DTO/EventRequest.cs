namespace Api.DTO
{
	public record CreateEventRequest(
		string Title,
		string? Description,
		DateTime StartDate,
		DateTime EndDate,
		string? Location,
		bool IsPublic
	);

	public record UpdateEventRequest(
		string Title,
		string? Description,
		DateTime StartDate,
		DateTime EndDate,
		string? Location,
		bool IsPublic
	);
}
