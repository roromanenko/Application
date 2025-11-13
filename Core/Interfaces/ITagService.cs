using Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
	public interface ITagService
	{
		Task<IEnumerable<Tag>> GetAllTagsAsync();
		Task<Tag?> GetTagByIdAsync(string tagId);
		Task<Tag?> GetTagByNameAsync(string name);
		Task<Tag> CreateTagAsync(Tag newTag);
		Task<bool> DeleteTagAsync(string tagId);
	}
}
