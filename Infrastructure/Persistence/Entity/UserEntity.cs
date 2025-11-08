using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Persistence.Entity
{
	[Table("users")]
	public class UserEntity
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public Guid Id { get; set; }

		[Required]
		[MaxLength(100)]
		public string Username { get; set; } = string.Empty;

		[Required]
		[MaxLength(256)]
		public string Email { get; set; } = string.Empty;

		[Required]
		[MaxLength(100)]
		public string FirstName { get; set; } = string.Empty;

		[Required]
		[MaxLength(100)]
		public string LastName { get; set; } = string.Empty;

		[Required]
		public string PasswordHash { get; set; } = string.Empty;

		public List<string> Roles { get; set; } = new();

		// Navigation properties
		public ICollection<EventEntity> OrganizedEvents { get; set; } = new List<EventEntity>();
		public ICollection<ParticipantEntity> Participations { get; set; } = new List<ParticipantEntity>();
	}
}