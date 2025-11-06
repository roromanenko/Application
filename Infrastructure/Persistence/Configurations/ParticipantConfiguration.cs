using Infrastructure.Persistence.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
	public class ParticipantConfiguration : IEntityTypeConfiguration<ParticipantEntity>
	{
		public void Configure(EntityTypeBuilder<ParticipantEntity> builder)
		{
			builder.ToTable("participants");

			builder.HasKey(p => p.Id);

			builder.HasIndex(p => new { p.UserId, p.EventId }).IsUnique();
			builder.HasIndex(p => p.UserId);
			builder.HasIndex(p => p.EventId);

			builder.HasOne(p => p.User)
				.WithMany(u => u.Participations)
				.HasForeignKey(p => p.UserId)
				.OnDelete(DeleteBehavior.Cascade);

			builder.HasOne(p => p.Event)
				.WithMany(e => e.Participants)
				.HasForeignKey(p => p.EventId)
				.OnDelete(DeleteBehavior.Cascade);
		}
	}
}