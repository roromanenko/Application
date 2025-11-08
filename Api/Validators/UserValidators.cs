using Api.DTO;
using FluentValidation;

namespace Api.Validators
{
	public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
	{
		public RegisterRequestValidator()
		{
			RuleFor(x => x.Username)
				.NotEmpty().WithMessage("Username is required")
				.MinimumLength(3).WithMessage("Username must be at least 3 characters");

			RuleFor(x => x.Email)
				.NotEmpty().WithMessage("Email is required")
				.EmailAddress().WithMessage("Invalid email format");

			RuleFor(x => x.Password)
				.NotEmpty().WithMessage("Password is required")
				.MinimumLength(6).WithMessage("Password must be at least 6 characters long");

			RuleFor(x => x.ConfirmPassword)
				.Equal(x => x.Password).WithMessage("Passwords do not match");

			RuleFor(x => x.FirstName)
				.NotEmpty().WithMessage("First name is required");

			RuleFor(x => x.LastName)
				.NotEmpty().WithMessage("Last name is required");
		}
	}

	public class LoginRequestValidator : AbstractValidator<LoginRequest>
	{
		public LoginRequestValidator()
		{
			RuleFor(x => x.UsernameOrEmail)
				.NotEmpty().WithMessage("Username or email is required");

			RuleFor(x => x.Password)
				.NotEmpty().WithMessage("Password is required");
		}
	}

	public class ChangePasswordRequestValidator : AbstractValidator<ChangePasswordRequest>
	{
		public ChangePasswordRequestValidator()
		{
			RuleFor(x => x.CurrentPassword).NotEmpty();
			RuleFor(x => x.NewPassword)
				.NotEmpty().MinimumLength(6);
			RuleFor(x => x.ConfirmPassword)
				.Equal(x => x.NewPassword).WithMessage("Passwords do not match");
		}
	}

	public class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
	{
		public UpdateUserRequestValidator()
		{
			RuleFor(x => x.Email)
				.EmailAddress()
				.When(x => !string.IsNullOrWhiteSpace(x.Email))
				.WithMessage("Invalid email format");
		}
	}
}
