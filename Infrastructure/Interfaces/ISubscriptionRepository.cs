using Core.Domain;
using Infrastructure.Persistence.Entity;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Interfaces
{
	public interface ISubscriptionRepository
	{
		Task<SubscriptionEntity> CreateSubscription(SubscriptionEntity entity);
		Task<SubscriptionEntity?> GetSubscription(string followerId, string targetId, string targetType);
		Task<IEnumerable<SubscriptionEntity>> GetByFollower(string followerId);
		Task<IEnumerable<SubscriptionEntity>> GetByTarget(string targetId);
		Task DeleteSubscription(ObjectId id);
	}
}
