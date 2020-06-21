using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
//using System.Diagnostics.CodeAnalysis;


namespace Sag.Data.Common.Query
{

    #region ItemBase

    [DebuggerDisplay("Value = {InternalKey.ToString()};     HashCode= {GetHashCode()}")]
   // [Serializable]
    public abstract class QueryItem : QueryNode, IEquatable<QueryItem>
    {

        #region 构造

        public QueryItem(string value) : this() { InternalKey = value; }

        public QueryItem(int value) : this() { InternalKey = value; }

        public QueryItem(object value) : this() { InternalKey = value; }

        public QueryItem() { initClass(); }
        #endregion //构造


        #region Propertys


        /// <summary>
        /// 属性名或查询值,作为在块集合中时的键值组成
        /// </summary>
        internal virtual object InternalKey { get; set; }

        #endregion //Propertys


        #region  methods

        /// <summary>
        /// 获取或设置相等比较器实例,不可为空值.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        QueryEqualComparer<QueryItem> _equalComparer;

        /// <summary>
        /// 获取相等比较器实例
        /// </summary>
        protected IEqualityComparer<QueryItem> GetEqualComparer() => _equalComparer;

        /// <summary>
        /// 设置相等比较器实例
        /// </summary>        
        protected void SetEqualComparer(QueryEqualComparer<QueryItem> value)
        {
                if (value == null) throw new ArgumentNullException(nameof(GetEqualComparer), MsgStrings.ValueCannotNull);
                _equalComparer = value as QueryEqualComparer<QueryItem>
                    ?? throw new InvalidCastException(string.Format(MsgStrings.InvalidTypeConvert, nameof(GetEqualComparer)));
            
        }



        #endregion  methods


        #region private methods

        private void initClass()
        {
            _equalComparer = new QueryEqualComparer<QueryItem>((x, y) =>
            {
                //if (y is QueryItem obj)
                //{
                if (ReferenceEquals(x, null) || ReferenceEquals(y, null)) return false; //任何一个为Null
                if (ReferenceEquals(x, y)) return true;  //是否同引用的相同实例

                return x.GetHashCode() == y.GetHashCode();
                //}
                //else return false;
            });
        }

        #endregion private methods


        #region 实现比较接口相关

        public override int GetHashCode()
        {
            if (int.TryParse(InternalKey.ToString(), out int v))
                return v.GetHashCode();
            if (InternalKey is ValueType value)
                return value.GetHashCode();
            return this.InternalKey.GetHashCode();
        }


        public override bool Equals(object obj)
        {
            if (!(obj is QueryItem o)) return false;
            return Equals(o);
        }

        public bool Equals([AllowNull]QueryItem other)
        {
            //if (EqualComparer == null)
            //    EqualComparer = new QueryEqualComparer<TItem>(obj =>
            //  {
            //      if (ReferenceEquals(this, obj)) return true;
            //      if (ReferenceEquals(obj, null)) return false;
            //      //if (string.Compare(this.ItemBody, obj.ItemBody, true) == 0) return true;
            //      return this.GetHashCode() == obj.GetHashCode();
            //      //return false;
            //  });
            return _equalComparer.Equals(this, other);

        }


        public static bool operator ==(QueryItem x, object y)
        {
            return x.Equals(y);
        }

        public static bool operator !=(QueryItem x, object y)
        {
            return !x.Equals(y);
        }



        #endregion //实现比较接口相关


    }

    #endregion ItemBase

}