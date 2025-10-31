using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain
{
	public sealed class Subscription
	{
		public required string Id { get; init; }

		public required string FollowerId { get; init; }
		public required string TargetId { get; init; }

		public required SubscriptionTargetType TargetType { get; init; }

		public bool IsBlocked { get; set; } = false;

		public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
	}

	public enum SubscriptionTargetType
	{
		User,
		Event,
		Organization
	}
}
