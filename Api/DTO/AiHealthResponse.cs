namespace Api.DTO
{
	public class AiHealthResponse
	{
		public string Status { get; set; }
		public string Service { get; set; }
		public DateTime Timestamp { get; set; }
		public string? Error { get; set; }
	}
}
