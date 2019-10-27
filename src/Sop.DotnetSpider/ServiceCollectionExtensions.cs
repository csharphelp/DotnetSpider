using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sop.Spider.Common;
using Sop.Spider.DownloadAgent;
using Sop.Spider.DownloadAgentRegisterCenter;
using Sop.Spider.DownloadAgentRegisterCenter.Internal;
using Sop.Spider.EventBus;
using Sop.Spider.Network.InternetDetector;
using Sop.Spider.Statistics;
using System;


namespace Sop.Spider
{
	/// <summary>
	/// ע���¼���չ
	/// </summary>
	public static class ServiceCollectionExtensions
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="services"></param>
		/// <param name="config"></param>
		/// <returns></returns>
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
			services.AddDownloadCenter(x => x.UseLocalDownloadAgentStore());
			return services;
		}

		public static DownloadAgentRegisterCenterBuilder UseMySqlDownloadAgentStore(
			this DownloadAgentRegisterCenterBuilder builder)
		{
			builder.Services.AddSingleton<IDownloadAgentStore, MySqlDownloadAgentStore>();
			return builder;
		}
		/// <summary>
		/// ʹ��ע�뱾���¼����ش���洢
		/// </summary>
		/// <param name="builder"></param>
		/// <returns></returns>
		public static DownloadAgentRegisterCenterBuilder UseLocalDownloadAgentStore(
			this DownloadAgentRegisterCenterBuilder builder)
		{
			Check.NotNull(builder, nameof(builder));
			builder.Services.AddSingleton<IDownloadAgentStore, LocalDownloadAgentStore>();
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

		#region DownloadAgent
		/// <summary>
		/// ע���¼����ش�����
		/// </summary>
		/// <param name="services">ע�����</param>
		/// <param name="configure">�������ί���¼�</param>
		/// <returns></returns>
		public static IServiceCollection AddDownloadAgent(this IServiceCollection services,
			Action<DownloadAgentBuilder> configure = null)
		{
			
			services.AddSingleton<IHostedService, DefaultDownloadAgent>();
			//services.AddSingleton<NetworkCenter>();
			services.AddSingleton<DownloadAgentOptions>();

			DownloadAgentBuilder spiderAgentBuilder = new DownloadAgentBuilder(services);
			configure?.Invoke(spiderAgentBuilder);

			return services;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="builder"></param>
		/// <returns></returns>
		public static DownloadAgentBuilder UseFileLocker(this DownloadAgentBuilder builder)
		{
			Check.NotNull(builder, nameof(builder));

			builder.Services.AddSingleton<ILockerFactory, FileLockerFactory>();

			return builder;
		}
		/// <summary>
		/// �������
		/// </summary>
		/// <param name="builder"></param>
		/// <returns></returns>
		public static DownloadAgentBuilder UseDefaultInternetDetector(this DownloadAgentBuilder builder)
		{
			Check.NotNull(builder, nameof(builder));

			builder.Services.AddSingleton<IInternetDetector, DefaultInternetDetector>();

			return builder;
		}
 
		#endregion

		#region  Statistics
		/// <summary>
		/// 
		/// </summary>
		/// <param name="services"></param>
		/// <param name="configure"></param>
		/// <returns></returns>
		public static IServiceCollection AddStatisticsCenter(this IServiceCollection services,
			Action<StatisticsBuilder> configure)
		{
			services.AddSingleton<IHostedService, StatisticsCenter>();

			var spiderStatisticsBuilder = new StatisticsBuilder(services);
			configure?.Invoke(spiderStatisticsBuilder);

			return services;
		}
		/// <summary>
		/// ʹ���ڴ���
		/// </summary>
		/// <param name="builder"></param>
		/// <returns></returns>
		public static StatisticsBuilder UseMemory(this StatisticsBuilder builder)
		{
			Check.NotNull(builder, nameof(builder));
			builder.Services.AddSingleton<IStatisticsStore, MemoryStatisticsStore>();
			return builder;
		}
		/// <summary>
		/// ʹ��mysql
		/// </summary>
		/// <param name="builder"></param>
		/// <returns></returns>
		public static StatisticsBuilder UseMySql(this StatisticsBuilder builder)
		{
			Check.NotNull(builder, nameof(builder));
			builder.Services.AddSingleton<IStatisticsStore, MySqlStatisticsStore>();
			return builder;
		}
		/// <summary>
		/// ʹ��Redis
		/// </summary>
		/// <param name="builder"></param>
		/// <returns></returns>
		public static StatisticsBuilder UseRedis(this StatisticsBuilder builder)
		{
			Check.NotNull(builder, nameof(builder));
			
			builder.Services.AddSingleton<IStatisticsStore, RedisStatisticsStore>();
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