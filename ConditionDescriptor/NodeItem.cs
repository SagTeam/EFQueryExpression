using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Sag.Data.Common.Query
{
    //[DefaultProperty("Value")]
    [DebuggerDisplay("Value = {Value};   Flag={Flag.ToString()}")]
    //[Serializable]
    public class NodeItem : QueryItem, IEquatable<NodeItem>
    {

        #region 属性

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        object _value = null;
        /// <summary>
        /// 字段名或者字段值
        /// </summary>
        public object Value { get=>_value; set { _value = value;InternalKey = value; } }

        /// <summary>
        /// 指定属性:<see cref="Value"/>指示为字段属性还是字段值
        /// </summary>
        public QueryValueFlag Flag { get; set; }

        #endregion 属性


        #region 构造

        public NodeItem( QueryValueFlag flag ,object value,Type typeAs=null):this(value,typeAs)
        {
            Flag = flag;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value">属性名(字段名)</param>
        /// <param name="typeAs">需要强制转换到的类型(默认=<see langword="null"/>,表示与右值的Type相同)</param>
        public NodeItem(object value, Type typeAs = null) : base(value)
        {
            _value = value;
            TypeAs = typeAs;
        }

        public NodeItem(object value) : base(value) { _value = value; }

        public NodeItem() : base() { }

        #endregion 构造


        public bool Equals([AllowNull] NodeItem other)
        {
            return base.Equals(other);
        }

        public override bool Equals([AllowNull] QueryNode other)
        {
            return base.Equals((NodeItem)other);
        }

    }



}
