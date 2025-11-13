using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
	public interface ISqlExecutor
	{
		Task<List<Dictionary<string, object>>> ExecuteRawQueryAsync(
			string sql,
			CancellationToken cancellationToken = default);
	}
}
