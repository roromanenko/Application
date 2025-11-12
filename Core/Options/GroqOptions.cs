using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Options
{
	public class GroqOptions
	{
		public const string SectionName = "Groq";

		[Required(ErrorMessage = "Groq API Key is required")]
		[MinLength(10, ErrorMessage = "Invalid API Key format")]
		public string ApiKey { get; set; } = string.Empty;

		[Required]
		public string Model { get; set; } = "llama-3.3-70b-versatile";

		[Required]
		[Url]
		public string BaseUrl { get; set; } = "https://api.groq.com/openai/v1/chat/completions";

		[Range(0.0, 2.0)]
		public double Temperature { get; set; } = 0.1;

		[Range(100, 10000)]
		public int MaxTokens { get; set; } = 2000;

		[Range(5, 120)]
		public int TimeoutSeconds { get; set; } = 30;
	}
}
