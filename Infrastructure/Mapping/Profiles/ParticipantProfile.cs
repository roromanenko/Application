using AutoMapper;
using Core.Domain;
using Infrastructure.Persistence.Entity;

namespace Infrastructure.Mapping.Profiles
{
	public class ParticipantProfile : Profile
	{
		public ParticipantProfile()
		{
			CreateMap<Participant, ParticipantEntity>()
				.ForMember(dest => dest.Id, opt => opt.MapFrom(src =>
					string.IsNullOrEmpty(src.Id) ? Guid.NewGuid() : Guid.Parse(src.Id)))
				.ForMember(dest => dest.UserId, opt => opt.MapFrom(src => Guid.Parse(src.UserId)))
				.ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User))
				.ForMember(dest => dest.EventId, opt => opt.MapFrom(src => Guid.Parse(src.EventId)));

			CreateMap<ParticipantEntity, Participant>()
				.ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
				.ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId.ToString()))
				.ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User))
				.ForMember(dest => dest.EventId, opt => opt.MapFrom(src => src.EventId.ToString()));
		}
	}
}
