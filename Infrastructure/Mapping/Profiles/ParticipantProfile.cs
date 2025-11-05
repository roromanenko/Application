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
	public class ParticipantProfile : Profile
	{
		public ParticipantProfile()
		{
			CreateMap<Participant, ParticipantEntity>()
				.ConstructUsing(s => new ParticipantEntity
				{
					Id = string.IsNullOrEmpty(s.Id) ? ObjectId.GenerateNewId() : ObjectId.Parse(s.Id),
					FollowerId = s.FollowerId,
					TargetId = s.TargetId,
					CreatedAt = s.CreatedAt
				});

			CreateMap<ParticipantEntity, Participant>()
				.ConstructUsing(e => new Participant
				{
					Id = e.Id.ToString(),
					FollowerId = e.FollowerId,
					TargetId = e.TargetId,
					CreatedAt = e.CreatedAt
				});
		}
	}
}
