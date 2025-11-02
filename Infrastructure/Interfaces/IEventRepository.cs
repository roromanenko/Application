using Core.Options;
using Infrastructure.Persistence.Entity;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Interfaces
{
	public interface IEventRepository
	{
		Task<EventEntity> CreateEvent(EventEntity newEvent);
		Task<EventEntity?> GetEventById(ObjectId eventId);
		Task<IEnumerable<EventEntity>> GetEventsByTitle(string title);
		Task<IEnumerable<EventEntity>> GetEventsByOrganizer(string organizerId);
		Task<IEnumerable<EventEntity>> GetEvents(EventQueryOptions options);
		Task<bool> UpdateEvent(EventEntity updatedEvent);
		Task<bool> DeleteEvent(ObjectId eventId);
	}
}
