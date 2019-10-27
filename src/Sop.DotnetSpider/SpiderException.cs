using System;

namespace Sop.Spider
{
	/// <summary>
	/// SpiderFormatException�쳣
	/// </summary>
	public class SpiderFormatException : FormatException
	{
		public SpiderFormatException(string msg) : base(msg)
		{
		}

	}


	/// <summary>
	/// 
	/// </summary>
	public class SpiderArgumentException : ArgumentException
	{
		public SpiderArgumentException(string msg) : base(msg)
		{
		}
		public SpiderArgumentException(string msg, string ad) : base(msg, ad)
		{
		}

	}
	/// <summary>
	/// ��ʾӦ�ó���ִ���ڼ䷢���Ĵ���
	/// </summary>
	public class SpiderException : Exception
	{
		public SpiderException(string msg) : base(msg)
		{
		}

	}
	/// <summary>
	/// ��֧�ֵ��õķ���ʱ�����߳��Զ�ȡ��������д�벻֧�ֵ��õĹ��ܵ���ʱ�������쳣
	/// </summary>
	public class SpiderNotSupportedException : NotSupportedException
	{
		public SpiderNotSupportedException(string msg) : base(msg)
		{
		}

	}
	public class SpiderInvalidOperationException : InvalidOperationException
	{
		public SpiderInvalidOperationException(string msg) : base(msg)
		{
		}

	}



}