using Api.DTO;
using AutoMapper;
using Core.Domain;
using Core.Interfaces;
using Core.Options;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class EventController : BaseController
	{
		private readonly IEventService _eventService;
		private readonly IMapper _mapper;
		private readonly ILogger<EventController> _logger;

		public EventController(IEventService eventService, IMapper mapper, ILogger<EventController> logger)
		{
			_eventService = eventService;
			_mapper = mapper;
			_logger = logger;
		}

		[HttpPost]
		[Authorize]
		public async Task<ActionResult<ApiResponse<EventDto>>> CreateEvent([FromBody] CreateEventRequest request)
		{
			try
			{
				var newEvent = _mapper.Map<Event>(request);
				newEvent.OrganizerId = UserId!;
				var created = await _eventService.CreateEventAsync(newEvent);
				var eventDto = _mapper.Map<EventDto>(created);

				return Ok(new ApiResponse<EventDto>(true, "Event created successfully", eventDto));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error creating event");
				return StatusCode(500, new ApiResponse<EventDto>(false, "Error creating event"));
			}
		}

		[HttpGet("{id}")]
		[AllowAnonymous]
		public async Task<ActionResult<ApiResponse<EventDto>>> GetEventById(string id)
		{
			var @event = await _eventService.GetEventByIdAsync(id);
			if (@event == null)
				return Ok(new ApiResponse<EventDto>(false, "Event not found"));

			var eventDto = _mapper.Map<EventDto>(@event);
			return Ok(new ApiResponse<EventDto>(true, "Event retrieved successfully", eventDto));
		}

		[HttpGet]
		[AllowAnonymous]
		public async Task<ActionResult<ApiResponse<IEnumerable<EventDto>>>> GetEvents(
			[FromQuery] int page = 1,
			[FromQuery] int pageSize = 20,
			[FromQuery] string? sortBy = "createdAt",
			[FromQuery] bool sortDescending = true,
			[FromQuery] string? title = null,
			[FromQuery] string? organizerId = null)
		{
			var options = new EventQueryOptions
			{
				Page = page,
				PageSize = pageSize,
				SortBy = sortBy,
				SortDescending = sortDescending,
				Title = title,
				OrganizerId = organizerId
			};

			bool isAuthorized = User?.Identity?.IsAuthenticated == true;

			var events = await _eventService.GetEventsAsync(options, includePrivate: isAuthorized);
			var eventDtosList = _mapper.Map<IEnumerable<EventDto>>(events);

			return Ok(new ApiResponse<IEnumerable<EventDto>>(true, "Events retrieved successfully", eventDtosList));
		}

		[HttpGet("mine")]
		[Authorize]
		public async Task<ActionResult<ApiResponse<IEnumerable<EventDto>>>> GetMyEvents()
		{
			var events = await _eventService.GetEventsByOrganizerAsync(UserId!);
			var eventDtos = _mapper.Map<IEnumerable<EventDto>>(events);

			return Ok(new ApiResponse<IEnumerable<EventDto>>(true, "Your events retrieved successfully", eventDtos));
		}

		[HttpGet("upcoming")]
		[AllowAnonymous]
		public async Task<ActionResult<ApiResponse<IEnumerable<EventDto>>>> GetUpcomingEvents(
			[FromQuery] int daysAhead = 30,
			[FromQuery] int page = 1,
			[FromQuery] int pageSize = 20)
		{
			var options = new EventQueryOptions
			{
				Page = page,
				PageSize = pageSize,
				SortBy = "startDate",
				SortDescending = false
			};

			var allEvents = await _eventService.GetEventsAsync(options, includePrivate: false);
			var upcoming = allEvents
				.Where(e => e.StartDate >= DateTime.UtcNow && e.StartDate <= DateTime.UtcNow.AddDays(daysAhead))
				.OrderBy(e => e.StartDate)
				.ToList();

			var eventDtos = _mapper.Map<IEnumerable<EventDto>>(upcoming);
			return Ok(new ApiResponse<IEnumerable<EventDto>>(true, "Upcoming events retrieved successfully", eventDtos));
		}

		[HttpGet("popular")]
		[AllowAnonymous]
		public async Task<ActionResult<ApiResponse<IEnumerable<EventDto>>>> GetPopularEvents(
			[FromQuery] int page = 1,
			[FromQuery] int pageSize = 20)
		{
			var options = new EventQueryOptions
			{
				Page = page,
				PageSize = pageSize,
				SortBy = "participants",
				SortDescending = true
			};

			var allEvents = await _eventService.GetEventsAsync(options, includePrivate: false);
			var popular = allEvents
				//.OrderByDescending(e => e.ParticipantIds?.Count ?? 0)
				.ToList();

			var eventDtos = _mapper.Map<IEnumerable<EventDto>>(popular);
			return Ok(new ApiResponse<IEnumerable<EventDto>>(true, "Popular events retrieved successfully", eventDtos));
		}

		[HttpPut("{id}")]
		[Authorize]
		public async Task<ActionResult<ApiResponse<bool>>> UpdateEvent(string id, [FromBody] UpdateEventRequest request)
		{
			try
			{
				var updated = _mapper.Map<Event>(request);
				updated.Id = id;
				updated.OrganizerId = UserId!;

				var success = await _eventService.UpdateEventAsync(updated);
				if (!success)
					return Ok(new ApiResponse<bool>(false, "Event not found or update failed"));

				return Ok(new ApiResponse<bool>(true, "Event updated successfully", true));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error updating event");
				return StatusCode(500, new ApiResponse<bool>(false, "Error updating event"));
			}
		}

		[HttpDelete("{id}")]
		[Authorize]
		public async Task<ActionResult<ApiResponse<bool>>> DeleteEvent(string id)
		{
			var success = await _eventService.DeleteEventAsync(id);
			if (!success)
				return Ok(new ApiResponse<bool>(false, "Event not found or already deleted"));

			return Ok(new ApiResponse<bool>(true, "Event deleted successfully", true));
		}

		[HttpPost("{id}/join")]
		[Authorize]
		public async Task<ActionResult<ApiResponse<bool>>> JoinEvent(string id)
		{
			try
			{
				var success = await _eventService.JoinEventAsync(id, UserId!);
				return Ok(new ApiResponse<bool>(true, "Joined successfully", success));
			}
			catch (Exception ex)
			{
				return Ok(new ApiResponse<bool>(false, ex.Message));
			}
		}

		[HttpPost("{id}/leave")]
		[Authorize]
		public async Task<ActionResult<ApiResponse<bool>>> LeaveEvent(string id)
		{
			try
			{
				var success = await _eventService.LeaveEventAsync(id, UserId!);
				return Ok(new ApiResponse<bool>(true, "Left successfully", success));
			}
			catch (Exception ex)
			{
				return Ok(new ApiResponse<bool>(false, ex.Message));
			}
		}

		[HttpGet("{id}/participants/count")]
		[AllowAnonymous]
		public async Task<ActionResult<ApiResponse<int>>> GetParticipantCount(string id)
		{
			var count = await _eventService.GetParticipantCountAsync(id);
			return Ok(new ApiResponse<int>(true, "Count retrieved", count));
		}
	}
}
