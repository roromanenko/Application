using Api.DTO;
using AutoMapper;
using Core.Domain;

namespace Api.Mapping.Profiles
{
	public class ParticipantDtoProfile : Profile
	{
		public ParticipantDtoProfile()
		{
			CreateMap<Participant, ParticipantDto>().ReverseMap();
		}
	}
}