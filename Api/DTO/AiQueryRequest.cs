using System.ComponentModel.DataAnnotations;

namespace Api.DTO
{
	public class AiQueryRequest
	{
		[Required(ErrorMessage = "Query is required")]
		[StringLength(500, MinimumLength = 3, ErrorMessage = "Query must be between 3 and 500 characters")]
		public string Query { get; set; }
	}
}
