using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain
{
	public class Event
	{
		public required string Id { get; set; }
		public required string Title { get; set; }
		public string? Description { get; set; }

		public required string OrganizerId { get; set; }
		public DateTimeOffset StartDate { get; set; }
		public DateTimeOffset EndDate { get; set; }
		public string? Location { get; set; }
		public bool IsPublic { get; set; } = true;
		public int Capacity { get; set; }
		public List<Tag> Tags { get; set; } = new();

		public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.Now;
		public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.Now;
	}
}
