using AutoMapper;
using Core.Domain;
using Infrastructure.Persistence.Entity;

namespace Infrastructure.Mapping.Profiles
{
	public class UserProfile : Profile
	{
		public UserProfile()
		{
			CreateMap<User, UserEntity>()
				.ForMember(dest => dest.Id, opt => opt.MapFrom(src =>
					string.IsNullOrEmpty(src.Id) ? Guid.NewGuid() : Guid.Parse(src.Id)))
				.ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Username))
				.ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
				.ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
				.ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
				.ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => src.PasswordHash))
				.ForMember(dest => dest.Roles, opt => opt.MapFrom(src => src.Roles));

			CreateMap<UserEntity, User>()
				.ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
				.ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Username))
				.ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
				.ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
				.ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
				.ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => src.PasswordHash))
				.ForMember(dest => dest.Roles, opt => opt.MapFrom(src => src.Roles));
		}
	}
}
