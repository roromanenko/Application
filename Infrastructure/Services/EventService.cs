using AutoMapper;
using Core.Domain;
using Core.Interfaces;
using Core.Options;
using Infrastructure.Interfaces;
using Infrastructure.Persistence.Entity;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
	public class EventService : IEventService
	{
		private readonly IEventRepository _eventRepository;
		private readonly IMapper _mapper;

		public EventService(IEventRepository eventRepository, IMapper mapper)
		{
			_eventRepository = eventRepository;
			_mapper = mapper;
		}

		public async Task<Event> CreateEventAsync(Event newEvent)
		{
			var entity = _mapper.Map<EventEntity>(newEvent);
			var created = await _eventRepository.CreateEvent(entity);
			return _mapper.Map<Event>(created);
		}

		public async Task<Event?> GetEventByIdAsync(string eventId)
		{
			if (!ObjectId.TryParse(eventId, out var objectId))
				throw new ArgumentException("Invalid event ID format");

			var entity = await _eventRepository.GetEventById(objectId);
			return entity is null ? null : _mapper.Map<Event>(entity);
		}

		public async Task<IEnumerable<Event>> GetEventsByTitleAsync(string title)
		{
			var entities = await _eventRepository.GetEventsByTitle(title);
			return _mapper.Map<IEnumerable<Event>>(entities);
		}

		public async Task<IEnumerable<Event>> GetEventsByOrganizerAsync(string organizerId)
		{
			var entities = await _eventRepository.GetEventsByOrganizer(organizerId);
			return _mapper.Map<IEnumerable<Event>>(entities);
		}

		public async Task<IEnumerable<Event>> GetEventsAsync(EventQueryOptions options, bool includePrivate = false)
		{
			var entities = await _eventRepository.GetEvents(options);

			if (!includePrivate)
				entities = entities.Where(e => e.IsPublic);

			return _mapper.Map<IEnumerable<Event>>(entities);
		}

		public async Task<bool> UpdateEventAsync(Event updatedEvent)
		{
			var entity = _mapper.Map<EventEntity>(updatedEvent);
			return await _eventRepository.UpdateEvent(entity);
		}

		public async Task<bool> DeleteEventAsync(string eventId)
		{
			if (!ObjectId.TryParse(eventId, out var objectId))
				throw new ArgumentException("Invalid event ID format");

			return await _eventRepository.DeleteEvent(objectId);
		}
	}
}
