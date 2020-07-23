using System;
using System.Collections;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.Json.Serialization;
using Sag.Data.Common.Query.Internal;

namespace Sag.Data.Common.Query
{
    /// <summary>
    /// 表示属性名或者属性值
    /// </summary>
    [DebuggerDisplay("NodeItem: Flag[{Flag}]; Value[{Value}]; TypeAs[{TypeAs?.ToString()}]")]
    [JsonConverter(typeof(NodeItemJsonConverter))]
    public class NodeItem : QueryItem //, IEquatable<NodeItem>
    {

        #region 属性

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        object _value = null;
        int _keyVersion = 0;
        /// <summary>
        /// 字段名或者字段值
        /// </summary>
        public object Value 
        { 
            get=>_value;
            set {
                InnerKey = value;
                _value = value;
                _keyVersion++;
                if (value != null && TypeAs==null && Flag== QueryValueFlag.Value) TypeAs =value.GetType() ;
            } 
        }

        /// <summary>
        /// 指定属性:<see cref="Value"/>指示为字段属性还是字段值
        /// </summary>
        public QueryValueFlag Flag { get; set; }

        /// <summary>
        /// 属性名或查询值,作为在块集合中时的键值组成
        /// </summary>
        internal virtual object InnerKey { get; set; }

        #endregion 属性


        #region 构造

        /// <summary>
        /// 
        /// </summary>
        /// <param name="flag">指示<see cref="value"/>是属性名称或者是属性值</param>
        /// <param name="value">属性名(字段名)或属性值</param>
        /// <param name="typeAs">需要强制转换到的类型(默认=<see langword="null"/>,表示与右值的Type相同)</param>
        public NodeItem( QueryValueFlag flag ,object value,Type typeAs):base()
        {
            if (flag == QueryValueFlag.Property && value ==null)
                throw new Exception(MsgStrings.PropertyNameCanotNull);
            InnerKey = value;
            Flag = flag;
            _value = value;
            TypeAs =flag== QueryValueFlag.Value? typeAs ?? value?.GetType():null;
        }


        public NodeItem(QueryValueFlag flag, object value) : this(flag, value, null) { }


        /// <summary>
        /// 初始化为节点值的项
        /// </summary>
        /// <param name="value">节点值</param>
        public NodeItem(object value) : this(QueryValueFlag.Value, value, null) { }


        public NodeItem() : base() { }

        #endregion 构造

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        int _lastKeyVersion=-1;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        int _hashCodeCache;
        public override int GetHashCode()
        {
            //return InternalKey.GetHashCode();
            if (InnerKey == null) return 0;
            if (_lastKeyVersion == _keyVersion)  return _hashCodeCache;

            
            if (InnerKey is string || InnerKey is ValueType)
            {
                _lastKeyVersion = _keyVersion;
                _hashCodeCache= HashCode.Combine(InnerKey.GetHashCode(), Flag);
                return _hashCodeCache;
            }            
            while (true)
            {
                redoGetHashCoce:
                _lastKeyVersion = _keyVersion;
                int hc = 0;
                //for each与直接使用index比, 性能差别大而不可忽略,因此分开为IList 与IEnumerable
                if (InnerKey is IList lst)
                {
                    var ct = lst.Count;
                    for (int i = 0; i < ct; i++)
                    {
                        if (_lastKeyVersion == _keyVersion)
                            hc = HashCode.Combine(hc, lst[i]);
                        else
                            goto redoGetHashCoce;
                    }
                    _hashCodeCache =HashCode.Combine(hc, Flag); 
                    return _hashCodeCache;
                }
                if (InnerKey is IEnumerable emb)
                {
                    foreach (var m in emb)
                    {
                        if (_lastKeyVersion == _keyVersion)
                            hc = HashCode.Combine(hc, emb);
                        else
                            goto redoGetHashCoce; 
                    }
                     _hashCodeCache = HashCode.Combine(hc, Flag); 
                    return _hashCodeCache;
                }
                _hashCodeCache = HashCode.Combine(InnerKey.GetHashCode(), Flag); 
                return _hashCodeCache;
            }            
        }

 

    }



}
