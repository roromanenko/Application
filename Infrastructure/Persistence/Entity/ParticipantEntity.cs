using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Persistence.Entity
{
	[Table("participant")]
	public class ParticipantEntity
	{
		[BsonId]
		public ObjectId Id { get; set; }

		[BsonRequired]
		public string FollowerId { get; set; } = string.Empty;

		[BsonRequired]
		public string TargetId { get; set; } = string.Empty;

		[BsonRequired]
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
	}
}
