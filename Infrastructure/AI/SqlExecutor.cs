using Core.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.AI
{
	public class SqlExecutor : ISqlExecutor
	{
		private readonly ApplicationDbContext _dbContext;
		private readonly ILogger<SqlExecutor> _logger;

		public SqlExecutor(
			ApplicationDbContext dbContext,
			ILogger<SqlExecutor> logger)
		{
			_dbContext = dbContext;
			_logger = logger;
		}

		public async Task<List<Dictionary<string, object>>> ExecuteRawQueryAsync(
			string sql,
			CancellationToken cancellationToken = default)
		{
			if (!IsSelectQuery(sql))
			{
				_logger.LogWarning("Attempted to execute non-SELECT query: {Sql}", sql);
				throw new InvalidOperationException("Only SELECT queries are allowed");
			}

			try
			{
				_logger.LogInformation("Executing AI-generated SQL: {Sql}", sql);

				using var command = _dbContext.Database.GetDbConnection().CreateCommand();
				command.CommandText = sql;
				command.CommandType = System.Data.CommandType.Text;

				await _dbContext.Database.OpenConnectionAsync(cancellationToken);

				using var reader = await command.ExecuteReaderAsync(cancellationToken);
				var results = new List<Dictionary<string, object>>();

				while (await reader.ReadAsync(cancellationToken))
				{
					var row = new Dictionary<string, object>();

					for (int i = 0; i < reader.FieldCount; i++)
					{
						var columnName = reader.GetName(i);
						var value = await reader.IsDBNullAsync(i, cancellationToken) ? null : reader.GetValue(i);
						row[columnName] = value;
					}

					results.Add(row);
				}

				return results;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error executing AI-generated SQL: {Sql}", sql);
				throw new InvalidOperationException("Failed to execute query", ex);
			}
		}

		private bool IsSelectQuery(string sql)
		{
			if (string.IsNullOrWhiteSpace(sql))
				return false;

			var trimmed = sql.Trim().ToUpperInvariant();

			if (!trimmed.StartsWith("SELECT"))
				return false;

			var dangerousKeywords = new[]
			{
			"INSERT", "UPDATE", "DELETE", "DROP", "ALTER", "CREATE",
			"TRUNCATE", "EXEC", "EXECUTE", "GRANT", "REVOKE"
		};

			foreach (var keyword in dangerousKeywords)
			{
				if (trimmed.Contains(keyword))
				{
					_logger.LogWarning("Dangerous keyword detected in SQL: {Keyword}", keyword);
					return false;
				}
			}

			return true;
		}
	}
}
