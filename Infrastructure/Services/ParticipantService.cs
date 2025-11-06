using AutoMapper;
using Core.Domain;
using Core.Interfaces;
using Infrastructure.Interfaces;
using Infrastructure.Persistence.Entity;

namespace Infrastructure.Services
{
	public class ParticipantService : IParticipantService
	{
		private readonly IParticipantRepository _participantRepository;
		private readonly IMapper _mapper;

		public ParticipantService(IParticipantRepository participantRepository, IMapper mapper)
		{
			_participantRepository = participantRepository;
			_mapper = mapper;
		}

		public async Task<Participant> SubscribeAsync(string userId, string eventId)
		{
			if (!Guid.TryParse(userId, out var userGuid))
				throw new ArgumentException("Invalid user ID format");

			if (!Guid.TryParse(eventId, out var eventGuid))
				throw new ArgumentException("Invalid event ID format");

			var existingEntity = await _participantRepository.GetSubscription(userGuid, eventGuid);
			if (existingEntity is not null)
			{
				return _mapper.Map<Participant>(existingEntity);
			}

			var newEntity = new ParticipantEntity
			{
				UserId = userGuid,
				EventId = eventGuid,
				CreatedAt = DateTimeOffset.UtcNow
			};

			var createdEntity = await _participantRepository.CreateSubscription(newEntity);
			return _mapper.Map<Participant>(createdEntity);
		}

		public async Task<bool> UnsubscribeAsync(string userId, string eventId)
		{
			if (!Guid.TryParse(userId, out var userGuid))
				throw new ArgumentException("Invalid user ID format");

			if (!Guid.TryParse(eventId, out var eventGuid))
				throw new ArgumentException("Invalid event ID format");

			var entity = await _participantRepository.GetSubscription(userGuid, eventGuid);
			if (entity is null)
				return false;

			await _participantRepository.DeleteSubscription(entity.Id);
			return true;
		}

		public async Task<IEnumerable<Participant>> GetFollowingAsync(string userId)
		{
			if (!Guid.TryParse(userId, out var userGuid))
				throw new ArgumentException("Invalid user ID format");

			var entities = await _participantRepository.GetByFollower(userGuid);
			return _mapper.Map<IEnumerable<Participant>>(entities);
		}

		public async Task<IEnumerable<Participant>> GetFollowersAsync(string eventId)
		{
			if (!Guid.TryParse(eventId, out var eventGuid))
				throw new ArgumentException("Invalid event ID format");

			var entities = await _participantRepository.GetByTarget(eventGuid);
			return _mapper.Map<IEnumerable<Participant>>(entities);
		}

		public async Task<bool> IsFollowingAsync(string userId, string eventId)
		{
			if (!Guid.TryParse(userId, out var userGuid))
				throw new ArgumentException("Invalid user ID format");

			if (!Guid.TryParse(eventId, out var eventGuid))
				throw new ArgumentException("Invalid event ID format");

			var entity = await _participantRepository.GetSubscription(userGuid, eventGuid);
			return entity is not null;
		}
	}
}