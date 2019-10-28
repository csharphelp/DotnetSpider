using Microsoft.Extensions.Logging;
using Serilog.Core;
using Sop.DotnetSpider.Common;
using Sop.DotnetSpider.DataStorage;
using Sop.DotnetSpider.Download;
using Sop.DotnetSpider.EventBus;
using Sop.DotnetSpider.RequestSupplier;
using Sop.DotnetSpider.Scheduler;
using Sop.DotnetSpider.Statistics;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Sop.DotnetSpider
{
	/// <summary>
	/// Spider 实体属性类
	/// </summary>
	public partial class Spider
	{
		#region private
		private readonly List<Request> _requests = new List<Request>();

		private readonly List<IDataFlow> _dataFlows = new List<IDataFlow>();
		private readonly IEventBus _eventBus;
		private readonly ILogger _logger;
		private readonly IStatisticsService _statisticsService;
		private readonly List<IRequestSupplier> _requestSupplies = new List<IRequestSupplier>();
		private readonly List<Action<Request>> _configureRequestDelegates = new List<Action<Request>>();
		private readonly AtomicInteger _enqueued = new AtomicInteger(0);
		private readonly AtomicInteger _responded = new AtomicInteger(0);

		private readonly ConcurrentDictionary<string, Request> _enqueuedRequestDict =
			new ConcurrentDictionary<string, Request>();

		private DateTime _lastRequestedTime;
		private IScheduler _scheduler;
		private int _emptySleepTime = 30;
		private int _statisticsInterval = 5;
		private double _speed = 1;
		/// <summary>
		/// 速度控制器间隔
		/// </summary>
		private int _speedControllerInterval = 1000;

		public void SetDepth(int v)
		{
			throw new NotImplementedException();
		}

		private int _dequeueBatchCount = 1;
		private int _depth = int.MaxValue;
		private string _id;
		private bool _retryWhenResultIsEmpty;
		private bool _mmfSignal;

	
		#endregion


		/// <summary>
		/// 
		/// </summary>
		public string NewTaskId
		{
			get
			{
				CheckIfRunning();
				Id = Guid.NewGuid().ToString("N");
				return Id;
			}
		}


		/// <summary>
		/// 遍历深度
		/// </summary>
		/// <exception cref="ArgumentException"></exception>
		public int Depth
		{
			get => _depth;
			set
			{
				if (value <= 0)
				{
					value = 1;
					_logger?.LogError("【Spider】遍历深度必须大于 0");
				 
				}

				CheckIfRunning();
				_depth = value;
			}
		}

		/// <summary>
		/// 是否支持通过 MMF 操作爬虫
		/// </summary>
		public bool MmfSignal
		{
			get => _mmfSignal;
			set
			{
				CheckIfRunning();
				_mmfSignal = value;
			}
		}

		/// <summary>
		/// 如果结析结果为空, 重试。默认值为 否。
		/// </summary>
		public bool RetryWhenResultIsEmpty
		{
			get => _retryWhenResultIsEmpty;
			set
			{
				CheckIfRunning();
				_retryWhenResultIsEmpty = value;
			}
		}

		/// <summary>
		/// 爬虫运行状态
		/// </summary>
		public Status Status { get; private set; }

		/// <summary>
		/// 调度器
		/// </summary>
		/// <exception cref="SpiderException"></exception>
		public IScheduler Scheduler
		{
			get => _scheduler;
			set
			{
				CheckIfRunning();
				if (_scheduler != null && _scheduler.Total > 0)
				{ 
					_logger?.LogError("【Spider】当调度器不为空时，不能更换调度器");
				}

				_scheduler = value;
			}
		}

		/// <summary>
		/// 每秒尝试下载多少个请求
		/// </summary>
		public double Speed
		{
			get => _speed;
			set
			{
				if (value <= 0)
				{ 
					value = 1;
					_logger?.LogError("【Spider】每秒尝试下载多少个请求必须大于0");
				}
				CheckIfRunning();

				_speed = value;

				if (_speed >= 1)
				{
					_speedControllerInterval = 1000;
					_dequeueBatchCount = (int)_speed;
				}
				else
				{
					_speedControllerInterval = (int)(1 / _speed) * 1000;
					_dequeueBatchCount = 1;
				}

				var maybeEmptySleepTime = _speedControllerInterval / 1000;
				if (maybeEmptySleepTime >= EmptySleepTime)
				{
					var larger = (int)(maybeEmptySleepTime * 1.5);
					EmptySleepTime = larger > 30 ? larger : 30;
				}
			}
		}
		/// <summary>
		/// 
		/// </summary>
		public int EnqueueBatchCount { get; set; } = 1000;

		/// <summary>
		/// 上报状态的间隔时间，单位: 秒
		/// </summary>
		/// <exception cref="SpiderException"></exception>
		public int StatisticsInterval
		{
			get => _statisticsInterval;
			set
			{
				if (value < 5)
				{
					value = 6;
					_logger?.LogError("上报状态间隔必须大于 5 (秒)");
				}

				CheckIfRunning();
				_statisticsInterval = value;
			}
		}

		/// <summary>
		/// 任务的唯一标识
		/// </summary>
		public string Id
		{
			get => _id;
			set
			{
				if (string.IsNullOrWhiteSpace(value))
				{
					value = NewTaskId;
					_logger?.LogError("任务标识不能为空");					 
				}
				_logger?.LogInformation($"【Spider】任务标识ID长度:{value.Length}");
				CheckIfRunning();

				_id = value;
			}
		}

		/// <summary>
		/// 任务名称
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// 等待一定时间，如果队列中没有新的请求则认为任务结束
		/// </summary>
		/// <exception cref="SpiderException"></exception>
		public int EmptySleepTime
		{
			get
			{
				return _emptySleepTime;
			}

			set
			{
				if (value < _speedControllerInterval / 1000)
				{
					value = 1;
					_logger?.LogError("等待结束时间必需大于速度控制器间隔");
				}

				if (value < 0)
				{
					_logger?.LogError("等待结束时间必需大于 0 (秒)");
				}

				CheckIfRunning();
				_emptySleepTime = value;
			}
		}

		/// <summary>
		/// 当多少个下载请求未得到回应时，暂停任务
		/// TODO: 设置范围限制，不能小于 50
		/// </summary>
		public int NonRespondedLimitation { get; set; } = 100;

		/// <summary>
		/// 当一直得不到下载请求一段时间后，任务退出
		/// 单位: 秒
		/// TODO: 设置配置限制，不能小于 30
		/// </summary>
		public int NonRespondedTimeLimitation { get; set; } = 300;

		/// <summary>
		/// 多久没有收到回复认为是超时，尝试重试下载
		/// 单位秒
		/// </summary>
		public int RespondedTimeout { get; set; } = 15;

		/// <summary>
		/// 回应超时的重试次数，如果任一请求的重试次数超过则退出任务
		/// TODO: 设置配置限制，不能小于 20
		/// </summary>
		public int RespondedTimeoutRetryTimes { get; set; } = 20;
	}
}