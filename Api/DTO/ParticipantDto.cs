namespace Api.DTO
{
	public record ParticipantDto
	(
		string Id,
		string UserId,
		string EventId,
		DateTime CreatedAt
	);
}
