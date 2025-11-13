using Api.DTO;
using AutoMapper;
using Core.Domain;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class TagController : BaseController
	{
		private readonly IMapper _mapper;
		private readonly ILogger<TagController> _logger;
		private readonly ITagService _tagService;

		public TagController(ITagService tagService, IMapper mapper, ILogger<TagController> logger)
		{
			_tagService = tagService;
			_mapper = mapper;
			_logger = logger;
		}

		[HttpGet]
		[Authorize]
		public async Task<ActionResult<ApiResponse<IEnumerable<TagDto>>>> GetAllTags()
		{
			var tags = await _tagService.GetAllTagsAsync();
			var tagDtos = _mapper.Map<IEnumerable<TagDto>>(tags);

			return Ok(new ApiResponse<IEnumerable<TagDto>>(true, "Tags retrieved successfully", tagDtos));
		}

		[HttpGet("{id}")]
		[Authorize]
		public async Task<ActionResult<ApiResponse<TagDto>>> GetTagById(string id)
		{
			var tag = await _tagService.GetTagByIdAsync(id);
			if (tag == null)
				return Ok(new ApiResponse<TagDto>(false, "Tag not found"));

			var tagDto = _mapper.Map<TagDto>(tag);
			return Ok(new ApiResponse<TagDto>(true, "Tag retrieved successfully", tagDto));
		}

		[HttpPost]
		[Authorize(Roles = "admin")]
		public async Task<ActionResult<ApiResponse<TagDto>>> CreateTag([FromBody] TagDtoRequest request)
		{
			try
			{
				var newTag = new Tag { Id = Guid.NewGuid().ToString(), Name = request.Name };
				var created = await _tagService.CreateTagAsync(newTag);
				var tagDto = _mapper.Map<TagDto>(created);

				return Ok(new ApiResponse<TagDto>(true, "Tag created successfully", tagDto));
			}
			catch (InvalidOperationException ex)
			{
				_logger.LogWarning(ex, "Tag with this name already exists");
				return BadRequest(new ApiResponse<TagDto>(false, ex.Message));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error creating tag");
				return StatusCode(500, new ApiResponse<TagDto>(false, "Error creating tag"));
			}
		}

		[HttpDelete("{id}")]
		[Authorize(Roles = "admin")]
		public async Task<ActionResult<ApiResponse<bool>>> DeleteTag(string id)
		{
			try
			{
				var success = await _tagService.DeleteTagAsync(id);
				if (!success)
					return Ok(new ApiResponse<bool>(false, "Tag not found or already deleted"));

				return Ok(new ApiResponse<bool>(true, "Tag deleted successfully", true));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error deleting tag");
				return StatusCode(500, new ApiResponse<bool>(false, "Error deleting tag"));
			}
		}
	}
}
