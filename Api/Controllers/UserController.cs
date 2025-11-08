using Api.DTO;
using AutoMapper;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class UserController : BaseController
	{
		private readonly IUserService _userService;
		private readonly ILogger<UserController> _logger;
		private readonly IJwtService _jwtService;
		private readonly IMapper _mapper;

		public UserController(IUserService userService, ILogger<UserController> logger, IJwtService jwtService, IMapper mapper)
		{
			_userService = userService;
			_logger = logger;
			_jwtService = jwtService;
			_mapper = mapper;
		}

		[HttpPost("register")]
		public async Task<ActionResult<ApiResponse<UserDto>>> Register([FromBody] RegisterRequest request)
		{
			var user = await _userService.RegisterUser(
				request.Username, request.Email, request.FirstName, request.LastName, request.Password);

			var response = _mapper.Map<UserDto>(user);
			return Ok(new ApiResponse<UserDto>(true, "User registered successfully", response));
		}

		[HttpPost("login")]
		public async Task<ActionResult<ApiResponse<LoginResponse>>> Login([FromBody] LoginRequest request)
		{
			var user = await _userService.VerifyUserLogin(request.UsernameOrEmail, request.Password);
			if (user == null)
				return Ok(new ApiResponse<LoginResponse>(false, "Invalid credentials"));

			var token = _jwtService.GenerateToken(user);
			var userResponse = _mapper.Map<UserDto>(user);
			var loginResponse = new LoginResponse(userResponse, token);

			return Ok(new ApiResponse<LoginResponse>(true, "Login successful", loginResponse));
		}

		[HttpPut("password")]
		[Authorize]
		public async Task<ActionResult<ApiResponse<string>>> ChangePassword([FromBody] ChangePasswordRequest request)
		{
			var user = await _userService.VerifyUserLogin(Username!, request.CurrentPassword);
			if (user == null)
				return Ok(new ApiResponse<string>(false, "Current password is incorrect"));

			await _userService.ChangePassword(UserId!, request.NewPassword);
			return Ok(new ApiResponse<string>(true, "Password changed successfully"));
		}

		[HttpPut("profile")]
		[Authorize]
		public async Task<ActionResult<ApiResponse<string>>> UpdateProfile([FromBody] UpdateUserRequest request)
		{
			var user = await _userService.GetUserById(UserId!);
			if (user == null)
				return Ok(new ApiResponse<string>(false, "User not found"));

			if (!string.IsNullOrWhiteSpace(request.FirstName))
				user.FirstName = request.FirstName;
			if (!string.IsNullOrWhiteSpace(request.LastName))
				user.LastName = request.LastName;
			if (!string.IsNullOrWhiteSpace(request.Username))
				user.Username = request.Username;
			if (!string.IsNullOrWhiteSpace(request.Email))
				user.Email = request.Email;

			var success = await _userService.UpdateUser(user);
			return Ok(new ApiResponse<string>(success, success ? "Profile updated successfully" : "Update failed"));
		}

		[HttpGet("me")]
		[Authorize]
		public async Task<ActionResult<ApiResponse<UserDto>>> GetCurrentUser()
		{
			try
			{
				if (string.IsNullOrEmpty(UserId))
					return Unauthorized(new ApiResponse<UserDto>(false, "Invalid token or user not authenticated"));

				var user = await _userService.GetUserById(UserId);
				var userDto = _mapper.Map<UserDto>(user);
				return Ok(new ApiResponse<UserDto>(true, "User found", userDto));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error getting current user");
				return StatusCode(500, new ApiResponse<UserDto>(false, "Internal server error"));
			}
		}
	}
}
