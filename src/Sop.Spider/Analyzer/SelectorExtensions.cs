using System;

namespace Sop.Spider.Analyzer
{
	/// <summary>
	/// 选择器扩展
	/// </summary>
	public static class SelectorExtensions
    {
        /// <summary>
        /// 把 BaseSelector 转换成真正的查询器
        /// </summary>
        /// <param name="selector">BaseSelector</param>
        /// <returns>查询器</returns>
        public static ISelector ToSelector(this SelectAttribute selector)
        {
            if (selector != null)
            {
                string expression = selector.Expression;

                switch (selector.Type)
                {
                    case SelectorType.Css:
                    {
                        NotNullExpression(selector);
                        return Selectors.Css(expression);
                    }
                    case SelectorType.JsonPath:
                    {
                        NotNullExpression(selector);
                        return Selectors.JsonPath(expression);
                    }
                    case SelectorType.Regex:
                    {
                        NotNullExpression(selector);
                        if (string.IsNullOrEmpty(selector.Arguments))
                        {
                            return Selectors.Regex(expression);
                        }

                        if (int.TryParse(selector.Arguments, out var group))
                        {
                            return Selectors.Regex(expression, group);
                        }
                        throw new SpiderArgumentException($"Regex argument should be a number set to group: {selector}");
                    }
                    case SelectorType.XPath:
                    {
                        NotNullExpression(selector);
                        return Selectors.XPath(expression);
                    }
                    default:
                    {
                        throw new NotSupportedException($"{selector} unsupported");
                    }
                }
            }

            return null;
        }

        private static void NotNullExpression(SelectAttribute selector)
        {
            if (string.IsNullOrWhiteSpace(selector.Expression))
            {
                throw new SpiderArgumentException($"Expression of {selector} should not be null/empty");
            }
        }
    }
}