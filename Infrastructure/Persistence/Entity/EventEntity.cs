using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Persistence.Entity
{
	[Table("events")]
	public class EventEntity
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public Guid Id { get; set; }

		[Required]
		[MaxLength(200)]
		public string Title { get; set; } = string.Empty;

		[MaxLength(2000)]
		public string? Description { get; set; }

		[Required]
		public Guid OrganizerId { get; set; }

		[Required]
		public DateTimeOffset StartDate { get; set; }

		[Required]
		public DateTimeOffset EndDate { get; set; }

		[MaxLength(500)]
		public string? Location { get; set; }

		public bool IsPublic { get; set; } = true;

		public int Capacity { get; set; }

		[Required]
		public DateTimeOffset CreatedAt { get; set; }

		[Required]
		public DateTimeOffset UpdatedAt { get; set; }

		// Navigation properties
		[ForeignKey(nameof(OrganizerId))]
		public UserEntity Organizer { get; set; } = null!;

		public ICollection<ParticipantEntity> Participants { get; set; } = new List<ParticipantEntity>();
		public ICollection<TagEntity> Tags { get; set; } = new List<TagEntity>();
	}
}