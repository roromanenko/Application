using AutoMapper;
using Core.Domain;
using Core.Interfaces;
using Infrastructure.Interfaces;
using Infrastructure.Persistence.Entity;

namespace Infrastructure.Services
{
	public class TagService : ITagService
	{
		private readonly ITagRepository _tagRepository;
		private readonly IMapper _mapper;

		public TagService(ITagRepository tagRepository, IMapper mapper)
		{
			_tagRepository = tagRepository;
			_mapper = mapper;
		}

		public async Task<IEnumerable<Tag>> GetAllTagsAsync()
		{
			var entities = await _tagRepository.GetAllTags();
			return _mapper.Map<IEnumerable<Tag>>(entities);
		}

		public async Task<Tag?> GetTagByIdAsync(string tagId)
		{
			if (!Guid.TryParse(tagId, out var guid))
				throw new ArgumentException("Invalid tag ID format");

			var entity = await _tagRepository.GetTagById(guid);
			return entity is null ? null : _mapper.Map<Tag>(entity);
		}

		public async Task<Tag?> GetTagByNameAsync(string name)
		{
			var entity = await _tagRepository.GetTagByName(name);
			return entity is null ? null : _mapper.Map<Tag>(entity);
		}

		public async Task<Tag> CreateTagAsync(Tag newTag)
		{
			var existing = await _tagRepository.GetTagByName(newTag.Name);
			if (existing != null)
				throw new InvalidOperationException($"Tag '{newTag.Name}' already exists.");

			var entity = _mapper.Map<TagEntity>(newTag);
			var created = await _tagRepository.CreateTag(entity);
			return _mapper.Map<Tag>(created);
		}

		public async Task<bool> DeleteTagAsync(string tagId)
		{
			if (!Guid.TryParse(tagId, out var guid))
				throw new ArgumentException("Invalid tag ID format");

			return await _tagRepository.DeleteTag(guid);
		}
	}
}
