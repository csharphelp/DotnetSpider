namespace Sop.Spider.Common
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