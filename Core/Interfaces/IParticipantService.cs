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
		Task<Participant> SubscribeAsync(string followerId, string targetId);
		Task<bool> UnsubscribeAsync(string followerId, string targetId);

		Task<IEnumerable<Participant>> GetFollowingAsync(string followerId);
		Task<IEnumerable<Participant>> GetFollowersAsync(string targetId);

		Task<bool> IsFollowingAsync(string followerId, string targetId);
	}
}
