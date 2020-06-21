using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Sag.Data.Common.Query.Internal;
//using System.Diagnostics.CodeAnalysis;


namespace Sag.Data.Common.Query
{

    #region QueryNode

    // [Serializable]
    public abstract class QueryNode : IEquatable<QueryNode>
    {

        /// <summary>
        /// 获取或设置要强制类型转换到的指定类型,默认值null,表示与其对应的值类型相同
        /// </summary>
        public Type TypeAs { get; set; } = null;

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        internal JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions();
        public QueryNode()
        {
            _jsonSerializerOptions.Converters.Add(new TypeToJsonConverter());
            _jsonSerializerOptions.IgnoreReadOnlyProperties=true;
            _jsonSerializerOptions.IgnoreNullValues = true;
        }

        public virtual string ToJson()
        {
            return JsonSerializer.Serialize<object>(this, _jsonSerializerOptions);
        }

        public abstract bool Equals([AllowNull] QueryNode other);

        public abstract override int GetHashCode();

    }

    #endregion QueryNode

}