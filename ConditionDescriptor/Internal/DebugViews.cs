using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Sag.Data.Common.Query
{
    internal sealed class ICollectionDebugView<T>
    {
        private readonly ICollection<T> _collection;

        public ICollectionDebugView(ICollection<T> collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            _collection = collection;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public T[] Items
        {
            get
            {
                T[] items = new T[_collection.Count];
                _collection.CopyTo(items, 0);
                return items;
            }
        }
    }

    internal sealed class QueryExprInternalListDebugView<T>
    {
        private readonly InternalList<T> _collection;
        private T[]? _catchedList;
        public QueryExprInternalListDebugView(InternalList<T> collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            _collection = collection;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public T[] Items
        {
            get
            {
                return _catchedList ?? (_catchedList = _collection.ToArray(0, _collection.Count));
            }
        }
    }

    
    internal sealed class QueryPartsCollectionDebugView<Op, Expr> where Op : Enum where Expr:QueryNode//,IEquatable<Expr>
    {
        private readonly NodeCollection<Op, Expr> _collection;
        private NodeOperatorPair<Op, Expr>[]? _cachedItems;
        public QueryPartsCollectionDebugView(NodeCollection<Op, Expr> collection)
        {
            _collection = collection ?? throw new ArgumentNullException(nameof(collection));
        }

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public NodeOperatorPair<Op, Expr>[] Items
        {
            get
            {
                return _cachedItems ?? (_cachedItems = _collection.ToArray());

            }
        }

        
    }


    internal sealed class QueryExprOperatorPairDebugView<op, expr> where op : Enum where expr:QueryNode
    {
        private readonly NodeOperatorPair<op, expr> _pair;

        public QueryExprOperatorPairDebugView(NodeOperatorPair<op, expr> pair)
        {
            _pair = pair ?? throw new ArgumentNullException(nameof(pair));
        }

        public op Operator => _pair.Operator;

        public expr Expression => _pair.Node;

    }

    internal sealed class QueryBlockDebugView<op, item, block> 
        where op : Enum 
        where item : QueryItem,IEquatable<item>
        where block : QueryBlock<item, block, op>//,IEquatable<QueryBlock<item,block,op>>
    {
        private readonly QueryBlock<item, block, op> _queryBlock;
        private NodeOperatorPair<op,item>[] _cacheditems;
        private NodeOperatorPair<op, block>[]_cachedblocks;
        public QueryBlockDebugView(QueryBlock<item, block, op> queryBlock)
        {
            _queryBlock = queryBlock;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public NodeOperatorPair<op,item>[] Items
            => _cacheditems ?? (_cacheditems = _queryBlock.Items);

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public NodeOperatorPair<op, block>[] Blocks
            => _cachedblocks ?? (_cachedblocks = _queryBlock.Blocks);
    }
}
