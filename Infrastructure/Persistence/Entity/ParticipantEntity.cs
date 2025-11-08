using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Persistence.Entity
{
	[Table("participants")]
	public class ParticipantEntity
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public Guid Id { get; set; }

		[Required]
		public Guid UserId { get; set; }

		[Required]
		public Guid EventId { get; set; }

		[Required]
		public DateTimeOffset CreatedAt { get; set; }

		// Navigation properties
		[ForeignKey(nameof(UserId))]
		public UserEntity User { get; set; } = null!;

		[ForeignKey(nameof(EventId))]
		public EventEntity Event { get; set; } = null!;
	}
}