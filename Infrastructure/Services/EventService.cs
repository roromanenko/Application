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
		private readonly IParticipantService _participantService;
		private readonly IMapper _mapper;

		public EventService(IEventRepository eventRepository, IParticipantService participantService, IMapper mapper)
		{
			_eventRepository = eventRepository;
			_participantService = participantService;
			_mapper = mapper;
		}

		public async Task<Event> CreateEventAsync(Event newEvent)
		{
			var entity = _mapper.Map<EventEntity>(newEvent);
			var created = await _eventRepository.CreateEvent(entity);
			return _mapper.Map<Event>(created);
		}

		public async Task<bool> JoinEventAsync(string eventId, string userId)
		{
			var ev = await _eventRepository.GetEventById(new ObjectId(eventId));
			if (ev == null)
				throw new ArgumentException("Event not found");

			var participants = await _participantService.GetFollowersAsync(eventId);
			if (ev.Capacity > 0 && participants.Count() >= ev.Capacity)
				throw new InvalidOperationException("Event is already full");

			if (participants.Any(p => p.FollowerId == userId))
				throw new InvalidOperationException("Already joined this event");

			await _participantService.SubscribeAsync(userId, eventId);
			return true;
		}

		public async Task<bool> LeaveEventAsync(string eventId, string userId)
		{
			var existing = await _participantService.IsFollowingAsync(userId, eventId);
			if (!existing)
				return false;

			await _participantService.UnsubscribeAsync(userId, eventId);
			return true;
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

		public async Task<int> GetParticipantCountAsync(string eventId)
		{
			var followers = await _participantService.GetFollowersAsync(eventId);
			return followers.Count();
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
