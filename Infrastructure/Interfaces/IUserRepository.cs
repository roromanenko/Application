using Infrastructure.Persistence.Entity;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Interfaces
{
	public interface IUserRepository
	{
		Task<UserEntity> CreateUser(UserEntity newUser);
		Task<UserEntity> GetUserByUsernameOrEmail(string usernameOrEmail);
		Task<UserEntity> GetUserById(ObjectId userid);
		Task UpdateUser(UserEntity user);
		Task ChangePassword(ObjectId userId, string newPasswordHash);
	}
}
