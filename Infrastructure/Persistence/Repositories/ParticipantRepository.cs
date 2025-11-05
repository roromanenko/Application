using Core.Domain;
using Core.Interfaces;
using Core.Options;
using Infrastructure.Interfaces;
using Infrastructure.Persistence.Entity;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Repositories
{
	public class ParticipantRepository : IParticipantRepository
	{
		private readonly MongoDbOptions _mongoDbOptions;
		private readonly IMongoDbContext _dbContext;

		public ParticipantRepository(IOptions<MongoDbOptions> mongoDbOptions, IMongoDbContext dbContext)
		{
			_mongoDbOptions = mongoDbOptions.Value;
			_dbContext = dbContext;
		}

		public async Task<ParticipantEntity> CreateSubscription(ParticipantEntity entity)
		{
			await _dbContext.GetCollection<ParticipantEntity>().InsertOneAsync(entity);
			return entity;
		}

		public async Task<ParticipantEntity?> GetSubscription(string followerId, string targetId)
		{
			var filter = Builders<ParticipantEntity>.Filter.Eq(s => s.FollowerId, followerId) &
						 Builders<ParticipantEntity>.Filter.Eq(s => s.TargetId, targetId);

			return await _dbContext.GetCollection<ParticipantEntity>()
				.Find(filter)
				.FirstOrDefaultAsync();
		}

		public async Task<IEnumerable<ParticipantEntity>> GetByFollower(string followerId)
		{
			var filter = Builders<ParticipantEntity>.Filter.Eq(s => s.FollowerId, followerId);
			return await _dbContext.GetCollection<ParticipantEntity>()
				.Find(filter)
				.ToListAsync();
		}

		public async Task<IEnumerable<ParticipantEntity>> GetByTarget(string targetId)
		{
			var filter = Builders<ParticipantEntity>.Filter.Eq(s => s.TargetId, targetId);
			return await _dbContext.GetCollection<ParticipantEntity>()
				.Find(filter)
				.ToListAsync();
		}

		public async Task DeleteSubscription(ObjectId id)
		{
			var filter = Builders<ParticipantEntity>.Filter.Eq(s => s.Id, id);
			await _dbContext.GetCollection<ParticipantEntity>().DeleteOneAsync(filter);
		}
	}
}
