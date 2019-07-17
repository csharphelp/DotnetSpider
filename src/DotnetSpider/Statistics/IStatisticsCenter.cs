#if !NET451
using Microsoft.Extensions.Hosting;

#else
using Sop.Spider.Core;
#endif

namespace Sop.Spider.Statistics
{
	/// <summary>
	/// 统计服务中心
	/// </summary>
	public interface IStatisticsCenter : IHostedService
	{
	}
}