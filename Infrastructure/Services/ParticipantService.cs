using AutoMapper;
using Core.Domain;
using Core.Interfaces;
using Infrastructure.Interfaces;
using Infrastructure.Persistence.Entity;
using MongoDB.Bson;

namespace Infrastructure.Services
{
	public class ParticipantService : IParticipantService
	{
		private readonly IParticipantRepository _participantRepository;
		private readonly IMapper _mapper;

		public ParticipantService(IParticipantRepository subscriptionRepository, IMapper mapper)
		{
			_participantRepository = subscriptionRepository;
			_mapper = mapper;
		}

		public async Task<Participant> SubscribeAsync(string followerId, string targetId)
		{
			var existingEntity = await _participantRepository.GetSubscription(followerId, targetId);
			if (existingEntity is not null)
			{
				return _mapper.Map<Participant>(existingEntity);
			}

			var newEntity = new ParticipantEntity
			{
				FollowerId = followerId,
				TargetId = targetId,
				CreatedAt = DateTime.UtcNow
			};

			var createdEntity = await _participantRepository.CreateSubscription(newEntity);
			return _mapper.Map<Participant>(createdEntity);
		}

		public async Task<bool> UnsubscribeAsync(string followerId, string targetId)
		{
			var entity = await _participantRepository.GetSubscription(followerId, targetId);
			if (entity is null)
				return false;

			await _participantRepository.DeleteSubscription(entity.Id);
			return true;
		}

		public async Task<IEnumerable<Participant>> GetFollowingAsync(string followerId)
		{
			var entities = await _participantRepository.GetByFollower(followerId);

			return _mapper.Map<IEnumerable<Participant>>(entities);
		}

		public async Task<IEnumerable<Participant>> GetFollowersAsync(string targetId)
		{
			var entities = await _participantRepository.GetByTarget(targetId);

			return _mapper.Map<IEnumerable<Participant>>(entities);
		}

		public async Task<bool> IsFollowingAsync(string followerId, string targetId)
		{
			var entity = await _participantRepository.GetSubscription(followerId, targetId);
			return entity is not null;
		}
	}
}
