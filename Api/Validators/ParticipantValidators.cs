using FluentValidation;

namespace Api.Validators
{
	public class EventIdValidator : AbstractValidator<string>
	{
		public EventIdValidator()
		{
			RuleFor(x => x)
				.NotEmpty().WithMessage("Event ID is required")
				.Matches("^[a-zA-Z0-9-]+$").WithMessage("Event ID must be alphanumeric");
		}
	}
}
