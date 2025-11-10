using Infrastructure.Persistence.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Interfaces
{
	public interface ITagRepository
	{
		Task<TagEntity> CreateTag(TagEntity tag);
		Task<TagEntity?> GetTagById(Guid id);
		Task<TagEntity?> GetTagByName(string name);
		Task<IEnumerable<TagEntity>> GetAllTags();
		Task<bool> DeleteTag(Guid id);
	}
}
