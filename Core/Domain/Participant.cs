using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain
{
	public sealed class Participant
	{
		public required string Id { get; init; }
		public required string FollowerId { get; init; }
		public required string TargetId { get; init; }
		public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
	}
}
