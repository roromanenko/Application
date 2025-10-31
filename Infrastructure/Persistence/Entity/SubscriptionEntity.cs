using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Entity
{
	[Table("subscription")]
	public class SubscriptionEntity
	{
		[BsonId]
		public ObjectId Id { get; set; }

		[BsonRequired]
		public string FollowerId { get; set; } = string.Empty;

		[BsonRequired]
		public string TargetId { get; set; } = string.Empty;

		[BsonRequired]
		public string TargetType { get; set; } = string.Empty;	//(User, Event, Organization)

		[BsonDefaultValue(false)]
		public bool IsBlocked { get; set; } = false;

		[BsonRequired]
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
	}
}
