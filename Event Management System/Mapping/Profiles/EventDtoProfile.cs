using Api.DTO;
using AutoMapper;
using Core.Domain;

namespace Api.Mapping.Profiles
{
	public class EventDtoProfile : Profile
	{
		public EventDtoProfile()
		{
			CreateMap<CreateEventRequest, Event>();
			CreateMap<UpdateEventRequest, Event>();
			CreateMap<Event, EventDto>().ReverseMap();
		}
	}
}
