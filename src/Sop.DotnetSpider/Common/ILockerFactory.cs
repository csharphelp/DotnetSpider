namespace Sop.DotnetSpider.Common
{
	/// <summary>
	/// �ļ��������ӿ�
	/// </summary>
    public interface ILockerFactory
    {
        ILocker GetLocker();

        ILocker GetLocker(string locker);
    }
}