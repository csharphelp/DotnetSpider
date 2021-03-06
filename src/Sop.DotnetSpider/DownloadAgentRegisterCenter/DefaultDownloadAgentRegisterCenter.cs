using Sop.DotnetSpider.Common;
using Sop.DotnetSpider.EventBus;
using Microsoft.Extensions.Logging;

namespace Sop.DotnetSpider.DownloadAgentRegisterCenter
{
	/// <summary>
	/// 下载中心
	/// </summary>
	public class DefaultDownloadAgentRegisterCenter : DownloadAgentRegisterCenterBase
	{
		/// <summary>
		/// 构造方法
		/// </summary>
		/// <param name="eventBus">消息队列</param>
		/// <param name="downloaderAgentStore">下载器代理存储</param>
		/// <param name="options">系统选项</param>
		/// <param name="logger">日志接口</param>
		public DefaultDownloadAgentRegisterCenter(IEventBus eventBus, IDownloadAgentStore downloaderAgentStore, SpiderOptions options,
			ILogger<DefaultDownloadAgentRegisterCenter> logger) : base(eventBus, downloaderAgentStore, options, logger)
		{
		}
	}
}