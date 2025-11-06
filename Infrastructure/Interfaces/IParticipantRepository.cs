using Infrastructure.Persistence.Entity;

namespace Infrastructure.Interfaces
{
	public interface IParticipantRepository
	{
		Task<ParticipantEntity> CreateSubscription(ParticipantEntity entity);
		Task<ParticipantEntity?> GetSubscription(Guid userId, Guid eventId);
		Task<IEnumerable<ParticipantEntity>> GetByFollower(Guid userId);
		Task<IEnumerable<ParticipantEntity>> GetByTarget(Guid eventId);
		Task DeleteSubscription(Guid id);
	}
}
