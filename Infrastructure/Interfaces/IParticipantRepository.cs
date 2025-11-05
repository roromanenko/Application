using Core.Domain;
using Infrastructure.Persistence.Entity;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Interfaces
{
	public interface IParticipantRepository
	{
		Task<ParticipantEntity> CreateSubscription(ParticipantEntity entity);
		Task<ParticipantEntity?> GetSubscription(string followerId, string targetId);
		Task<IEnumerable<ParticipantEntity>> GetByFollower(string followerId);
		Task<IEnumerable<ParticipantEntity>> GetByTarget(string targetId);
		Task DeleteSubscription(ObjectId id);
	}
}
