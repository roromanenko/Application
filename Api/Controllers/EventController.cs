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
			var newEvent = _mapper.Map<Event>(request);
			newEvent.OrganizerId = UserId!;

			var created = await _eventService.CreateEventAsync(newEvent);
			var eventDto = _mapper.Map<EventDto>(created);

			return Ok(new ApiResponse<EventDto>(true, "Event created successfully", eventDto));
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

			var isAuthorized = User?.Identity?.IsAuthenticated == true;
			var events = await _eventService.GetEventsAsync(options, includePrivate: isAuthorized);
			var eventDtosList = _mapper.Map<IEnumerable<EventDto>>(events);

			return Ok(new ApiResponse<IEnumerable<EventDto>>(true, "Events retrieved successfully", eventDtosList));
		}

		[HttpPut("{id}")]
		[Authorize]
		public async Task<ActionResult<ApiResponse<bool>>> UpdateEvent(string id, [FromBody] UpdateEventRequest request)
		{
			var updated = _mapper.Map<Event>(request);
			updated.Id = id;
			updated.OrganizerId = UserId!;

			var success = await _eventService.UpdateEventAsync(updated);
			if (!success)
				return Ok(new ApiResponse<bool>(false, "Event not found or update failed"));

			return Ok(new ApiResponse<bool>(true, "Event updated successfully", true));
		}

		[HttpDelete("{id}")]
		[Authorize]
		public async Task<ActionResult<ApiResponse<bool>>> DeleteEvent(string id)
		{
			var success = await _eventService.DeleteEventAsync(id);
			return Ok(new ApiResponse<bool>(success, success ? "Event deleted successfully" : "Event not found", success));
		}
	}
}
