namespace Api.DTO
{
	public record ParticipantDto
	(
		string Id,
		string FollowerId,
		string TargetId,
		DateTime CreatedAt
	);
}
