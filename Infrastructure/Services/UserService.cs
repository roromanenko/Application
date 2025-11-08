using AutoMapper;
using Core.Domain;
using Core.Interfaces;
using Infrastructure.Interfaces;
using Infrastructure.Persistence.Entity;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Services
{
	public class UserService : IUserService
	{
		private readonly IUserRepository _userRepository;
		private readonly PasswordHasher<UserEntity> _passwordHasher;
		private readonly IMapper _mapper;

		public UserService(IUserRepository userRepository, PasswordHasher<UserEntity> passwordHasher, IMapper mapper)
		{
			_userRepository = userRepository;
			_passwordHasher = passwordHasher;
			_mapper = mapper;
		}

		public async Task<User?> VerifyUserLogin(string usernameOrEmail, string password)
		{
			var userEntity = await _userRepository.GetUserByUsernameOrEmail(usernameOrEmail);
			if (userEntity is null)
			{
				return default;
			}

			var result = _passwordHasher.VerifyHashedPassword(userEntity, userEntity.PasswordHash, password);
			if (result == PasswordVerificationResult.Success)
			{
				return _mapper.Map<User>(userEntity);
			}

			return default;
		}

		public async Task<User> RegisterUser(string username, string email, string firstName, string lastName, string password)
		{
			var userEntity = await _userRepository.GetUserByUsernameOrEmail(username);
			if (userEntity != null)
			{
				throw new ArgumentException("User with this username or email already exists");
			}

			var newUser = new UserEntity
			{
				Username = username,
				Email = email,
				FirstName = firstName,
				LastName = lastName,
				Roles = ["user"]
			};

			newUser.PasswordHash = _passwordHasher.HashPassword(newUser, password);
			newUser = await _userRepository.CreateUser(newUser);

			return _mapper.Map<User>(newUser);
		}

		public async Task<User> GetUserById(string userId)
		{
			if (!Guid.TryParse(userId, out var userGuid))
				throw new ArgumentException("Invalid user ID format");

			UserEntity? userEntity = await _userRepository.GetUserById(userGuid);
			return userEntity is null
				? throw new KeyNotFoundException($"User with ID '{userId}' was not found.")
				: _mapper.Map<User>(userEntity);
		}

		public async Task<bool> UpdateUser(User user)
		{
			var existingUser = await _userRepository.GetUserById(Guid.Parse(user.Id));
			if (existingUser == null)
				return false;

			existingUser.FirstName = user.FirstName;
			existingUser.LastName = user.LastName;
			existingUser.Email = user.Email;
			existingUser.Username = user.Username;

			await _userRepository.UpdateUser(existingUser);
			return true;
		}

		public async Task ChangePassword(string userId, string newPassword)
		{
			if (!Guid.TryParse(userId, out var userGuid))
				throw new ArgumentException("Invalid user ID format");

			var user = await _userRepository.GetUserById(userGuid)
				?? throw new ArgumentException("User not found");

			var hashedPassword = _passwordHasher.HashPassword(user, newPassword);
			await _userRepository.ChangePassword(userGuid, hashedPassword);
		}
	}
}