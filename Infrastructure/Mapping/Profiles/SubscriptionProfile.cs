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
	public class SubscriptionProfile : Profile
	{
		public SubscriptionProfile()
		{
			CreateMap<Subscription, SubscriptionEntity>()
				.ConstructUsing(s => new SubscriptionEntity
				{
					Id = string.IsNullOrEmpty(s.Id) ? ObjectId.GenerateNewId() : ObjectId.Parse(s.Id),
					FollowerId = s.FollowerId,
					TargetId = s.TargetId,
					TargetType = s.TargetType.ToString(),
					IsBlocked = s.IsBlocked,
					CreatedAt = s.CreatedAt
				});

			CreateMap<SubscriptionEntity, Subscription>()
				.ConstructUsing(e => new Subscription
				{
					Id = e.Id.ToString(),
					FollowerId = e.FollowerId,
					TargetId = e.TargetId,
					TargetType = Enum.Parse<SubscriptionTargetType>(e.TargetType, true),
					IsBlocked = e.IsBlocked,
					CreatedAt = e.CreatedAt
				});
		}
	}
}
