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
	[Table("event")]
	public class EventEntity
	{
		[BsonId]
		public ObjectId Id { get; set; }

		[BsonRequired]
		public string Title { get; set; }
		public string Description { get; set; }

		[BsonRequired]
		public string OrganizerId { get; set; } = string.Empty;
		public DateTimeOffset StartDate { get; set; }
		public DateTimeOffset EndDate { get; set; }
		public string? Location { get; set; }

		[BsonDefaultValue(true)]
		public bool IsPublic { get; set; }

		public List<string> ParticipantIds { get; set; } = [];
		
		[BsonRequired] 
		public DateTimeOffset CreatedAt { get; set; } = DateTime.UtcNow;

		[BsonRequired] 
		public DateTimeOffset UpdatedAt { get; set; } = DateTime.UtcNow;
	}
}
