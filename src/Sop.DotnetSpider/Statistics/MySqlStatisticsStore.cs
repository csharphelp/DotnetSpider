using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using MySql.Data.MySqlClient;
using Sop.DotnetSpider.Common;
using Sop.DotnetSpider.Entity;

namespace Sop.DotnetSpider.Statistics
{
	/// <summary>
	/// 
	/// </summary>
	public class MySqlStatisticsStore : IStatisticsStore
	{
		private readonly SpiderOptions _options;
		/// <summary>
		/// 
		/// </summary>
		/// <param name="options"></param>
		public MySqlStatisticsStore(SpiderOptions options)
		{
			_options = options;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public async Task EnsureDatabaseAndTableCreatedAsync()
		{
			using (var conn = new MySqlConnection(_options.ConnectionString))
			{
				await conn.ExecuteAsync("CREATE SCHEMA IF NOT EXISTS sopspider DEFAULT CHARACTER SET utf8mb4;");
				var sql1 =
					$"create table if not exists sopspider.spider_statistics(owner_id nvarchar(40) primary key, `start` timestamp null, `exit` timestamp null, total bigint default 0 not null, success bigint default 0 not null, failed bigint default 0 not null, creation_time timestamp default CURRENT_TIMESTAMP not null, last_modification_time timestamp default CURRENT_TIMESTAMP not null, key CREATION_TIME_INDEX (`creation_time`), key LAST_MODIFICATION_TIME_INDEX (`last_modification_time`));";
				var sql2 =
					$"create table if not exists sopspider.download_statistics(agent_id nvarchar(40) primary key, success bigint default 0 not null, failed bigint default 0 not null, elapsed_milliseconds bigint default 0 not null, creation_time timestamp default CURRENT_TIMESTAMP not null, last_modification_time timestamp default CURRENT_TIMESTAMP not null, key CREATION_TIME_INDEX (`creation_time`), key LAST_MODIFICATION_TIME_INDEX (`last_modification_time`));";
				await conn.ExecuteAsync(sql1);
				await conn.ExecuteAsync(sql2);
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="ownerId"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		public async Task IncrementTotalAsync(string ownerId, int count)
		{
			var sql =
				$"INSERT INTO sopspider.spider_statistics (owner_id, total) VALUES (@OwnerId, @Count) ON DUPLICATE key UPDATE total = total + @Count, last_modification_time = CURRENT_TIMESTAMP;";
			using (var conn = new MySqlConnection(_options.ConnectionString))
			{
				await conn.ExecuteAsync(sql,
					new
					{
						OwnerId = ownerId,
						Count = count
					});
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="ownerId"></param>
		/// <returns></returns>
		public async Task IncrementSuccessAsync(string ownerId)
		{
			var sql =
				$"INSERT INTO sopspider.spider_statistics (owner_id) VALUES (@OwnerId) ON DUPLICATE key UPDATE success = success + 1, last_modification_time = CURRENT_TIMESTAMP;";
			using (var conn = new MySqlConnection(_options.ConnectionString))
			{
				await conn.ExecuteAsync(sql,
					new
					{
						OwnerId = ownerId
					});
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="ownerId"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		public async Task IncrementFailedAsync(string ownerId, int count = 1)
		{
			var sql =
				$"INSERT INTO sopspider.spider_statistics (owner_id) VALUES (@OwnerId) ON DUPLICATE key UPDATE failed = failed + 1, last_modification_time = CURRENT_TIMESTAMP;";
			using (var conn = new MySqlConnection(_options.ConnectionString))
			{
				await conn.ExecuteAsync(sql,
					new
					{
						OwnerId = ownerId
					});
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="ownerId"></param>
		/// <returns></returns>
		public async Task StartAsync(string ownerId)
		{
			var sql =
				$"INSERT INTO sopspider.spider_statistics (owner_id, start) VALUES (@OwnerId, @Start) ON DUPLICATE key UPDATE start = @Start, last_modification_time = CURRENT_TIMESTAMP;";
			using (var conn = new MySqlConnection(_options.ConnectionString))
			{
				await conn.ExecuteAsync(sql,
					new
					{
						OwnerId = ownerId,
						Start = DateTime.Now
					});
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="ownerId"></param>
		/// <returns></returns>
		public async Task ExitAsync(string ownerId)
		{
			var sql =
				$"INSERT INTO sopspider.spider_statistics (owner_id, `exit`) VALUES (@OwnerId, @Exit) ON DUPLICATE key UPDATE `exit` = @Exit, last_modification_time = CURRENT_TIMESTAMP;";
			using (var conn = new MySqlConnection(_options.ConnectionString))
			{
				await conn.ExecuteAsync(sql,
					new
					{
						OwnerId = ownerId,
						Exit = DateTime.Now
					});
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="agentId"></param>
		/// <param name="count"></param>
		/// <param name="elapsedMilliseconds"></param>
		/// <returns></returns>
		public async Task IncrementDownloadSuccessAsync(string agentId, int count, long elapsedMilliseconds)
		{
			var sql =
				$"INSERT INTO sopspider.download_statistics (agent_id, success, elapsed_milliseconds) VALUES (@AgentId, @Count, @ElapsedMilliseconds) ON DUPLICATE key UPDATE success = success + @Count, elapsed_milliseconds = elapsed_milliseconds + @ElapsedMilliseconds, last_modification_time = CURRENT_TIMESTAMP;";
			using (var conn = new MySqlConnection(_options.ConnectionString))
			{
				await conn.ExecuteAsync(sql,
					new
					{
						AgentId = agentId,
						Count = count,
						ElapsedMilliseconds = elapsedMilliseconds
					});
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="agentId"></param>
		/// <param name="count"></param>
		/// <param name="elapsedMilliseconds"></param>
		/// <returns></returns>
		public async Task IncrementDownloadFailedAsync(string agentId, int count, long elapsedMilliseconds)
		{
			var sql =
				$"INSERT INTO sopspider.download_statistics (agent_id, failed, elapsed_milliseconds) VALUES (@AgentId, @Count, @ElapsedMilliseconds) ON DUPLICATE key UPDATE failed = failed + @Count, elapsed_milliseconds = elapsed_milliseconds + @ElapsedMilliseconds, last_modification_time = CURRENT_TIMESTAMP;";
			using (var conn = new MySqlConnection(_options.ConnectionString))
			{
				await conn.ExecuteAsync(sql,
					new
					{
						AgentId = agentId,
						Count = count,
						ElapsedMilliseconds = elapsedMilliseconds
					});
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="page"></param>
		/// <param name="size"></param>
		/// <returns></returns>
		public async Task<IEnumerable<DownloadStatistics>> GetDownloadStatisticsListAsync(int page, int size)
		{
			using (var conn = new MySqlConnection(_options.ConnectionString))
			{
				var start = size * (page - 1);
				return (await conn.QueryAsync<DownloadStatistics>(
					$"SELECT * FROM sopspider.download_statistics ORDER BY creation_time LIMIT @Start, @Offset;",
					new
					{
						Start = start,
						Offfset = size
					})).ToList();
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="agentId"></param>
		/// <returns></returns>
		public async Task<DownloadStatistics> GetDownloadStatisticsAsync(string agentId)
		{
			using (var conn = new MySqlConnection(_options.ConnectionString))
			{
				return await conn.QuerySingleOrDefaultAsync<DownloadStatistics>(
					$"SELECT * FROM sopspider.download_statistics WHERE agent_id = @AgentId LIMIT 1;",
					new
					{
    						AgentId = agentId
					});
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="ownerId"></param>
		/// <returns></returns>
		public async Task<SpiderStatistics> GetSpiderStatisticsAsync(string ownerId)
		{
			using (var conn = new MySqlConnection(_options.ConnectionString))
			{
				return await conn.QuerySingleOrDefaultAsync<SpiderStatistics>(
					$"SELECT * FROM sopspider.spider_statistics WHERE owner_id = @OwnerId LIMIT 1;",
					new
					{
						OwnerId = ownerId
					});
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="page"></param>
		/// <param name="size"></param>
		/// <returns></returns>
		public async Task<IEnumerable<SpiderStatistics>> GetSpiderStatisticsListAsync(int page, int size)
		{
			using (var conn = new MySqlConnection(_options.ConnectionString))
			{
				var start = size * (page - 1);
				return (await conn.QueryAsync<SpiderStatistics>(
					$"SELECT * FROM sopspider.spider_statistics ORDER BY creation_time LIMIT @Start, @Offset;",
					new
					{
						Start = start,
						Offfset = size
					})).ToList();
			}
		}
	}
}