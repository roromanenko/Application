using Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
	public interface IParticipantService
	{
		Task<Participant> SubscribeAsync(string userId, string eventId);
		Task<bool> UnsubscribeAsync(string userId, string eventId);

		Task<IEnumerable<Participant>> GetFollowingAsync(string userId);
		Task<IEnumerable<Participant>> GetFollowersAsync(string eventId);

		Task<bool> IsFollowingAsync(string userId, string eventId);
	}
}
