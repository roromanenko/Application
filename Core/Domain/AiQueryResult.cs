using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain
{
	public class AiQueryResult
	{
		public string Sql { get; set; }
		public string Explanation { get; set; }
		public string Request { get; set; }
		public string Notes { get; set; }
		public bool IsValid => !string.IsNullOrEmpty(Sql);
	}
}
