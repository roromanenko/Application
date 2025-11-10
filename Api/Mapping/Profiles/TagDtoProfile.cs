using Api.DTO;
using AutoMapper;
using Core.Domain;

namespace Api.Mapping.Profiles
{
	public class TagDtoProfile : Profile
	{
		public TagDtoProfile()
		{
			CreateMap<Tag, TagDto>().ReverseMap();
		}
	}
}
