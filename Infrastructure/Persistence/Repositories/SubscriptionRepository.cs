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
	public class SubscriptionRepository : ISubscriptionRepository
	{
		private readonly MongoDbOptions _mongoDbOptions;
		private readonly IMongoDbContext _dbContext;

		public SubscriptionRepository(IOptions<MongoDbOptions> mongoDbOptions, IMongoDbContext dbContext)
		{
			_mongoDbOptions = mongoDbOptions.Value;
			_dbContext = dbContext;
		}

		public async Task<SubscriptionEntity> CreateSubscription(SubscriptionEntity entity)
		{
			await _dbContext.GetCollection<SubscriptionEntity>().InsertOneAsync(entity);
			return entity;
		}

		public async Task<SubscriptionEntity?> GetSubscription(string followerId, string targetId, string targetType)
		{
			var filter = Builders<SubscriptionEntity>.Filter.Eq(s => s.FollowerId, followerId) &
						 Builders<SubscriptionEntity>.Filter.Eq(s => s.TargetId, targetId) &
						 Builders<SubscriptionEntity>.Filter.Eq(s => s.TargetType, targetType);

			return await _dbContext.GetCollection<SubscriptionEntity>()
				.Find(filter)
				.FirstOrDefaultAsync();
		}

		public async Task<IEnumerable<SubscriptionEntity>> GetByFollower(string followerId)
		{
			var filter = Builders<SubscriptionEntity>.Filter.Eq(s => s.FollowerId, followerId);
			return await _dbContext.GetCollection<SubscriptionEntity>()
				.Find(filter)
				.ToListAsync();
		}

		public async Task<IEnumerable<SubscriptionEntity>> GetByTarget(string targetId)
		{
			var filter = Builders<SubscriptionEntity>.Filter.Eq(s => s.TargetId, targetId);
			return await _dbContext.GetCollection<SubscriptionEntity>()
				.Find(filter)
				.ToListAsync();
		}

		public async Task DeleteSubscription(ObjectId id)
		{
			var filter = Builders<SubscriptionEntity>.Filter.Eq(s => s.Id, id);
			await _dbContext.GetCollection<SubscriptionEntity>().DeleteOneAsync(filter);
		}
	}
}
