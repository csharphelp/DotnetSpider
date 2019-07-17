using System;
using Sop.Spider.Common;
using Sop.Spider.DataFlow;
using Sop.Spider.DownloadAgent;
using Sop.Spider.DownloadAgentRegisterCenter;
using Sop.Spider.DownloadAgentRegisterCenter.Internal;
using Sop.Spider.EventBus;
using Sop.Spider.Network;
using Sop.Spider.Network.InternetDetector;
using Sop.Spider.Statistics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Sop.Spider
{
	/// <summary>
	/// ע���¼���չ
	/// </summary>
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection ConfigureAppConfiguration(this IServiceCollection services,
			string config = null)
		{
			Check.NotNull(services, nameof(services));

			var configurationBuilder = Framework.CreateConfigurationBuilder(config);
			IConfigurationRoot configurationRoot = configurationBuilder.Build();
			services.AddSingleton<IConfiguration>(configurationRoot);

			return services;
		}

		#region DownloadCenter

		public static IServiceCollection AddDownloadCenter(this IServiceCollection services,
			Action<DownloadAgentRegisterCenterBuilder> configure = null)
		{
			services.AddSingleton<IHostedService, DefaultDownloadAgentRegisterCenter>();

			DownloadAgentRegisterCenterBuilder downloadCenterBuilder = new DownloadAgentRegisterCenterBuilder(services);
			configure?.Invoke(downloadCenterBuilder);

			return services;
		}
		/// <summary>
		/// ע�뱾�������¼�
		/// </summary>
		/// <param name="services"></param>
		/// <returns></returns>
		public static IServiceCollection AddLocalDownloadCenter(this IServiceCollection services)
		{
			services.AddDownloadCenter(x => x.UseLocalDownloaderAgentStore());
			return services;
		}

		public static DownloadAgentRegisterCenterBuilder UseMySqlDownloaderAgentStore(
			this DownloadAgentRegisterCenterBuilder builder)
		{
			builder.Services.AddSingleton<IDownloaderAgentStore, MySqlDownloaderAgentStore>();
			return builder;
		}
		/// <summary>
		/// ʹ��ע�뱾���¼����ش���洢
		/// </summary>
		/// <param name="builder"></param>
		/// <returns></returns>
		public static DownloadAgentRegisterCenterBuilder UseLocalDownloaderAgentStore(
			this DownloadAgentRegisterCenterBuilder builder)
		{
			Check.NotNull(builder, nameof(builder));
			builder.Services.AddSingleton<IDownloaderAgentStore, LocalDownloaderAgentStore>();
			return builder;
		}

		#endregion

		#region  EventBus
		/// <summary>
		/// �¼�ע�루�����¼�ע�룩
		/// </summary>
		/// <param name="services"></param>
		/// <returns></returns>
		public static IServiceCollection AddLocalEventBus(this IServiceCollection services)
		{
			services.AddSingleton<IEventBus, LocalEventBus>();
			return services;
		}

		#endregion

		#region DownloaderAgent
		/// <summary>
		/// ע���¼����ش�����
		/// </summary>
		/// <param name="services">ע�����</param>
		/// <param name="configure">�������ί���¼�</param>
		/// <returns></returns>
		public static IServiceCollection AddDownloaderAgent(this IServiceCollection services,
			Action<DownloaderAgentBuilder> configure = null)
		{
			
			services.AddSingleton<IHostedService, DefaultDownloaderAgent>();
			services.AddSingleton<NetworkCenter>();
			services.AddSingleton<DownloaderAgentOptions>();

			DownloaderAgentBuilder spiderAgentBuilder = new DownloaderAgentBuilder(services);
			configure?.Invoke(spiderAgentBuilder);

			return services;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="builder"></param>
		/// <returns></returns>
		
		public static DownloaderAgentBuilder UseFileLocker(this DownloaderAgentBuilder builder)
		{
			Check.NotNull(builder, nameof(builder));

			builder.Services.AddSingleton<ILockerFactory, FileLockerFactory>();

			return builder;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="builder"></param>
		/// <returns></returns>
		[Obsolete("���粦�����ϳ�������֮�ж���·����֪ͨ����")]
		public static DownloaderAgentBuilder UseDefaultAdslRedialer(this DownloaderAgentBuilder builder)
		{
			Check.NotNull(builder, nameof(builder));

			builder.Services.AddSingleton<IAdslRedialer, DefaultAdslRedialer>();

			return builder;
		}

		public static DownloaderAgentBuilder UseDefaultInternetDetector(this DownloaderAgentBuilder builder)
		{
			Check.NotNull(builder, nameof(builder));

			builder.Services.AddSingleton<IInternetDetector, DefaultInternetDetector>();

			return builder;
		}

		public static DownloaderAgentBuilder UseVpsInternetDetector(this DownloaderAgentBuilder builder)
		{
			Check.NotNull(builder, nameof(builder));

			builder.Services.AddSingleton<IInternetDetector, VpsInternetDetector>();

			return builder;
		}

		#endregion

		#region  Statistics

		public static IServiceCollection AddStatisticsCenter(this IServiceCollection services,
			Action<StatisticsBuilder> configure)
		{
			services.AddSingleton<IHostedService, StatisticsCenter>();

			var spiderStatisticsBuilder = new StatisticsBuilder(services);
			configure?.Invoke(spiderStatisticsBuilder);

			return services;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="builder"></param>
		/// <returns></returns>
		public static StatisticsBuilder UseMemory(this StatisticsBuilder builder)
		{
			Check.NotNull(builder, nameof(builder));
			builder.Services.AddSingleton<IStatisticsStore, MemoryStatisticsStore>();
			return builder;
		}

		public static StatisticsBuilder UseMySql(this StatisticsBuilder builder)
		{
			Check.NotNull(builder, nameof(builder));
			builder.Services.AddSingleton<IStatisticsStore, MySqlStatisticsStore>();
			return builder;
		}

		#endregion
		
		#region Sop.Spider

		public static IServiceCollection AddSpider(this IServiceCollection services)
		{
			
			return services;
		}
		
		#endregion
	}
}