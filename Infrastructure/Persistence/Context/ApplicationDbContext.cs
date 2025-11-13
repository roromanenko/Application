using Infrastructure.Persistence.Entity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence
{
	public class ApplicationDbContext : DbContext
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
			: base(options)
		{
		}

		public DbSet<UserEntity> Users { get; set; }
		public DbSet<EventEntity> Events { get; set; }
		public DbSet<ParticipantEntity> Participants { get; set; }
		public DbSet<TagEntity> Tags { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);
			modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
		}

		public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
		{
			ConvertDateTimeOffsetsToUtc();
			return await base.SaveChangesAsync(cancellationToken);
		}

		public override int SaveChanges()
		{
			ConvertDateTimeOffsetsToUtc();
			return base.SaveChanges();
		}

		private void ConvertDateTimeOffsetsToUtc()
		{
			var entries = ChangeTracker.Entries()
				.Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

			foreach (var entry in entries)
			{
				foreach (var property in entry.Properties)
				{
					if (property.Metadata.ClrType == typeof(DateTimeOffset))
					{
						var value = (DateTimeOffset?)property.CurrentValue;
						if (value.HasValue && value.Value.Offset != TimeSpan.Zero)
						{
							property.CurrentValue = value.Value.ToUniversalTime();
						}
					}
				}
			}
		}
	}
}