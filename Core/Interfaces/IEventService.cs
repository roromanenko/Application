using Core.Domain;
using Core.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
	public interface IEventService
	{
		Task<Event> CreateEventAsync(Event newEvent);
		Task<Event?> GetEventByIdAsync(string eventId);
		Task<IEnumerable<Event>> GetEventsByTitleAsync(string title);
		Task<IEnumerable<Event>> GetEventsByOrganizerAsync(string organizerId);
		Task<IEnumerable<Event>> GetEventsAsync(EventQueryOptions options, bool includePrivate);
		Task<bool> UpdateEventAsync(Event updatedEvent);
		Task<bool> DeleteEventAsync(string eventId);
	}
}
