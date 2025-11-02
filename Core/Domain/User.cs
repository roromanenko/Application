using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain
{
	public sealed class User
	{
		public required string Id { get; init; }
		public required string Username { get; set; }
		public required string Email { get; set; }
		public required string FirstName { get; set; }
		public required string LastName { get; set; }
		public required string PasswordHash { get; set; }
		public List<string> Roles { get; set; } = [];
	}
}
