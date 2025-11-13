namespace Api.DTO
{
	public class AiCapabilitiesResponse
	{
		public string Description { get; set; }
		public List<string> ExampleQueries { get; set; }
		public List<string> SupportedOperations { get; set; }
	}
}
