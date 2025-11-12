using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain
{
	public class AiAssistantResponse
	{
		public bool Success { get; set; }
		public string? Answer { get; set; }
		public List<Dictionary<string, object>> Data { get; set; }
		public string? Query { get; set; }
		public string? Explanation { get; set; }
		public string? ErrorMessage { get; set; }
		public int ResultCount { get; set; }
	}
}
