namespace Api.DTO
{
	public record SubscribeRequest(
		string TargetId,
		string TargetType // "User", "Event", "Organization"
	);

	public record UnsubscribeRequest(
		string TargetId,
		string TargetType
	);
}