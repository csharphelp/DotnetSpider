using System;
using Sop.DotnetSpider.Common;
using Sop.DotnetSpider.EventBus;
using Sop.DotnetSpider.Network;
using Microsoft.Extensions.Logging;

namespace Sop.DotnetSpider.DownloadAgent
{
	/// <summary>
	/// 本地下器代理
	/// </summary>
	public class LocalDownloadedAgent : DefaultDownloadAgent
	{
	 
		/// <summary>
		/// 构造方法
		/// </summary>
		/// <param name="options">下载器代理选项</param>
		/// <param name="spiderOptions"></param>
		/// <param name="eventBus">消息队列</param>
		/// <param name="logger">日志接口</param>
		public LocalDownloadedAgent(DownloadAgentOptions options, SpiderOptions spiderOptions,
			IEventBus eventBus,
			ILogger<LocalDownloadedAgent> logger) : base(options, spiderOptions,
			eventBus, logger)
		{
			// ConfigureDownload = download => download.Logger = null;
		}
	}
}