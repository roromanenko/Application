namespace Api.DTO
{
	public record UserDto
	(
		string Id,
		string Username,
		string FirstName,
		string LastName,
		IEnumerable<string> Roles
	);
}
