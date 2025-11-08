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
		public required string UserId { get; init; }
		public required string EventId { get; init; }
		public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;

		public User? User { get; set; }
	}
}
