using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Api.Extensions
{
	public static class HealthCheckExtensions
	{
		public static IServiceCollection AddOrganizaHealthChecks(
			this IServiceCollection services,
			IConfiguration configuration)
		{
			var connectionString = configuration.GetConnectionString("PostgreSql");

			services.AddHealthChecks()
				.AddNpgSql(
					connectionString!,
					name: "postgres",
					failureStatus: HealthStatus.Unhealthy,
					tags: new[] { "db", "postgresql", "ready" },
					timeout: TimeSpan.FromSeconds(5))
				.AddCheck(
					"self",
					() => HealthCheckResult.Healthy("API is running"),
					tags: new[] { "api" });

			return services;
		}

		public static IEndpointRouteBuilder MapOrganizaHealthChecks(
			this IEndpointRouteBuilder endpoints)
		{
			endpoints.MapHealthChecks("/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
			{
				ResponseWriter = async (context, report) =>
				{
					context.Response.ContentType = "application/json";
					var result = System.Text.Json.JsonSerializer.Serialize(new
					{
						status = report.Status.ToString(),
						checks = report.Entries.Select(e => new
						{
							name = e.Key,
							status = e.Value.Status.ToString(),
							description = e.Value.Description,
							duration = e.Value.Duration.TotalMilliseconds
						}),
						totalDuration = report.TotalDuration.TotalMilliseconds
					});
					await context.Response.WriteAsync(result);
				}
			});

			endpoints.MapHealthChecks("/health/ready", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
			{
				Predicate = check => check.Tags.Contains("ready")
			});

			endpoints.MapHealthChecks("/health/live", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
			{
				Predicate = check => check.Tags.Contains("api")
			});

			return endpoints;
		}
	}
}