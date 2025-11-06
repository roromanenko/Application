using Infrastructure.Persistence.Entity;

namespace Infrastructure.Interfaces
{
	public interface IUserRepository
	{
		Task<UserEntity> CreateUser(UserEntity newUser);
		Task<UserEntity> GetUserByUsernameOrEmail(string usernameOrEmail);
		Task<UserEntity> GetUserById(Guid userid);
		Task UpdateUser(UserEntity user);
		Task ChangePassword(Guid userId, string newPasswordHash);
	}
}
