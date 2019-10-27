using System;
using System.IO;
using System.Threading;

namespace Sop.DotnetSpider.Common
{
	/// <summary>
	/// �ļ���
	/// </summary>
    public class FileLockerFactory : ILockerFactory
    {
        private readonly string _folder;

        public FileLockerFactory()
        {
            _folder = Path.Combine(Framework.GlobalDirectory, "sessions");
            if (!Directory.Exists(_folder))
            {
                Directory.CreateDirectory(_folder);
            }
        }
		/// <summary>
		/// ��ȡ�ļ������ļ�����ʹ�ðɣ���
		/// </summary>
		/// <returns></returns>
        ILocker ILockerFactory.GetLocker()
        {
            var path = Path.Combine(_folder, $"{Guid.NewGuid():N}.lock");
            return new FileLocker(path);
        }
		/// <summary>
		///��ȡ�ļ�������
		/// </summary>
		/// <param name="locker"></param>
		/// <returns></returns>
        ILocker ILockerFactory.GetLocker(string locker)
        {
            var path = Path.Combine(Framework.GlobalDirectory, $"{locker}.lock");

            while (true)
            {
                try
                {
                    return new FileLocker(path);
                }
                catch
                {
                    Thread.Sleep(100);
                }
            }
        }
    }
}