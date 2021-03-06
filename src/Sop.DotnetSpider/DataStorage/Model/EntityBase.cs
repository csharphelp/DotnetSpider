using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using Sop.DotnetSpider.Common;
using Sop.DotnetSpider.DataStorage;

namespace Sop.DotnetSpider
{
	/// <summary>
	/// 实体基类（解析实体）
	/// </summary>
	/// <typeparam name="T"></typeparam>
    public class EntityBase<T> where T : class, new()
    {
        private Lazy<TableMetadata> _tableMetadata;

        protected EntityBase()
        {
            if (GetType().FullName != typeof(T).FullName)
            {
                throw new SpiderException("实体类型继承定义不正确");
            }
        }

        /// <summary>
        /// 获取实体的表数据,创建表结构
        /// </summary>
        /// <returns></returns>
        public TableMetadata GetTableMetadata()
        {
            _tableMetadata = new Lazy<TableMetadata>();

            Configure();

            var type = GetType();
			//获取数据库表名称
            var schema = type.GetCustomAttributes(typeof(Schema), false).FirstOrDefault();
            if (schema != null)
            {
                _tableMetadata.Value.Schema = (Schema) schema;
                if (string.IsNullOrWhiteSpace(_tableMetadata.Value.Schema.Table))
                {
                    _tableMetadata.Value.Schema = new Schema(_tableMetadata.Value.Schema.Database, type.Name);
                }
            }
            else
            {
                _tableMetadata.Value.Schema = new Schema(null, type.Name);
            }
			//获取实体属性，创建数据表结构
            var properties = type.GetProperties().Where(x => x.CanRead && x.CanWrite).ToList();

            foreach (var property in properties)
            {
                var column = new Column
                {
                    PropertyInfo = property,
                    Name = property.Name,
                    Type = property.PropertyType.Name,
                    Required = property.GetCustomAttributes(typeof(RequiredAttribute), false).Any()
                };

                var stringLength =
                    (StringLengthAttribute) property.GetCustomAttributes(typeof(StringLengthAttribute), false)
                        .FirstOrDefault();
                if (stringLength != null)
                {
                    column.Length = stringLength.MaximumLength;
                }

                _tableMetadata.Value.Columns.Add(property.Name, column);
            }

            // 如果未设置主键, 但实体中有名为 Id 的属性, 则默认把 Id 作为主键
            if (_tableMetadata.Value.Primary == null || _tableMetadata.Value.Primary.Count == 0)
            {
                var primary = properties.FirstOrDefault(x => x.Name.ToLower() == "id");
                if (primary != null)
                {
                    _tableMetadata.Value.Primary = new HashSet<string> {primary.Name};
                }
            }

            _tableMetadata.Value.TypeName = type.FullName;

            // 如果有主键，但没有设置更新字段，则完全更新
            if (_tableMetadata.Value.Primary != null && _tableMetadata.Value.Primary.Count > 0 &&
                !_tableMetadata.Value.HasUpdateColumns)
            {
                var columns = _tableMetadata.Value.Columns.Select(x => x.Key).ToList();
                foreach (var primary in _tableMetadata.Value.Primary)
                {
                    columns.Remove(primary);
                }

                _tableMetadata.Value.Updates = new HashSet<string>(columns);
            }


            return _tableMetadata.Value;
        }
		/// <summary>
		/// 
		/// </summary>
        protected virtual void Configure()
        {
        }

        protected T HasKey(Expression<Func<T, object>> indexExpression)
        {
            Check.NotNull(indexExpression, nameof(indexExpression));
            var columns = GetColumns(indexExpression);
            if (columns == null || columns.Count == 0)
            {
                throw new SpiderException("主键不能为空");
            }

            _tableMetadata.Value.Primary = new HashSet<string>(columns);
            return this as T;
        }
		/// <summary>
		/// 索引
		/// </summary>
		/// <param name="indexExpression"></param>
		/// <param name="isUnique"></param>
		/// <returns></returns>
        protected T HasIndex(Expression<Func<T, object>> indexExpression, bool isUnique = false)
        {
            Check.NotNull(indexExpression, nameof(indexExpression));

            var columns = GetColumns(indexExpression);

            if (columns == null || columns.Count == 0)
            {
                throw new SpiderException("索引列不能为空");
            }

            _tableMetadata.Value.Indexes.Add(new IndexMetadata(columns.ToArray(), isUnique));
            return this as T;
        }
		/// <summary>
		/// 
		/// </summary>
		/// <param name="indexExpression"></param>
		/// <returns></returns>
        protected T ConfigureUpdateColumns(Expression<Func<T, object>> indexExpression)
        {
            Check.NotNull(indexExpression, nameof(indexExpression));
            var columns = GetColumns(indexExpression);
            _tableMetadata.Value.Updates = columns;
            return this as T;
        }

        private HashSet<string> GetColumns(Expression<Func<T, object>> indexExpression)
        {
            var nodeType = indexExpression.Body.NodeType;
            var columns = new HashSet<string>();
            switch (nodeType)
            {
                case ExpressionType.New:
                {
                    var body = (NewExpression) indexExpression.Body;
                    foreach (var argument in body.Arguments)
                    {
                        var memberExpression = (MemberExpression) argument;
                        columns.Add(memberExpression.Member.Name);
                    }

                    if (columns.Count != body.Arguments.Count)
                    {
                        throw new SpiderException("表达式不正确");
                    }

                    break;
                }
                case ExpressionType.MemberAccess:
                {
                    var memberExpression = (MemberExpression) indexExpression.Body;
                    columns.Add(memberExpression.Member.Name);
                    break;
                }
                case ExpressionType.Convert:
                {
                    UnaryExpression body = (UnaryExpression) indexExpression.Body;
                    var memberExpression = (MemberExpression) body.Operand;
                    columns.Add(memberExpression.Member.Name);
                    break;
                }
                default:
                {
                    throw new SpiderException("表达式不正确");
                }
            }

            return columns;
        }
    }
}