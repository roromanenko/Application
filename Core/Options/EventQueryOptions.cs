using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Options
{
	public class EventQueryOptions
	{
		public int Page { get; set; } = 1;
		public int PageSize { get; set; } = 20;
		public string? OrganizerId { get; set; }
		public string? Title { get; set; }
		public string? SortBy { get; set; } = "createdAt"; // createdAt, startDate, participants
		public bool SortDescending { get; set; } = true;
	}
}
