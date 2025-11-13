using AutoMapper;
using Core.Domain;
using Infrastructure.Persistence.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Mapping.Profiles
{
	public class TagProfile : Profile
	{
		public TagProfile()
		{
			CreateMap<Tag, TagEntity>()
				.ForMember(dest => dest.Id, opt => opt.MapFrom(src =>
					string.IsNullOrEmpty(src.Id) ? Guid.NewGuid() : Guid.Parse(src.Id)))
				.ForMember(dest => dest.Events, opt => opt.Ignore());

			CreateMap<TagEntity, Tag>()
				.ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()));
		}
	}
}
