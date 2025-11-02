namespace Api.DTO
{
	public record EventDto
	(
		string Id,
		string Title,
		string? Description,
		string OrganizerId,
		DateTime StartDate,
		DateTime EndDate,
		string? Location,
		bool IsPublic,
		IEnumerable<string> ParticipantIds,
		DateTime CreatedAt,
		DateTime UpdatedAt
	);
}
