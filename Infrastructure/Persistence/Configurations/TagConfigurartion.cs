using Infrastructure.Persistence.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Configurations
{
	public class TagConfigurartion : IEntityTypeConfiguration<TagEntity>
	{
		public void Configure(EntityTypeBuilder<TagEntity> builder)
		{
			builder.ToTable("tags");

			builder.HasKey(t => t.Id);

			builder.Property(t => t.Name)
				.IsRequired()
				.HasMaxLength(50);

			builder.HasIndex(t => t.Name).IsUnique();

			builder
				.HasMany(t => t.Events)
				.WithMany(e => e.Tags)
				.UsingEntity<Dictionary<string, object>>(
					"event_tags",
					j => j
						.HasOne<EventEntity>()
						.WithMany()
						.HasForeignKey("EventId")
						.OnDelete(DeleteBehavior.Cascade),
					j => j
						.HasOne<TagEntity>()
						.WithMany()
						.HasForeignKey("TagId")
						.OnDelete(DeleteBehavior.Cascade),
					j =>
					{
						j.HasKey("EventId", "TagId");
						j.ToTable("event_tags");
					});
		}
	}
}
