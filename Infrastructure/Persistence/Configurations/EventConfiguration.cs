using Infrastructure.Persistence.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
	public class EventConfiguration : IEntityTypeConfiguration<EventEntity>
	{
		public void Configure(EntityTypeBuilder<EventEntity> builder)
		{
			builder.ToTable("events");

			builder.HasKey(e => e.Id);

			builder.HasIndex(e => e.StartDate);
			builder.HasIndex(e => e.IsPublic);
			builder.HasIndex(e => e.OrganizerId);

			builder.HasOne(e => e.Organizer)
				.WithMany(u => u.OrganizedEvents)
				.HasForeignKey(e => e.OrganizerId)
				.OnDelete(DeleteBehavior.Cascade);

			builder.HasMany(e => e.Participants)
				.WithOne(p => p.Event)
				.HasForeignKey(p => p.EventId)
				.OnDelete(DeleteBehavior.Cascade);
		}
	}
}