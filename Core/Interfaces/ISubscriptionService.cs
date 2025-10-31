using Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
	public interface ISubscriptionService
	{
		Task<Subscription> SubscribeAsync(string followerId, string targetId, SubscriptionTargetType targetType);
		Task<bool> UnsubscribeAsync(string followerId, string targetId, SubscriptionTargetType targetType);

		Task<IEnumerable<Subscription>> GetFollowingAsync(string followerId, SubscriptionTargetType? filterType = null);
		Task<IEnumerable<Subscription>> GetFollowersAsync(string targetId, SubscriptionTargetType? filterType = null);

		Task<bool> IsFollowingAsync(string followerId, string targetId, SubscriptionTargetType targetType);
	}
}
