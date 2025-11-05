using AutoMapper;
using Core.Domain;
using Infrastructure.Persistence.Entity;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Mapping.Profiles
{
	public class EventProfile : Profile
	{
		public EventProfile()
		{
			CreateMap<Event, EventEntity>()
			.ConstructUsing(e => new EventEntity
			{
				Id = string.IsNullOrEmpty(e.Id)
						? ObjectId.GenerateNewId()
						: ObjectId.Parse(e.Id),
				Title = e.Title,
				Description = e.Description,
				OrganizerId = e.OrganizerId,
				StartDate = e.StartDate,
				EndDate = e.EndDate,
				Location = e.Location,
				IsPublic = e.IsPublic,
				Capacity = e.Capacity,
				CreatedAt = e.CreatedAt,
				UpdatedAt = e.UpdatedAt
			});

			CreateMap<EventEntity, Event>()
			.ConstructUsing(e => new Event
			{
				Id = e.Id.ToString(),
				Title = e.Title,
				Description = e.Description,
				OrganizerId = e.OrganizerId,
				StartDate = e.StartDate,
				EndDate = e.EndDate,
				Location = e.Location,
				IsPublic = e.IsPublic,
				Capacity = e.Capacity,
				CreatedAt = e.CreatedAt,
				UpdatedAt = e.UpdatedAt
			});
		}
	}
}
