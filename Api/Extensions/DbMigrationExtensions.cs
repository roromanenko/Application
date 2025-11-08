using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Api.Extensions
{
	public static class DbMigrationExtensions
	{
		public static async Task<IApplicationBuilder> UseDatabaseMigration(this IApplicationBuilder app)
		{
			using var scope = app.ApplicationServices.CreateScope();
			var services = scope.ServiceProvider;
			var logger = services.GetRequiredService<ILogger<ApplicationDbContext>>();

			try
			{
				var context = services.GetRequiredService<ApplicationDbContext>();

				logger.LogInformation("Starting database migration...");

				var pendingMigrations = await context.Database.GetPendingMigrationsAsync();

				if (pendingMigrations.Any())
				{
					logger.LogInformation($"Found {pendingMigrations.Count()} pending migrations");

					await context.Database.MigrateAsync();

					logger.LogInformation("Database migration completed successfully");
				}
				else
				{
					logger.LogInformation("Database is up to date, no migrations needed");
				}

				var canConnect = await context.Database.CanConnectAsync();
				if (canConnect)
				{
					logger.LogInformation("Database connection successful");
				}
				else
				{
					logger.LogError("Cannot connect to database");
				}
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "An error occurred while migrating the database");
				throw;
			}

			return app;
		}
	}
}