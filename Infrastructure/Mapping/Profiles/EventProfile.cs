using AutoMapper;
using Core.Domain;
using Infrastructure.Persistence.Entity;

namespace Infrastructure.Mapping.Profiles
{
	public class EventProfile : Profile
	{
		public EventProfile()
		{
			CreateMap<Event, EventEntity>()
				.ForMember(dest => dest.Id, opt => opt.MapFrom(src =>
					string.IsNullOrEmpty(src.Id) ? Guid.NewGuid() : Guid.Parse(src.Id)))
				.ForMember(dest => dest.OrganizerId, opt => opt.MapFrom(src => Guid.Parse(src.OrganizerId)))
				.ForMember(dest => dest.Organizer, opt => opt.Ignore())
				.ForMember(dest => dest.Participants, opt => opt.Ignore())
				.ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Tags));

			CreateMap<EventEntity, Event>()
				.ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
				.ForMember(dest => dest.OrganizerId, opt => opt.MapFrom(src => src.OrganizerId.ToString()))
				.ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Tags));
		}
	}
}