using Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
	public interface IUserService
	{
		Task<User?> VerifyUserLogin(string usernameOrEmail, string password);
		Task<User> RegisterUser(string username, string email, string firstName, string lastName, string password);
		Task<User> GetUserById(string userId);
		Task<bool> UpdateUser(User user);
		Task ChangePassword(string userId, string newPassword);
	}
}
