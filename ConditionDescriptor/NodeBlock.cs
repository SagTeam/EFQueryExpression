using System.Diagnostics.CodeAnalysis;

namespace Sag.Data.Common.Query
{


    public class NodeBlock : QueryBlock<NodeItem, NodeBlock, QueryArithmetic>
    {

        #region 构造

        public NodeBlock() : base() { }

        public NodeBlock(QueryArithmetic op, InsertionBehavior behavior, params NodeBlock[] block) : base(op, behavior, block) { }

        public NodeBlock(string opName, InsertionBehavior behavior, params NodeBlock[] block) : base(opName, behavior, block) { }

        public NodeBlock(QueryArithmetic op, InsertionBehavior behavior, params NodeItem[] items) : base(op, behavior, items) { }

        public NodeBlock(string opName, InsertionBehavior behavior, params NodeItem[] items) : base(opName, behavior, items) { }


        #endregion //构造


        public override bool Equals([AllowNull] QueryNode other)
        {
            return base.Equals(other);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        //public override bool Equals(QueryLeftBlock other)
        //{
        //    return base.Equals(other);
        //}

 
 
    }

    

}
