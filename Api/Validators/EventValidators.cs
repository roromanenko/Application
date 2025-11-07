using Api.DTO;
using FluentValidation;

namespace Api.Validators
{
	public class CreateEventRequestValidator : AbstractValidator<CreateEventRequest>
	{
		public CreateEventRequestValidator()
		{
			RuleFor(x => x.Title)
				.NotEmpty().WithMessage("Title is required")
				.MaximumLength(100).WithMessage("Title cannot exceed 100 characters");

			RuleFor(x => x.Description)
				.MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters")
				.When(x => !string.IsNullOrEmpty(x.Description));

			RuleFor(x => x.StartDate)
				.NotEmpty().WithMessage("Start date is required")
				.Must(d => d > DateTime.UtcNow.AddMinutes(-5))
				.WithMessage("Start date must be in the future");

			RuleFor(x => x.EndDate)
				.NotEmpty().WithMessage("End date is required")
				.GreaterThan(x => x.StartDate)
				.WithMessage("End date must be after start date");

			RuleFor(x => x.Location)
				.MaximumLength(200).WithMessage("Location cannot exceed 200 characters")
				.When(x => !string.IsNullOrEmpty(x.Location));
		}
	}

	public class UpdateEventRequestValidator : AbstractValidator<UpdateEventRequest>
	{
		public UpdateEventRequestValidator()
		{
			RuleFor(x => x.Title)
				.NotEmpty().WithMessage("Title is required")
				.MaximumLength(100);

			RuleFor(x => x.Description)
				.MaximumLength(1000)
				.When(x => !string.IsNullOrEmpty(x.Description));

			RuleFor(x => x.StartDate)
				.Must(d => d > DateTime.UtcNow.AddMinutes(-5))
				.WithMessage("Start date must be in the future");

			RuleFor(x => x.EndDate)
				.GreaterThan(x => x.StartDate)
				.WithMessage("End date must be after start date");
		}
	}
}
