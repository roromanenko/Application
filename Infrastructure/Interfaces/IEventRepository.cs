using Core.Options;
using Infrastructure.Persistence.Entity;

namespace Infrastructure.Interfaces
{
	public interface IEventRepository
	{
		Task<EventEntity> CreateEvent(EventEntity newEvent);
		Task<EventEntity?> GetEventById(Guid eventId);
		Task<IEnumerable<EventEntity>> GetEventsByTitle(string title);
		Task<IEnumerable<EventEntity>> GetEventsByOrganizer(Guid organizerId);
		Task<IEnumerable<EventEntity>> GetEvents(EventQueryOptions options);
		Task<bool> UpdateEvent(EventEntity updatedEvent);
		Task<bool> DeleteEvent(Guid eventId);
	}
}
