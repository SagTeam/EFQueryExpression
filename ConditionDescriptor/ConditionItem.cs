using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Sag.Data.Common.Query
{
    #region Class QueryConditionItem

    /// <summary>
    /// 一个查询条件子句:例如它表示这样的作用:field1=conditionalValue1.
    /// </summary>
    public class ConditionItem : QueryItem, IEquatable<ConditionItem>
    {
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        private NodeBlock _ValueBlock;
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        private NodeBlock _propertyBlock;

        #region 构造

        public ConditionItem (NodeBlock propertys,QueryComparison comparison,NodeBlock value,Type typeAs)
        {
            SetPropertyBlock(propertys);
            SetValueBlock(value);
            TypeAs = typeAs;
            Comparison = comparison;
        }

        public ConditionItem(NodeBlock propertys, QueryComparison comparison, NodeItem value,Type typeAs) : this()
        {
            SetPropertyBlock(propertys);
            SetValueBlock(value);
            TypeAs = typeAs ;
            Comparison = comparison;            
        }
        public ConditionItem(NodeBlock propertys, QueryComparison comparison, object value,Type typeAs) : this()
        {
            SetPropertyBlock(propertys);
            SetValueBlock(value);
            TypeAs = typeAs ;
            Comparison = comparison;            
        }
        public ConditionItem(NodeItem property,QueryComparison comparison,NodeBlock value,Type typeAs):this()
        {
            SetPropertyBlock( property);
            SetValueBlock(value);
            TypeAs = typeAs;
            Comparison = comparison;
        }
        public ConditionItem(NodeItem property,QueryComparison comparison,NodeItem value,Type typeAs):this()
        {
            SetPropertyBlock( property);
            SetValueBlock(value);
            TypeAs = typeAs;
            Comparison = comparison;
        }
        public ConditionItem(NodeItem property,QueryComparison comparison,object value,Type typeAs):this()
        {
            SetPropertyBlock(property);
            SetValueBlock(value);
            TypeAs = typeAs;
            Comparison = comparison;
        }
        /// <summary>
        /// 查询条件
        /// </summary>
        /// <param name="name">属性名(字段名)</param>
        /// <param name="comparison">比较符</param>
        /// <param name="value">查询条件值</param>
        /// <param name="typeAs">字段的强制数据类型. typeAs 默认=null,即与value 的 type 相同.</param>
        public ConditionItem(string name, QueryComparison comparison, NodeBlock value, Type typeAs) : this()
        {
            SetPropertyBlock(name,typeAs);
            SetValueBlock(value);
            TypeAs = typeAs;
            Comparison = comparison;
          
        }

        public ConditionItem(string name,QueryComparison comparison,NodeItem value,Type typeAs)
        {
            SetPropertyBlock(name, typeAs);
            SetValueBlock(value);
            TypeAs = typeAs;
            Comparison = comparison;
        }

        /// <summary>
        /// 查询条件
        /// </summary>
        /// <param name="name">属性名称</param>
        /// <param name="comparison">比较符</param>
        /// <param name="value">查询条件值</param>
        /// <param name="typeAs">强制转换到类型</param>
        public ConditionItem(string name, QueryComparison comparison, object value, Type typeAs) : this()
        {
            //
            SetPropertyBlock(name,typeAs);
            SetValueBlock(value);

            TypeAs = typeAs;
            Comparison = comparison;
          
        }
        /// <summary>
        /// 查询参数,令 typeAs 默认=null,即与value 的 type 相同.
        /// </summary>
        /// <param name="name">属性名(字段名)</param>
        /// <param name="comparsionStr">比较符</param>
        /// <param name="value">查询条件值</param>
        public ConditionItem(string name, string comparsionStr, NodeBlock value)
            : this(name, GetComparisonWithString(comparsionStr), value, null) { }
        public ConditionItem(string name, string comparsionStr, object value)
            : this(name, GetComparisonWithString(comparsionStr), value, null) { }

        public ConditionItem() { }

        #region set value block
        NodeBlock SetValueBlock(object value)
        {
            if(value is NodeBlock vb)
                _ValueBlock = new NodeBlock(QueryArithmetic.Add, InsertionBehavior.Duplicates, vb);
            else if(value is NodeItem vi)
                _ValueBlock = new NodeBlock(QueryArithmetic.Add, InsertionBehavior.Duplicates, vi);
            else if(value is IEnumerable<NodeBlock> vblst)
                _ValueBlock = new NodeBlock(QueryArithmetic.Add, InsertionBehavior.Duplicates, vblst.ToArray());
            else if (value is IEnumerable<NodeItem> iblst)
                _ValueBlock = new NodeBlock(QueryArithmetic.Add, InsertionBehavior.Duplicates, iblst.ToArray());
            else
            {
                _ValueBlock = new NodeBlock(QueryArithmetic.Add, InsertionBehavior.Duplicates, new NodeItem(value));
            }
            return _ValueBlock;
        }
        NodeBlock SetValueBlock(NodeBlock value)
        {
            _ValueBlock = new NodeBlock(QueryArithmetic.Add, InsertionBehavior.Duplicates, value);
            return _ValueBlock;
        }
        NodeBlock SetValueBlock(NodeItem value)
        {
            _ValueBlock = new NodeBlock(QueryArithmetic.Add, InsertionBehavior.Duplicates, value);
            return _ValueBlock;
        }

        #endregion set value block

        #region set property block
        void SetPropertyBlock(NodeBlock value)
        {
            _propertyBlock = new NodeBlock(QueryArithmetic.Add, InsertionBehavior.Duplicates, value);
        }

        void SetPropertyBlock(NodeItem value)
        {
            _propertyBlock = new NodeBlock(QueryArithmetic.Add, InsertionBehavior.Duplicates, value);
        }

        void SetPropertyBlock(string value, Type typeAs = null)
        {
            _propertyBlock = new NodeBlock(QueryArithmetic.Add, InsertionBehavior.Duplicates, new NodeItem( QueryValueFlag.Property, value,typeAs));
        }

        #endregion set property block

        #endregion //构造

        #region Propertys

        /// <summary>
        /// 左值:条件的属性(字段)组
        /// </summary>
        public NodeBlock PropertyBlock
        {
            get { return _propertyBlock; }
            set {
                _propertyBlock = value;
            }
        }

        /// <summary>
        /// 查询条件值
        /// </summary>
        public NodeBlock ValueBlock
        {
            get => _ValueBlock;
            set => _ValueBlock = value;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        Type _dataType = null;
        /// <summary>
        /// 条件进行比较所基于的数据类型,
        /// 如果未指定本属性,优先使用属性的强制类型,
        /// 可能返回null,属性和值都没有指定强制类型,(后续将自动识别类型).
        /// </summary>
        private new Type TypeAs
        {
            get
            {
                if (_dataType != null)
                    return _dataType;
                else
                if (PropertyBlock.TypeAs!=null)
                    return PropertyBlock.TypeAs;
                else if(ValueBlock.TypeAs!=null)
                    return ValueBlock.TypeAs;
                else
                    return null;
            }
            set => _dataType = value;
        }

        /// <summary>
        /// 比较操作符
        /// </summary>
        public QueryComparison Comparison { get; set; }

        //public string QueryComparisonString { get => Comparison.ToString(); set => Comparison = GetComparisonWithString(value); }

        #endregion //Propertys


        #region private func

        static private QueryComparison GetComparisonWithString(string comparStr)
        {
            return comparStr.ToLower().Replace("  ", " ").Replace("  ", " ") switch
            {
                "=" => QueryComparison.Equal,
                "<" => QueryComparison.Less,
                ">" => QueryComparison.Greater,
                "<=" => QueryComparison.LessEqual,
                ">=" => QueryComparison.GreaterEqual,
                "!=" => QueryComparison.NotEqual,
                "<>" => QueryComparison.NotEqual,
                "like" => QueryComparison.Contains,
                "notlile" => QueryComparison.NotContains,
                "!like" => QueryComparison.NotContains,
                "in" => QueryComparison.InList,
                "notin" => QueryComparison.NotInList,
                "!in" => QueryComparison.NotInList,
                "startwith" => QueryComparison.StartWith,
                "endwith" => QueryComparison.EndWith,
                "isnull" => QueryComparison.IsNullValue,
                _ => QueryComparison.Contains,
            };
        }

        #endregion //private func

        #region 实现比较接口相关

        public override int GetHashCode()
        {
            var h1 = _propertyBlock.GetHashCode();
            //var hs = HashCode.Combine(h1, Comparison, Value);
            //return hs;
            return 1;
        }


        public override bool Equals([AllowNull] QueryNode other)
        {
            return GetEqualComparer().Equals(other);
        }

        public bool Equals(ConditionItem other)
        {
            return GetEqualComparer().Equals(other);
        }

        //public static bool operator ==(QueryConditionItem left, QueryConditionItem right)
        //{
        //    return left.CompareTo(right) == 0;
        //}

        //public static bool operator !=(QueryConditionItem left, QueryConditionItem right)
        //{
        //    return left.CompareTo(right) != 0;
        //}

        #endregion //实现比较接口相关

    }
    #endregion  //end region Class QueryConditionItem

}
