namespace Api.DTO
{
	public record SubscriptionDto
	(
		string Id,
		string FollowerId,
		string TargetId,
		string TargetType,
		bool IsBlocked,
		DateTime CreatedAt
	);
}
