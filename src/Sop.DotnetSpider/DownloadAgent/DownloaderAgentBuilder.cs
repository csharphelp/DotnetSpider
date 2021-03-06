using Microsoft.Extensions.DependencyInjection;

namespace Sop.DotnetSpider.DownloadAgent
{
	public class DownloadAgentBuilder
	{
		public IServiceCollection Services { get; }
		
		public DownloadAgentBuilder(IServiceCollection services)
		{
			Services = services;
		}
	}
}