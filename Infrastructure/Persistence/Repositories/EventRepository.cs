using Core.Options;
using Infrastructure.Interfaces;
using Infrastructure.Persistence.Entity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
	public class EventRepository : IEventRepository
	{
		private readonly ApplicationDbContext _dbContext;

		public EventRepository(ApplicationDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		public async Task<EventEntity> CreateEvent(EventEntity newEvent)
		{
			foreach (var tag in newEvent.Tags)
			{
				_dbContext.Tags.Attach(tag);
			}

			await _dbContext.Events.AddAsync(newEvent);
			await _dbContext.SaveChangesAsync();

			foreach (var tag in newEvent.Tags)
			{
				_dbContext.Entry(tag).State = EntityState.Detached;
			}

			newEvent.Tags.Clear();
			await _dbContext.Entry(newEvent).Collection(x => x.Tags).LoadAsync();

			return newEvent;
		}

		public async Task<bool> AddTagsToEvent(Guid eventId, IEnumerable<Guid> tagIds)
		{
			var eventEntity = await _dbContext.Events
				.Include(e => e.Tags)
				.FirstOrDefaultAsync(e => e.Id == eventId);

			if (eventEntity == null)
				return false;

			var tags = await _dbContext.Tags
				.Where(t => tagIds.Contains(t.Id))
				.ToListAsync();

			foreach (var tag in tags)
			{
				if (!eventEntity.Tags.Contains(tag))
					eventEntity.Tags.Add(tag);
			}

			await _dbContext.SaveChangesAsync();
			return true;
		}

		public async Task<bool> RemoveTagFromEvent(Guid eventId, Guid tagId)
		{
			var eventEntity = await _dbContext.Events
				.Include(e => e.Tags)
				.FirstOrDefaultAsync(e => e.Id == eventId);

			if (eventEntity == null)
				return false;

			var tag = eventEntity.Tags.FirstOrDefault(t => t.Id == tagId);
			if (tag == null)
				return false;

			eventEntity.Tags.Remove(tag);
			await _dbContext.SaveChangesAsync();
			return true;
		}

		public async Task<EventEntity?> GetEventById(Guid eventId)
		{
			return await _dbContext.Events
				.Include(x => x.Tags)
				.FirstOrDefaultAsync(e => e.Id == eventId);
		}

		public async Task<IEnumerable<EventEntity>> GetEventsByTitle(string title)
		{
			return await _dbContext.Events
				.Include(x => x.Tags)
				.Where(e => EF.Functions.ILike(e.Title, $"%{title}%"))
				.ToListAsync();
		}

		public async Task<IEnumerable<EventEntity>> GetEventsByOrganizer(Guid organizerId)
		{
			return await _dbContext.Events
				.Include(x => x.Tags)
				.Where(e => e.OrganizerId == organizerId)
				.ToListAsync();
		}

		public async Task<IEnumerable<EventEntity>> GetEvents(EventQueryOptions options)
		{
			var query = _dbContext.Events.Include(x => x.Tags).AsQueryable();

			if (!string.IsNullOrEmpty(options.OrganizerId))
			{
				var organizerGuid = Guid.Parse(options.OrganizerId);
				query = query.Where(e => e.OrganizerId == organizerGuid);
			}

			if (!string.IsNullOrEmpty(options.Title))
			{
				query = query.Where(e => EF.Functions.ILike(e.Title, $"%{options.Title}%"));
			}

			query = options.SortBy?.ToLower() switch
			{
				"startdate" => options.SortDescending
					? query.OrderByDescending(e => e.StartDate)
					: query.OrderBy(e => e.StartDate),
				"participants" => options.SortDescending
					? query.OrderByDescending(e => e.Participants.Count)
					: query.OrderBy(e => e.Participants.Count),
				_ => options.SortDescending
					? query.OrderByDescending(e => e.CreatedAt)
					: query.OrderBy(e => e.CreatedAt)
			};

			return await query
				.Skip((options.Page - 1) * options.PageSize)
				.Take(options.PageSize)
				.ToListAsync();
		}

		public async Task<bool> UpdateEvent(EventEntity updatedEvent)
		{
			updatedEvent.UpdatedAt = DateTimeOffset.UtcNow;
			_dbContext.Events.Update(updatedEvent);
			var result = await _dbContext.SaveChangesAsync();
			return result > 0;
		}

		public async Task<bool> DeleteEvent(Guid eventId)
		{
			var eventEntity = await _dbContext.Events.FindAsync(eventId);
			if (eventEntity != null)
			{
				_dbContext.Events.Remove(eventEntity);
				var result = await _dbContext.SaveChangesAsync();
				return result > 0;
			}
			return false;
		}
	}
}