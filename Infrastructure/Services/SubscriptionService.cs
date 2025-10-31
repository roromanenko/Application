using AutoMapper;
using Core.Domain;
using Core.Interfaces;
using Infrastructure.Interfaces;
using Infrastructure.Persistence.Entity;
using MongoDB.Bson;

namespace Infrastructure.Services
{
	public class SubscriptionService : ISubscriptionService
	{
		private readonly ISubscriptionRepository _subscriptionRepository;
		private readonly IMapper _mapper;

		public SubscriptionService(ISubscriptionRepository subscriptionRepository, IMapper mapper)
		{
			_subscriptionRepository = subscriptionRepository;
			_mapper = mapper;
		}

		public async Task<Subscription> SubscribeAsync(string followerId, string targetId, SubscriptionTargetType targetType)
		{
			var existingEntity = await _subscriptionRepository.GetSubscription(followerId, targetId, targetType.ToString());
			if (existingEntity is not null)
			{
				return _mapper.Map<Subscription>(existingEntity);
			}

			var newEntity = new SubscriptionEntity
			{
				FollowerId = followerId,
				TargetId = targetId,
				TargetType = targetType.ToString(),
				IsBlocked = false,
				CreatedAt = DateTime.UtcNow
			};

			var createdEntity = await _subscriptionRepository.CreateSubscription(newEntity);
			return _mapper.Map<Subscription>(createdEntity);
		}

		public async Task<bool> UnsubscribeAsync(string followerId, string targetId, SubscriptionTargetType targetType)
		{
			var entity = await _subscriptionRepository.GetSubscription(followerId, targetId, targetType.ToString());
			if (entity is null)
				return false;

			await _subscriptionRepository.DeleteSubscription(entity.Id);
			return true;
		}

		public async Task<IEnumerable<Subscription>> GetFollowingAsync(string followerId, SubscriptionTargetType? filterType = null)
		{
			var entities = await _subscriptionRepository.GetByFollower(followerId);

			if (filterType.HasValue)
				entities = entities.Where(e => e.TargetType == filterType.Value.ToString());

			return _mapper.Map<IEnumerable<Subscription>>(entities);
		}

		public async Task<IEnumerable<Subscription>> GetFollowersAsync(string targetId, SubscriptionTargetType? filterType = null)
		{
			var entities = await _subscriptionRepository.GetByTarget(targetId);

			if (filterType.HasValue)
				entities = entities.Where(e => e.TargetType == filterType.Value.ToString());

			return _mapper.Map<IEnumerable<Subscription>>(entities);
		}

		public async Task<bool> IsFollowingAsync(string followerId, string targetId, SubscriptionTargetType targetType)
		{
			var entity = await _subscriptionRepository.GetSubscription(followerId, targetId, targetType.ToString());
			return entity is not null && !entity.IsBlocked;
		}
	}
}
