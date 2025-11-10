using Core.Options;
using Infrastructure.Persistence.Entity;
using System.Threading.Tasks;

namespace Infrastructure.Interfaces
{
	public interface IEventRepository
	{
		Task<EventEntity> CreateEvent(EventEntity newEvent);
		Task<bool> AddTagsToEvent(Guid eventId, IEnumerable<Guid> tagIds);
		Task<bool> RemoveTagFromEvent(Guid eventId, Guid tagId);
		Task<EventEntity?> GetEventById(Guid eventId);
		Task<IEnumerable<EventEntity>> GetEventsByTitle(string title);
		Task<IEnumerable<EventEntity>> GetEventsByOrganizer(Guid organizerId);
		Task<IEnumerable<EventEntity>> GetEvents(EventQueryOptions options);
		Task<bool> UpdateEvent(EventEntity updatedEvent);
		Task<bool> DeleteEvent(Guid eventId);
	}
}
