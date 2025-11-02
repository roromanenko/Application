using AutoMapper;
using Infrastructure.Persistence.Entity;
using Core.Domain;
using MongoDB.Bson;

namespace Infrastructure.Mapping.Profiles
{
	public class UserProfile : Profile
	{
		public UserProfile()
		{
			CreateMap<User, UserEntity>()
				.ConstructUsing(u => new UserEntity
				{
					Id = ObjectId.Parse(u.Id),
					Username = u.Username,
					Email = u.Email,
					FirstName = u.FirstName,
					LastName = u.LastName,
					PasswordHash = u.PasswordHash,
					Roles = u.Roles.ToList()
				});

			CreateMap<UserEntity, User>()
			.ConstructUsing(e => new User
			{
				Id = e.Id.ToString(),
				Username = e.Username,
				Email = e.Email,
				FirstName = e.FirstName,
				LastName = e.LastName,
				PasswordHash = e.PasswordHash,
				Roles = e.Roles.ToList()
			});
		}
	}
}
