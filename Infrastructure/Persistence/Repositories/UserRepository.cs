using Infrastructure.Interfaces;
using Infrastructure.Persistence.Entity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
	public class UserRepository : IUserRepository
	{
		private readonly ApplicationDbContext _dbContext;

		public UserRepository(ApplicationDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		public async Task<UserEntity> CreateUser(UserEntity newUser)
		{
			await _dbContext.Users.AddAsync(newUser);
			await _dbContext.SaveChangesAsync();
			return newUser;
		}

		public async Task<UserEntity?> GetUserByUsernameOrEmail(string usernameOrEmail)
		{
			var user = await _dbContext.Users
				.FirstOrDefaultAsync(u => u.Username == usernameOrEmail || u.Email == usernameOrEmail);
			return user;
		}

		public async Task<UserEntity?> GetUserById(Guid userid)
		{
			var user = await _dbContext.Users
				.FirstOrDefaultAsync(u => u.Id == userid);
			return user;
		}

		public async Task UpdateUser(UserEntity user)
		{
			_dbContext.Users.Update(user);
			await _dbContext.SaveChangesAsync();
		}

		public async Task ChangePassword(Guid userId, string newPasswordHash)
		{
			var user = await _dbContext.Users.FindAsync(userId);
			if (user != null)
			{
				user.PasswordHash = newPasswordHash;
				await _dbContext.SaveChangesAsync();
			}
		}

		public async Task DeleteUser(Guid userId)
		{
			var user = await _dbContext.Users.FindAsync(userId);
			if (user != null)
			{
				_dbContext.Users.Remove(user);
				await _dbContext.SaveChangesAsync();
			}
		}

		public async Task<IEnumerable<UserEntity>> GetAllUsers()
		{
			return await _dbContext.Users.ToListAsync();
		}

		public async Task<bool> UserExistsByUsername(string username)
		{
			return await _dbContext.Users.AnyAsync(u => u.Username == username);
		}
	}
}