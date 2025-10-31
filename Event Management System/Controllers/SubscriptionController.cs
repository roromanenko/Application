using Api.DTO;
using AutoMapper;
using Core.Domain;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	[Authorize]
	public class SubscriptionController : BaseController
	{
		private readonly ISubscriptionService _subscriptionService;
		private readonly ILogger<SubscriptionController> _logger;
		private readonly IMapper _mapper;

		public SubscriptionController(
			ISubscriptionService subscriptionService,
			ILogger<SubscriptionController> logger,
			IMapper mapper)
		{
			_subscriptionService = subscriptionService;
			_logger = logger;
			_mapper = mapper;
		}

		[HttpPost("subscribe")]
		public async Task<ActionResult<ApiResponse<SubscriptionDto>>> Subscribe([FromBody] SubscribeRequest request)
		{
			try
			{
				if (string.IsNullOrEmpty(UserId))
					return Unauthorized(new ApiResponse<SubscriptionDto>(false, "User not authorized"));

				if (!Enum.TryParse<SubscriptionTargetType>(request.TargetType, true, out var type))
					return Ok(new ApiResponse<SubscriptionDto>(false, $"Invalid target type: {request.TargetType}"));

				var subscription = await _subscriptionService.SubscribeAsync(UserId, request.TargetId, type);
				var dto = _mapper.Map<SubscriptionDto>(subscription);

				return Ok(new ApiResponse<SubscriptionDto>(true, "Subscribed successfully", dto));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error subscribing to target {TargetId}", request.TargetId);
				return Ok(new ApiResponse<SubscriptionDto>(false, "Failed to subscribe"));
			}
		}

		[HttpPost("unsubscribe")]
		public async Task<ActionResult<ApiResponse<string>>> Unsubscribe([FromBody] UnsubscribeRequest request)
		{
			try
			{
				if (string.IsNullOrEmpty(UserId))
					return Unauthorized(new ApiResponse<string>(false, "User not authorized"));

				if (!Enum.TryParse<SubscriptionTargetType>(request.TargetType, true, out var type))
					return Ok(new ApiResponse<string>(false, $"Invalid target type: {request.TargetType}"));

				var result = await _subscriptionService.UnsubscribeAsync(UserId, request.TargetId, type);

				return result
					? Ok(new ApiResponse<string>(true, "Unsubscribed successfully"))
					: Ok(new ApiResponse<string>(false, "Subscription not found"));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error unsubscribing from target {TargetId}", request.TargetId);
				return Ok(new ApiResponse<string>(false, "Failed to unsubscribe"));
			}
		}

		[HttpGet("following")]
		public async Task<ActionResult<ApiResponse<IEnumerable<SubscriptionDto>>>> GetFollowing([FromQuery] string? type = null)
		{
			try
			{
				if (string.IsNullOrEmpty(UserId))
					return Unauthorized(new ApiResponse<IEnumerable<SubscriptionDto>>(false, "User not authorized"));

				SubscriptionTargetType? targetType = null;
				if (!string.IsNullOrEmpty(type) && Enum.TryParse<SubscriptionTargetType>(type, true, out var parsedType))
					targetType = parsedType;

				var subs = await _subscriptionService.GetFollowingAsync(UserId, targetType);
				var dto = _mapper.Map<IEnumerable<SubscriptionDto>>(subs);

				return Ok(new ApiResponse<IEnumerable<SubscriptionDto>>(true, "Following list retrieved", dto));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error getting following list");
				return Ok(new ApiResponse<IEnumerable<SubscriptionDto>>(false, "Failed to get following list"));
			}
		}

		[HttpGet("followers")]
		public async Task<ActionResult<ApiResponse<IEnumerable<SubscriptionDto>>>> GetFollowers([FromQuery] string? type = null)
		{
			try
			{
				if (string.IsNullOrEmpty(UserId))
					return Unauthorized(new ApiResponse<IEnumerable<SubscriptionDto>>(false, "User not authorized"));

				SubscriptionTargetType? targetType = null;
				if (!string.IsNullOrEmpty(type) && Enum.TryParse<SubscriptionTargetType>(type, true, out var parsedType))
					targetType = parsedType;

				var subs = await _subscriptionService.GetFollowersAsync(UserId, targetType);
				var dto = _mapper.Map<IEnumerable<SubscriptionDto>>(subs);

				return Ok(new ApiResponse<IEnumerable<SubscriptionDto>>(true, "Followers list retrieved", dto));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error getting followers list");
				return Ok(new ApiResponse<IEnumerable<SubscriptionDto>>(false, "Failed to get followers list"));
			}
		}
	}
}
