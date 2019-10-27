using System;
using System.IO;

namespace Sop.Spider.Common
{
	/// <summary>
	/// 锁接口
	/// </summary>
	public interface ILocker : IDisposable
    {
		/// <summary>
		/// 信息
		/// </summary>
        string Information { get; }
		/// <summary>
		/// 
		/// </summary>
        string Locker { get; }
    }
	/// <summary>
	/// 文件锁
	/// </summary>
	public class FileLocker : ILocker
    {
        private FileStream _stream;

        public string Information { get; }

        public FileLocker(string path)
        {
            Check.NotNull(path, nameof(path));
            Locker = path;
            _stream = File.Create(path);
            Information = _stream.ReadAllText();
        }

        public void Dispose()
        {
            _stream?.WriteAllText(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            _stream?.Dispose();
            _stream = null;
        }

        public string Locker { get; }
    }
}