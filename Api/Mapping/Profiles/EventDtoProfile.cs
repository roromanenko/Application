using Api.DTO;
using AutoMapper;
using Core.Domain;

namespace Api.Mapping.Profiles
{
	public class EventDtoProfile : Profile
	{
		public EventDtoProfile()
		{
			CreateMap<CreateEventRequest, Event>()
				.ForMember(dest => dest.Tags, opt => opt.MapFrom(src =>
					src.Tags != null
						? src.Tags.Select(id => new Tag { Id = id, Name = string.Empty }).ToList()
						: new List<Tag>()));

			CreateMap<UpdateEventRequest, Event>()
				.ForMember(dest => dest.Tags, opt => opt.MapFrom(src =>
					src.Tags != null
						? src.Tags.Select(id => new Tag { Id = id, Name = string.Empty }).ToList()
						: new List<Tag>()));

			CreateMap<Event, EventDto>().ReverseMap();
		}
	}
}
