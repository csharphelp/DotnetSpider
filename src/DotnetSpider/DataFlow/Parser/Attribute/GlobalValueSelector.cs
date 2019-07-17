using System;

namespace Sop.Spider.DataFlow.Parser.Attribute
{
	[AttributeUsage(AttributeTargets.Class)]
	public class GlobalValueSelector : ValueSelector
	{
		/// <summary>
		/// 解析值的名称
		/// </summary>
		public string Name { get; set; }
	}
}