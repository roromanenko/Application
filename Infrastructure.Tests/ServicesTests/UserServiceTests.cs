using AutoMapper;
using Core.Domain;
using Infrastructure.Interfaces;
using Infrastructure.Persistence.Entity;
using Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace Infrastructure.Tests.Services
{
	public class UserServiceTests
	{
		private readonly Mock<IUserRepository> _userRepositoryMock;
		private readonly Mock<IMapper> _mapperMock;
		private readonly PasswordHasher<UserEntity> _passwordHasher;
		private readonly UserService _userService;

		public UserServiceTests()
		{
			_userRepositoryMock = new Mock<IUserRepository>();
			_mapperMock = new Mock<IMapper>();
			_passwordHasher = new PasswordHasher<UserEntity>();
			_userService = new UserService(_userRepositoryMock.Object, _passwordHasher, _mapperMock.Object);
		}

		#region VerifyUserLogin Tests

		[Fact]
		public async Task VerifyUserLogin_WithValidCredentials_ReturnsUser()
		{
			// Arrange
			var username = "testuser";
			var password = "password123";
			var userEntity = new UserEntity
			{
				Id = Guid.NewGuid(),
				Username = username,
				Email = "test@example.com",
				PasswordHash = _passwordHasher.HashPassword(null!, password)
			};
			var expectedUser = new User
			{
				Id = userEntity.Id.ToString(),
				Username = username,
				Email = "test@example.com",
				FirstName = "John",
				LastName = "Doe",
				PasswordHash = userEntity.PasswordHash,
				Roles = new List<string> { "user" }
			};

			_userRepositoryMock
				.Setup(x => x.GetUserByUsernameOrEmail(username))
				.ReturnsAsync(userEntity);
			_mapperMock
				.Setup(x => x.Map<User>(userEntity))
				.Returns(expectedUser);

			// Act
			var result = await _userService.VerifyUserLogin(username, password);

			// Assert
			Assert.NotNull(result);
			Assert.Equal(expectedUser.Username, result.Username);
			_userRepositoryMock.Verify(x => x.GetUserByUsernameOrEmail(username), Times.Once);
		}

		[Fact]
		public async Task VerifyUserLogin_WithInvalidPassword_ReturnsNull()
		{
			// Arrange
			var username = "testuser";
			var correctPassword = "password123";
			var wrongPassword = "wrongpassword";
			var userEntity = new UserEntity
			{
				Id = Guid.NewGuid(),
				Username = username,
				PasswordHash = _passwordHasher.HashPassword(null!, correctPassword)
			};

			_userRepositoryMock
				.Setup(x => x.GetUserByUsernameOrEmail(username))
				.ReturnsAsync(userEntity);

			// Act
			var result = await _userService.VerifyUserLogin(username, wrongPassword);

			// Assert
			Assert.Null(result);
			_mapperMock.Verify(x => x.Map<User>(It.IsAny<UserEntity>()), Times.Never);
		}

		[Fact]
		public async Task VerifyUserLogin_WithNonExistentUser_ReturnsNull()
		{
			// Arrange
			var username = "nonexistent";
			var password = "password123";

			_userRepositoryMock
				.Setup(x => x.GetUserByUsernameOrEmail(username))
				.ReturnsAsync((UserEntity?)null);

			// Act
			var result = await _userService.VerifyUserLogin(username, password);

			// Assert
			Assert.Null(result);
			_mapperMock.Verify(x => x.Map<User>(It.IsAny<UserEntity>()), Times.Never);
		}

		#endregion

		#region RegisterUser Tests

		[Fact]
		public async Task RegisterUser_WithValidData_CreatesAndReturnsUser()
		{
			// Arrange
			var username = "newuser";
			var email = "newuser@example.com";
			var firstName = "John";
			var lastName = "Doe";
			var password = "password123";

			var createdUserEntity = new UserEntity
			{
				Id = Guid.NewGuid(),
				Username = username,
				Email = email,
				FirstName = firstName,
				LastName = lastName,
				Roles = new List<string> { "user" }
			};

			var expectedUser = new User
			{
				Id = createdUserEntity.Id.ToString(),
				Username = username,
				Email = email,
				FirstName = firstName,
				LastName = lastName,
				PasswordHash = "hashedpassword",
				Roles = new List<string> { "user" }
			};

			_userRepositoryMock
				.Setup(x => x.GetUserByUsernameOrEmail(username))
				.ReturnsAsync((UserEntity?)null);
			_userRepositoryMock
				.Setup(x => x.CreateUser(It.IsAny<UserEntity>()))
				.ReturnsAsync(createdUserEntity);
			_mapperMock
				.Setup(x => x.Map<User>(createdUserEntity))
				.Returns(expectedUser);

			// Act
			var result = await _userService.RegisterUser(username, email, firstName, lastName, password);

			// Assert
			Assert.NotNull(result);
			Assert.Equal(expectedUser.Username, result.Username);
			Assert.Equal(expectedUser.Email, result.Email);
			Assert.Equal(expectedUser.FirstName, result.FirstName);
			Assert.Equal(expectedUser.LastName, result.LastName);
			_userRepositoryMock.Verify(x => x.GetUserByUsernameOrEmail(username), Times.Once);
			_userRepositoryMock.Verify(x => x.CreateUser(It.Is<UserEntity>(u =>
				u.Username == username &&
				u.Email == email &&
				u.FirstName == firstName &&
				u.LastName == lastName &&
				u.Roles.Contains("user"))), Times.Once);
		}

		[Fact]
		public async Task RegisterUser_WithExistingUsername_ThrowsArgumentException()
		{
			// Arrange
			var username = "existinguser";
			var existingUser = new UserEntity { Username = username };

			_userRepositoryMock
				.Setup(x => x.GetUserByUsernameOrEmail(username))
				.ReturnsAsync(existingUser);

			// Act & Assert
			var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
				_userService.RegisterUser(username, "email@test.com", "John", "Doe", "password"));

			Assert.Equal("User with this username or email already exists", exception.Message);
			_userRepositoryMock.Verify(x => x.CreateUser(It.IsAny<UserEntity>()), Times.Never);
		}

		#endregion

		#region GetUserById Tests

		[Fact]
		public async Task GetUserById_WithValidId_ReturnsUser()
		{
			// Arrange
			var userId = Guid.NewGuid();
			var userEntity = new UserEntity
			{
				Id = userId,
				Username = "testuser",
				Email = "test@example.com"
			};
			var expectedUser = new User
			{
				Id = userId.ToString(),
				Username = "testuser",
				Email = "test@example.com",
				FirstName = "John",
				LastName = "Doe",
				PasswordHash = "hashedpassword",
				Roles = new List<string> { "user" }
			};

			_userRepositoryMock
				.Setup(x => x.GetUserById(userId))
				.ReturnsAsync(userEntity);
			_mapperMock
				.Setup(x => x.Map<User>(userEntity))
				.Returns(expectedUser);

			// Act
			var result = await _userService.GetUserById(userId.ToString());

			// Assert
			Assert.NotNull(result);
			Assert.Equal(expectedUser.Id, result.Id);
			Assert.Equal(expectedUser.Username, result.Username);
			_userRepositoryMock.Verify(x => x.GetUserById(userId), Times.Once);
		}

		[Fact]
		public async Task GetUserById_WithInvalidIdFormat_ThrowsArgumentException()
		{
			// Arrange
			var invalidId = "invalid-guid";

			// Act & Assert
			var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
				_userService.GetUserById(invalidId));

			Assert.Equal("Invalid user ID format", exception.Message);
			_userRepositoryMock.Verify(x => x.GetUserById(It.IsAny<Guid>()), Times.Never);
		}

		[Fact]
		public async Task GetUserById_WithNonExistentId_ThrowsKeyNotFoundException()
		{
			// Arrange
			var userId = Guid.NewGuid();

			_userRepositoryMock
				.Setup(x => x.GetUserById(userId))
				.ReturnsAsync((UserEntity?)null);

			// Act & Assert
			var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() =>
				_userService.GetUserById(userId.ToString()));

			Assert.Contains(userId.ToString(), exception.Message);
		}

		#endregion

		#region UpdateUser Tests

		[Fact]
		public async Task UpdateUser_WithValidUser_ReturnsTrue()
		{
			// Arrange
			var userId = Guid.NewGuid();
			var existingUserEntity = new UserEntity
			{
				Id = userId,
				Username = "oldusername",
				Email = "old@example.com",
				FirstName = "OldFirst",
				LastName = "OldLast"
			};

			var updatedUser = new User
			{
				Id = userId.ToString(),
				Username = "newusername",
				Email = "new@example.com",
				FirstName = "NewFirst",
				LastName = "NewLast",
				PasswordHash = "hashedpassword",
				Roles = new List<string> { "user" }
			};

			_userRepositoryMock
				.Setup(x => x.GetUserById(userId))
				.ReturnsAsync(existingUserEntity);
			_userRepositoryMock
				.Setup(x => x.UpdateUser(It.IsAny<UserEntity>()))
				.Returns(Task.CompletedTask);

			// Act
			var result = await _userService.UpdateUser(updatedUser);

			// Assert
			Assert.True(result);
			_userRepositoryMock.Verify(x => x.GetUserById(userId), Times.Once);
			_userRepositoryMock.Verify(x => x.UpdateUser(It.Is<UserEntity>(u =>
				u.Username == updatedUser.Username &&
				u.Email == updatedUser.Email &&
				u.FirstName == updatedUser.FirstName &&
				u.LastName == updatedUser.LastName)), Times.Once);
		}

		[Fact]
		public async Task UpdateUser_WithNonExistentUser_ReturnsFalse()
		{
			// Arrange
			var userId = Guid.NewGuid();
			var user = new User
			{
				Id = userId.ToString(),
				Username = "testuser",
				Email = "test@example.com",
				FirstName = "John",
				LastName = "Doe",
				PasswordHash = "hashedpassword",
				Roles = new List<string> { "user" }
			};

			_userRepositoryMock
				.Setup(x => x.GetUserById(userId))
				.ReturnsAsync((UserEntity?)null);

			// Act
			var result = await _userService.UpdateUser(user);

			// Assert
			Assert.False(result);
			_userRepositoryMock.Verify(x => x.UpdateUser(It.IsAny<UserEntity>()), Times.Never);
		}

		#endregion

		#region ChangePassword Tests

		[Fact]
		public async Task ChangePassword_WithValidUserId_ChangesPassword()
		{
			// Arrange
			var userId = Guid.NewGuid();
			var newPassword = "newpassword123";
			var userEntity = new UserEntity
			{
				Id = userId,
				Username = "testuser"
			};

			_userRepositoryMock
				.Setup(x => x.GetUserById(userId))
				.ReturnsAsync(userEntity);
			_userRepositoryMock
				.Setup(x => x.ChangePassword(userId, It.IsAny<string>()))
				.Returns(Task.CompletedTask);

			// Act
			await _userService.ChangePassword(userId.ToString(), newPassword);

			// Assert
			_userRepositoryMock.Verify(x => x.GetUserById(userId), Times.Once);
			_userRepositoryMock.Verify(x => x.ChangePassword(userId, It.IsAny<string>()), Times.Once);
		}

		[Fact]
		public async Task ChangePassword_WithInvalidIdFormat_ThrowsArgumentException()
		{
			// Arrange
			var invalidId = "invalid-guid";
			var newPassword = "newpassword123";

			// Act & Assert
			var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
				_userService.ChangePassword(invalidId, newPassword));

			Assert.Equal("Invalid user ID format", exception.Message);
			_userRepositoryMock.Verify(x => x.GetUserById(It.IsAny<Guid>()), Times.Never);
		}

		[Fact]
		public async Task ChangePassword_WithNonExistentUser_ThrowsArgumentException()
		{
			// Arrange
			var userId = Guid.NewGuid();
			var newPassword = "newpassword123";

			_userRepositoryMock
				.Setup(x => x.GetUserById(userId))
				.ReturnsAsync((UserEntity?)null);

			// Act & Assert
			var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
				_userService.ChangePassword(userId.ToString(), newPassword));

			Assert.Equal("User not found", exception.Message);
			_userRepositoryMock.Verify(x => x.ChangePassword(It.IsAny<Guid>(), It.IsAny<string>()), Times.Never);
		}

		#endregion
	}
}