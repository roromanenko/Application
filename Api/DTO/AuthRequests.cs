namespace Api.DTO
{
	public record LoginRequest(string UsernameOrEmail, string Password);
	public record RegisterRequest(string Username, string Email, string FirstName, string LastName, string Password, string ConfirmPassword);
	public record ChangePasswordRequest(string CurrentPassword, string NewPassword, string ConfirmPassword);
	public record NewPasswordRequest(string NewPassword, string ConfirmPassword);

	public class UpdateUserRequest
	{
		public string? FirstName { get; set; }
		public string? LastName { get; set; }
		public string? Username { get; set; }
		public string? Email { get; set; }
	}
}
