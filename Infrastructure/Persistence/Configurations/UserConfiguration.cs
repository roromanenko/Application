using Infrastructure.Persistence.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
	public class UserConfiguration : IEntityTypeConfiguration<UserEntity>
	{
		public void Configure(EntityTypeBuilder<UserEntity> builder)
		{
			builder.ToTable("users");

			builder.HasKey(u => u.Id);

			builder.Property(u => u.Roles)
				.HasConversion(
					v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
					v => System.Text.Json.JsonSerializer.Deserialize<List<string>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new List<string>()
				)
				.HasColumnType("jsonb");

			builder.HasIndex(u => u.Email).IsUnique();
			builder.HasIndex(u => u.Username).IsUnique();

			builder.HasMany(u => u.OrganizedEvents)
				.WithOne(e => e.Organizer)
				.HasForeignKey(e => e.OrganizerId)
				.OnDelete(DeleteBehavior.Cascade);

			builder.HasMany(u => u.Participations)
				.WithOne(p => p.User)
				.HasForeignKey(p => p.UserId)
				.OnDelete(DeleteBehavior.Cascade);
		}
	}
}