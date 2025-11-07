using Api.DTO;
using AutoMapper;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class ParticipantController : BaseController
	{
		private readonly IParticipantService _participantService;
		private readonly IMapper _mapper;
		private readonly ILogger<ParticipantController> _logger;

		public ParticipantController(
			IParticipantService participantService,
			IMapper mapper,
			ILogger<ParticipantController> logger)
		{
			_participantService = participantService;
			_mapper = mapper;
			_logger = logger;
		}

		[HttpPost("{eventId}/join")]
		[Authorize]
		public async Task<ActionResult<ApiResponse<bool>>> JoinEvent(string eventId)
		{
			if (string.IsNullOrWhiteSpace(eventId))
				return BadRequest(new ApiResponse<bool>(false, "Event ID is required"));

			try
			{
				await _participantService.SubscribeAsync(UserId!, eventId);
				return Ok(new ApiResponse<bool>(true, "Joined successfully", true));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error joining event");
				return Ok(new ApiResponse<bool>(false, ex.Message));
			}
		}

		[HttpPost("{eventId}/leave")]
		[Authorize]
		public async Task<ActionResult<ApiResponse<bool>>> LeaveEvent(string eventId)
		{
			if (string.IsNullOrWhiteSpace(eventId))
				return BadRequest(new ApiResponse<bool>(false, "Event ID is required"));

			try
			{
				var success = await _participantService.UnsubscribeAsync(UserId!, eventId);
				return Ok(new ApiResponse<bool>(true, success ? "Left successfully" : "Not subscribed", success));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error leaving event");
				return Ok(new ApiResponse<bool>(false, ex.Message));
			}
		}

		[HttpGet("{eventId}/followers")]
		[AllowAnonymous]
		public async Task<ActionResult<ApiResponse<IEnumerable<ParticipantDto>>>> GetEventFollowers(string eventId)
		{
			if (string.IsNullOrWhiteSpace(eventId))
				return BadRequest(new ApiResponse<IEnumerable<ParticipantDto>>(false, "Event ID is required"));

			var participants = await _participantService.GetFollowersAsync(eventId);
			var participantDtos = _mapper.Map<IEnumerable<ParticipantDto>>(participants);

			return Ok(new ApiResponse<IEnumerable<ParticipantDto>>(true, "Participants retrieved", participantDtos));
		}

		[HttpGet("{eventId}/is-following")]
		[Authorize]
		public async Task<ActionResult<ApiResponse<bool>>> IsFollowing(string eventId)
		{
			if (string.IsNullOrWhiteSpace(eventId))
				return BadRequest(new ApiResponse<bool>(false, "Event ID is required"));

			var isFollowing = await _participantService.IsFollowingAsync(UserId!, eventId);
			return Ok(new ApiResponse<bool>(true, "Check complete", isFollowing));
		}

		[HttpGet("following")]
		[Authorize]
		public async Task<ActionResult<ApiResponse<IEnumerable<ParticipantDto>>>> GetFollowing([FromQuery] string userId)
		{
			if (string.IsNullOrWhiteSpace(userId))
				return BadRequest(new ApiResponse<IEnumerable<ParticipantDto>>(false, "User ID is required"));

			try
			{
				var subscriptions = await _participantService.GetFollowingAsync(userId);
				var subscriptionDtos = _mapper.Map<IEnumerable<ParticipantDto>>(subscriptions);

				return Ok(new ApiResponse<IEnumerable<ParticipantDto>>(true, "Following events retrieved", subscriptionDtos));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error retrieving following events");
				return Ok(new ApiResponse<IEnumerable<ParticipantDto>>(false, ex.Message));
			}
		}
	}
}
