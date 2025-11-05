using Core.Interfaces;
using Core.Options;
using Infrastructure.Interfaces;
using Infrastructure.Persistence.Entity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Text.RegularExpressions;

namespace Infrastructure.Persistence.Repositories
{
	public class EventRepository : IEventRepository
	{
		private readonly MongoDbOptions _mongoDbOptions;
		private readonly IMongoDbContext _dbContext;

		public EventRepository(IOptions<MongoDbOptions> mongoDbOptions, IMongoDbContext dbContext)
		{
			_mongoDbOptions = mongoDbOptions.Value;
			_dbContext = dbContext;
		}

		public async Task<EventEntity> CreateEvent(EventEntity newEvent)
		{
			await _dbContext.GetCollection<EventEntity>().InsertOneAsync(newEvent);
			return newEvent;
		}

		public async Task<EventEntity> GetEventById(ObjectId eventId)
		{
			var filter = Builders<EventEntity>.Filter.Eq(e => e.Id, eventId);
			return await _dbContext.GetCollection<EventEntity>().Find(filter).FirstOrDefaultAsync(); 
		}

		public async Task<IEnumerable<EventEntity>> GetEventsByTitle(string title)
		{
			var filter = Builders<EventEntity>.Filter.Regex(
				e => e.Title,
				new BsonRegularExpression(title, "i")
			);

			return await _dbContext
				.GetCollection<EventEntity>()
				.Find(filter)
				.ToListAsync();
		}

		public async Task<IEnumerable<EventEntity>> GetEventsByOrganizer(string organizerId)
		{
			var filter = Builders<EventEntity>.Filter.Eq(e => e.OrganizerId, organizerId);
			return await _dbContext.GetCollection<EventEntity>().Find(filter).ToListAsync();
		}

		public async Task<IEnumerable<EventEntity>> GetEvents(EventQueryOptions options)
		{
			var collection = _dbContext.GetCollection<EventEntity>();
			var filterBuilder = Builders<EventEntity>.Filter;
			var filters = new List<FilterDefinition<EventEntity>>();

			if (!string.IsNullOrEmpty(options.OrganizerId))
				filters.Add(filterBuilder.Eq(e => e.OrganizerId, options.OrganizerId));

			if (!string.IsNullOrEmpty(options.Title))
				filters.Add(filterBuilder.Regex(e => e.Title, new BsonRegularExpression(options.Title, "i")));

			var finalFilter = filters.Any() ? filterBuilder.And(filters) : filterBuilder.Empty;

			SortDefinition<EventEntity> sort = options.SortBy?.ToLower() switch
			{
				"startdate" => options.SortDescending
					? Builders<EventEntity>.Sort.Descending(e => e.StartDate)
					: Builders<EventEntity>.Sort.Ascending(e => e.StartDate),
					/*
				"participants" => options.SortDescending
					? Builders<EventEntity>.Sort.Descending(e => e.ParticipantIds.Count)
					: Builders<EventEntity>.Sort.Ascending(e => e.ParticipantIds.Count),
					*/
				_ => options.SortDescending
					? Builders<EventEntity>.Sort.Descending(e => e.CreatedAt)
					: Builders<EventEntity>.Sort.Ascending(e => e.CreatedAt)
			};

			return await collection
				.Find(finalFilter)
				.Sort(sort)
				.Skip((options.Page - 1) * options.PageSize)
				.Limit(options.PageSize)
				.ToListAsync();
		}

		public async Task<bool> UpdateEvent(EventEntity updatedEvent)
		{
			var filter = Builders<EventEntity>.Filter.Eq(e => e.Id, updatedEvent.Id);
			updatedEvent.UpdatedAt = DateTime.UtcNow;

			var result = await _dbContext.GetCollection<EventEntity>()
				.ReplaceOneAsync(filter, updatedEvent);

			return result.ModifiedCount > 0;
		}

		public async Task<bool> DeleteEvent(ObjectId eventId)
		{
			var filter = Builders<EventEntity>.Filter.Eq(e => e.Id, eventId);
			var result = await _dbContext.GetCollection<EventEntity>().DeleteOneAsync(filter);
			return result.DeletedCount > 0;
		}
	}
}
