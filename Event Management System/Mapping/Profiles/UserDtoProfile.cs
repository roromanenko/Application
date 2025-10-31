using AutoMapper;
using Core.Domain;
using Api.DTO;

namespace Api.Mapping.Profiles
{
	public class UserDtoProfile : Profile
	{
		public UserDtoProfile()
		{
			CreateMap<User, UserDto>().ReverseMap();
		}
	}
}
