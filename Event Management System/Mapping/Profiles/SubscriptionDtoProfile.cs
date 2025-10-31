using Api.DTO;
using AutoMapper;
using Core.Domain;

namespace Api.Mapping.Profiles
{
	public class SubscriptionDtoProfile : Profile
	{
		public SubscriptionDtoProfile()
		{
			CreateMap<Subscription, SubscriptionDto>().ReverseMap();
		}
	}
}