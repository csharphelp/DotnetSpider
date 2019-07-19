﻿using Sample.samples;
using Serilog;
using Serilog.Events;
using System;
using System.Threading.Tasks;

namespace Sample
{
	class Program
	{
		
		static async Task Main(string[] args)
		{
			//项目log 设置默认debug模式，详细Serilog设置
			//请参考https://github.com/serilog/serilog
			var configure = new LoggerConfiguration()
				.MinimumLevel.Verbose()
				.MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
				.Enrich.FromLogContext()
				.WriteTo.Console()
				.WriteTo
				.RollingFile("dotnet-spider.log");
			Log.Logger = configure.CreateLogger();



		

			//案例1实体采集
			await CnblogsNewsSpider.Run();




			//await BaseUsage.Run();
			Console.Read();
		}
	}
}