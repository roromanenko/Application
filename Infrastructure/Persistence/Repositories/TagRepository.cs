using Infrastructure.Interfaces;
using Infrastructure.Persistence.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Repositories
{
	public class TagRepository : ITagRepository
	{
		private readonly ApplicationDbContext _dbContext;

		public TagRepository(ApplicationDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		public async Task<TagEntity> CreateTag(TagEntity tag)
		{
			await _dbContext.Tags.AddAsync(tag);
			await _dbContext.SaveChangesAsync();
			return tag;
		}

		public async Task<TagEntity?> GetTagById(Guid id)
		{
			return await _dbContext.Tags.FindAsync(id);
		}

		public async Task<TagEntity?> GetTagByName(string name)
		{
			return await _dbContext.Tags
				.FirstOrDefaultAsync(t => t.Name.ToLower() == name.ToLower());
		}

		public async Task<IEnumerable<TagEntity>> GetAllTags()
		{
			return await _dbContext.Tags
				.OrderBy(t => t.Name)
				.ToListAsync();
		}

		public async Task<bool> DeleteTag(Guid id)
		{
			var tag = await _dbContext.Tags.FindAsync(id);
			if (tag == null)
				return false;

			_dbContext.Tags.Remove(tag);
			var result = await _dbContext.SaveChangesAsync();
			return result > 0;
		}
	}
}
