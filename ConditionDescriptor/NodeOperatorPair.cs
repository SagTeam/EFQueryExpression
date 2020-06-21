using System;
using System.Diagnostics;

namespace Sag.Data.Common.Query
{
    /// <summary>
    /// 条件表达式和操作符的配对
    /// </summary>
    /// <typeparam name="TOperator"></typeparam>
    /// <typeparam name="TNode"></typeparam>
    //[DebuggerTypeProxy(typeof(QueryExprOperatorPairDebugView<,>))]
    [DebuggerDisplay("NodeOperatorPair: op={Operator} ; {Node}")]
    //[Serializable]
   public  class NodeOperatorPair<TOperator, TNode> where TOperator : Enum where TNode : QueryNode//,IEquatable<TNode>
    {
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        internal uint hashCode;

        public TOperator Operator { get;  set; }   // Flag of entry

        public TNode Node { get;  set; }         // Value of entry

        public NodeOperatorPair() { }

        internal NodeOperatorPair(TOperator op,TNode value)
        {
            Operator = op;
            Node = value;
        }

        public override string ToString()
        {
            var sb = StringBuilderCache.GetInstance();
            sb.Append("{Operator:");

            if (Operator != null)
            {
                sb.Append(Operator);
            }
            sb.Append(",");
            sb.Append("Node:");

            if (Node != null)
            {
                sb.Append(Node);
            }

            sb.Append('}');

            return StringBuilderCache.GetString(sb);
        }
    }


}
