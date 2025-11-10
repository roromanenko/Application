namespace Api.DTO
{
	public record EventDto
	(
		string Id,
		string Title,
		string? Description,
		string OrganizerId,
		DateTimeOffset StartDate,
		DateTimeOffset EndDate,
		string? Location,
		bool IsPublic,
		int Capacity,
		List<TagDto> Tags,
		DateTimeOffset CreatedAt,
		DateTimeOffset UpdatedAt
	);
}
