using Api.DTO;
using FluentValidation;

namespace Api.Validators
{
	public class AiQueryRequestValidator : AbstractValidator<AiQueryRequest>
	{
		public AiQueryRequestValidator()
		{
			RuleFor(x => x.Query)
				.NotEmpty()
				.WithMessage("Query cannot be empty")
				.MinimumLength(3)
				.WithMessage("Query must be at least 3 characters long")
				.MaximumLength(500)
				.WithMessage("Query cannot exceed 500 characters")
				.Must(BeValidQuery)
				.WithMessage("Query contains invalid characters");
		}

		private bool BeValidQuery(string query)
		{
			// Проверка на потенциально опасные символы
			var dangerousPatterns = new[] { ";--", "/*", "*/", "xp_", "sp_" };
			return !dangerousPatterns.Any(pattern =>
				query.Contains(pattern, StringComparison.OrdinalIgnoreCase));
		}
	}
}
