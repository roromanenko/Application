using Infrastructure.Interfaces;
using Infrastructure.Persistence.Entity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
	public class ParticipantRepository : IParticipantRepository
	{
		private readonly ApplicationDbContext _dbContext;

		public ParticipantRepository(ApplicationDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		public async Task<ParticipantEntity> CreateSubscription(ParticipantEntity entity)
		{
			await _dbContext.Participants.AddAsync(entity);
			await _dbContext.SaveChangesAsync();
			return entity;
		}

		public async Task<ParticipantEntity?> GetSubscription(Guid userId, Guid eventId)
		{
			return await _dbContext.Participants
				.FirstOrDefaultAsync(p => p.UserId == userId && p.EventId == eventId);
		}

		public async Task<IEnumerable<ParticipantEntity>> GetByFollower(Guid userId)
		{
			return await _dbContext.Participants
				.Where(p => p.UserId == userId)
				.ToListAsync();
		}

		public async Task<IEnumerable<ParticipantEntity>> GetByTarget(Guid eventId)
		{
			return await _dbContext.Participants
				.Where(p => p.EventId == eventId)
				.ToListAsync();
		}

		public async Task DeleteSubscription(Guid id)
		{
			var participant = await _dbContext.Participants.FindAsync(id);
			if (participant != null)
			{
				_dbContext.Participants.Remove(participant);
				await _dbContext.SaveChangesAsync();
			}
		}
	}
}