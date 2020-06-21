using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
//using System.Diagnostics.CodeAnalysis;


namespace Sag.Data.Common.Query
{


    #region blockbase

    //[DebuggerTypeProxy(typeof(QueryBlockDebugView<,,>))]
    [DebuggerDisplay("ItemCount={ItemCount},  BlockCount={BlockCount}")]
   // [Serializable]
    public abstract class QueryBlock<TItem, TBlock, TOpEnum> : QueryNode, IEquatable<QueryBlock<TItem, TBlock, TOpEnum>>
        where TItem : QueryNode, IEquatable<TItem>
        where TBlock : QueryBlock<TItem, TBlock, TOpEnum>//,IEquatable<QueryBlock<TItem,TBlock,TOpEnum>>
        where TOpEnum : Enum
    {
        #region 变量
        
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private NodeCollection<TOpEnum, TItem> m_items;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private NodeCollection<TOpEnum, TBlock> m_blocks;


        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private long _blockVersion = 0;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private long _itemVersion = 0;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private long _hashCodeVersion = 0;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private long _hashCodeOldVersion = 0;

        private int _hashCode = 0;

        const InsertionBehavior defaultItemInsertBehavior = InsertionBehavior.Duplicates;
        const InsertionBehavior defaultBlockInsertBehavvior = InsertionBehavior.IgnoreExists;

        #endregion //变量

        #region 构造

        /// <summary>
        /// 传入一个查询块,构造实例
        /// </summary>
        /// <param name="op">逻辑连接符,与块内其它子块或项之间逻辑关系</param>
        /// <param name="behavior">是否阻止添加具有相同逻辑连接符的重复的块</param>
        /// <param name="blocks">包含要加入的块的数组</param>
        public QueryBlock(TOpEnum op, InsertionBehavior behavior, params TBlock[] blocks) : this()
        {
            AddBlock(op, behavior, blocks);
        }

        /// <summary>
        /// 传入一个查询块数组,构造实例
        /// </summary>
        /// <param name="op">逻辑连接符,指示与块与块之间,项与项之间,项与块之间的逻辑关系</param>
        /// <param name="behavior">指示是否重复的行为</param>
        /// <param name="itemArray">包含要添加的块的数组</param>
        public QueryBlock(string opName, InsertionBehavior behavior, params TBlock[] blocks) : this()
        {
            AddBlock(opName, behavior, blocks);

        }

        /// <summary>
        /// 传入一个查询项数组,构造实例
        /// </summary>
        /// <param name="op">逻辑连接符,指示与同一块内其它项或子块之间的逻辑关系</param>
        /// <param name="behavior">指示条件之间是否重复的行为</param>
        /// <param name="itemArray">包含要添加的项的数组</param>
        public QueryBlock(TOpEnum op, InsertionBehavior behavior, params TItem[] itemArray) : this()
        {
            AddItem(op, behavior, itemArray);
        }

        public QueryBlock(string opName, InsertionBehavior behavior, params TItem[] itemArray)
        {
            AddItem(opName, behavior, itemArray);
        }

        /// <summary>
        /// 查询块构造函数
        /// </summary>
        public QueryBlock()
        {
            ClassInitialize();

        }

        #endregion 构造


        #region Add

        public int[] Add(TOpEnum op, InsertionBehavior behavior, params TItem[] items)
        {
            return AddItem(op, behavior, items);
        }

        public int[] Add(string opName, InsertionBehavior behavior, params TItem[] items)
        {
            return AddItem(opName, behavior, items);
        }

        public int[] Add(TOpEnum op, InsertionBehavior behavior, params TBlock[] blocks)
        {
            return AddBlock(op, behavior, blocks);
        }

        public int[] Add(string opName, InsertionBehavior behavior, params TBlock[] blocks)
        {
            return AddBlock(opName, behavior, blocks);
        }

        #endregion Add

        #region AddItem

        /// <summary>
        /// 增加查询项.
        /// </summary>
        /// <param name="item">要添加的项对象</param>
        /// <param name="behavior">是否可重复增加项(包括操作符)</param>
        /// <param name="op">逻辑连接操作符,与父组内其它条件之间的逻辑关系</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        private bool InternalAddItem(TItem item, TOpEnum op, InsertionBehavior behavior)
        {
            if (item == null) return false;

            _itemVersion++;
            _hashCodeVersion++;
            return m_items.Add(op, item, behavior);
        }

        public int[] AddItem(TOpEnum op, InsertionBehavior behavior, params TItem[] itemArray)
        {
            var list = new List<int>();
            var id = 0;
            foreach (TItem p in itemArray)
            {
                if (InternalAddItem(p, op, behavior))
                    list.Add(id);
                id++;
            }
            return list.ToArray();
        }

        public int[] AddItem(string opName, InsertionBehavior behavior, params TItem[] itemArray)
        {
            if (GetOperatorWithString == null)
                throw new Exception(string.Format(MsgStrings.NullGetOperatorWithStringDelegate, nameof(GetOperatorWithString)));

            var op = GetOperatorWithString(opName);
            return AddItem(op, behavior, itemArray);
        }

        #endregion AddItem

        //[MethodImpl(MethodImplOptions.Synchronized)]
        //private void Reduce()
        //{
        //    if(ItemCount==0 && BlockCount==1)
        //    {
        //        _blockVersion++;
        //        _itemVersion++;
        //        _hashCodeVersion++;
        //     
        //            var bk = m_blocks[0].Value;
        //            m_blocks.Clear();
        //            foreach(var im in bk.GetItems())
        //                AddItem(im.Key, false, im.Value);
        //            foreach (var ib in bk.GetBlocks()) 
        //            {
        //               // ib.Value.Reduce();
        //                AddBlock(ib.Key, false, ib.Value);
        //            }
        //     
        //    }

        //}

        #region AddBlock

        /// <summary>
        /// 增加块
        /// </summary>
        /// <param name="block">要添加的块</param>
        /// <param name="op">操作接符,与父组内其它块或项之间的操作符,</param>
        /// <remarks></remarks>
        [MethodImpl(MethodImplOptions.Synchronized)]
        private bool InternalAddBlock(TBlock block, TOpEnum op, InsertionBehavior behavior)
        {
            if (block == null)
                return false;

            var blockType = typeof(TBlock);
            var blockAllCount = blockType.GetProperty(nameof(AllCount))?.GetValue(block) ?? 0;
            if ((int)blockAllCount == 0)
                return false;

            _blockVersion++;
            _hashCodeVersion++;
            return m_blocks.Add(op, block, behavior);

        }

        public int[] AddBlock(TOpEnum op, InsertionBehavior behavior, params TBlock[] blocks)
        {
            var list = new List<int>();
            var id = 0;
            foreach (TBlock p in blocks)
            {

                if (InternalAddBlock(p, op, behavior))
                    list.Add(id);
                id++;
            }
            return list.ToArray();
        }

        public int[] AddBlock(string opName, InsertionBehavior behavior, params TBlock[] blocks)
        {
            if (GetOperatorWithString == null)
                throw new Exception(string.Format(MsgStrings.NullGetOperatorWithStringDelegate, nameof(GetOperatorWithString)));

            var op = GetOperatorWithString.Invoke(opName);
            return AddBlock(op, behavior, blocks);
        }

        #endregion AddParamBlock

        #region Remove,Clear

        /// <summary>
        /// 移除指定的键位的项
        /// </summary>
        /// <param name="index"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public bool RemoveItemAt(int index)
        {
            //typeof(TItem).GetProperty(nameof(ParentBlock)).SetValue(m_Items[index], null);
            _itemVersion++;
            _hashCodeVersion++;
            return m_items.RemoveAt(index);

        }

        /// <summary>
        /// 移除所有指定的独立条件
        /// </summary>
        /// <param name="param"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public int RemoveItem(TItem item)
        {
            try
            {
                var ids = 0;
                for (var i = 0; i < m_items.Count; i++)
                {
                    var m = m_items[i];
                    if (m.Node == item)
                    {
                        var rdc = m_items.Remove(m.Operator, m.Node, false);
                        i--;
                        ids += rdc;
                    }

                }
                return ids;
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }


        [MethodImpl(MethodImplOptions.Synchronized)]
        public int RemoveItem(TItem item, TOpEnum op, bool byReference)
        {
            try
            {
                return m_items.Remove(op, item, byReference);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 移除指定键的条件组
        /// </summary>
        /// <param name="index"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public bool RemoveBlockAt(int index)
        {
            try
            {
                _blockVersion++;
                _hashCodeVersion++;
                return m_blocks.RemoveAt(index);

            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        /// <summary>
        /// 移除指定的条件组
        /// </summary>
        /// <param name="block"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public int RemoveBlock(TBlock block)
        {
            try
            {
                var ids = 0;
                for (var i = 0; i < m_blocks.Count; i++)
                {
                    var m = m_blocks[i];
                    if (m.Node == block)
                    {
                        var rdc = m_blocks.Remove(m.Operator, m.Node, false);
                        i--;
                        ids += rdc;
                    }

                }
                return ids;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public int RemoveBlock(TBlock block, TOpEnum op, bool byReference)
        {
            try
            {
                return m_blocks.Remove(op, block, byReference);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 单独清除内含条件组
        /// </summary>
        public void ClearBlocks()
        {
            _blockVersion++;
            _hashCodeVersion++;
            m_blocks.Clear();
            _blockVersion = 0;

        }

        /// <summary>
        /// 清除独立条件
        /// </summary>
        public void ClearItems()
        {
            _itemVersion++;
            _hashCodeVersion++;
            m_items.Clear();
            _itemVersion = 0;
        }

        /// <summary>
        /// 清除所有条件
        /// </summary>
        public void ClearAll()
        {
            ClearItems();
            ClearBlocks();
            _hashCodeVersion = 0;
        }

        #endregion Remove ,Clear

        #region Get:Item,Block,Operater


        /// <summary>
        /// 返回块内的项与操作的配对
        /// </summary>
        /// <param name="item">index 索引位置(从0计起)</param>
        public NodeOperatorPair<TOpEnum, TItem>[] GetItems()
        {
            return m_items.ToArray();
        }

        public NodeOperatorPair<TOpEnum, TItem> GetItemPair(int index)
        {
            return m_items[index];
        }

        public NodeOperatorPair<TOpEnum, TBlock>[] GetBlocks()
        {
            return m_blocks.ToArray();
        }

        /// <summary>
        /// 返回一个子块对象
        /// </summary>
        /// <param name="index">index 索引位置(从0计起)</param>
        public NodeOperatorPair<TOpEnum, TBlock> GetBlockPair(int index)
        {
            return m_blocks[index];
        }


        #endregion Get:Item .....

        #region Contains



        /// <summary>
        /// 检测块是否包含指定项,连同对应的操作符,使用项的相等比较(==).
        /// </summary>
        /// <param name="item"></param>
        /// <param name="op"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public bool Contains(TItem item, TOpEnum op)
        {
            return m_items.Contains(op, item, false);

        }


        /// <summary>
        /// 检测块是否包含指定块,连同对应的操作符,使用块的相等比较(==).
        /// </summary>
        /// <param name="block"></param>
        /// <param name="op"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public bool Contains(TBlock block, TOpEnum op)
        {
            return m_blocks.Contains(op, block, false);

        }

        #endregion //contains

        #region Propertys


        /// <summary>
        /// 默认为根据枚举名称获取枚举值,可以另指定此委托.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private protected Func<string, TOpEnum> GetOperatorWithString { get; set; } = (n) =>
        {
            var r = Enum.TryParse(typeof(TOpEnum), n, true, out var op) ? op : default(QueryArithmetic);
            return (TOpEnum)r;
        };


        /// <summary>
        /// 父块对象,可以用来判断此块是否为其它块的子块.
        /// </summary>
        //public TBlock ParentBlock { get; set; }

        /// <summary>
        /// 块内项的计数
        /// </summary>
        public int ItemCount => m_items.Count;

        /// <summary>
        /// 块内子块的计数
        /// </summary>
        public int BlockCount => m_blocks.Count;

        /// <summary>
        /// 返回块内的项和子块的计数(非递归计数)
        /// </summary>
        public int AllCount => m_items.Count + m_blocks.Count;

 
        /// <summary>
        /// 是否自动简化,如果是,则当空块或只包含一个子块且没有独立项的块在被添加时,会被抛弃,而用其内含子块取代之.
        /// </summary>
        public bool AutoReduce { get; set; } = true;

        //[DebuggerBrowsable(DebuggerBrowsableState.Collapsed)]
        public NodeOperatorPair<TOpEnum, TItem>[] Items { get => GetItems(); set => m_items.Items = value; }
           
        //[DebuggerBrowsable(DebuggerBrowsableState.Collapsed)]
        public NodeOperatorPair<TOpEnum, TBlock>[] Blocks { get => GetBlocks(); set => m_blocks.Items = value; }
           
       #endregion Propertys   
     
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        QueryEqualComparer<TBlock> _equalComparer;

        /// <summary>
        /// 获取相等比较器实例
        /// </summary>
        /// <returns></returns>
        public QueryEqualComparer<TBlock> GetEqualComparer() => _equalComparer;

        /// <summary>
        /// 设置相等比较器实例
        /// </summary>        
        public void SetEqualComparer(QueryEqualComparer<TBlock> value)
        {
 
                if (value == null) throw new ArgumentNullException(nameof(SetEqualComparer), MsgStrings.ValueCannotNull);
                _equalComparer = value as QueryEqualComparer<TBlock>
                    ?? throw new InvalidCastException(string.Format(MsgStrings.InvalidTypeConvert, nameof(SetEqualComparer)));
            
        }
   

        #region private methods

        private void ClassInitialize()
        {
            m_items = new NodeCollection<TOpEnum, TItem>();
            m_blocks = new NodeCollection<TOpEnum, TBlock>();
            _equalComparer = new QueryEqualComparer<TBlock>((x, y) =>
            {
                if (y is TBlock obj)
                {
                    if (ReferenceEquals(this, obj)) return true;
                    if (ReferenceEquals(obj, null)) return false;
                    if (ItemCount != obj.ItemCount || BlockCount != obj.BlockCount) return false;
                    return this.GetHashCode() == obj.GetHashCode();
                }
                else return false;

            });

        }


        #endregion private methods


        public override string ToString()
        {
            return "ItemCount=" + ItemCount.ToString() + "BlockCount=" + BlockCount.ToString();
        }

        #region 实现接口相关

        [MethodImpl(MethodImplOptions.Synchronized)]
        public override int GetHashCode()
        {
            var v = _hashCodeVersion;
            if (v == _hashCodeOldVersion) return _hashCode;
            var redo = true;
            int hcode = 0;
            while (redo)//防止其它线程中途的修改
            {
                v = _hashCodeVersion;
                for (int i = 0; i < this.ItemCount; i++)
                {
                    if (v == _hashCodeVersion)
                    {
                        var itm = m_items[i];
                        hcode = HashCode.Combine(itm.Node.GetHashCode(), itm.Operator, hcode);
                    }
                    else
                        break;
                }
                for (int i = 0; i < this.BlockCount; i++)
                {
                    if (v == _hashCodeVersion)
                    {
                        var bk = m_blocks[i];
                        hcode = HashCode.Combine(bk.Node.GetHashCode(), bk.Operator, hcode);
                    }
                    else
                        break;
                }
                if (v == _hashCodeVersion)
                {
                    _hashCode = hcode;
                    _hashCodeOldVersion = _hashCodeVersion;
                    redo = false;
                }
            }
            return hcode;

        }

        public static bool operator ==(QueryBlock<TItem, TBlock, TOpEnum> x, object y)
        {
            return x.Equals(y);
        }

        public static bool operator !=(QueryBlock<TItem, TBlock, TOpEnum> x, object y)
        {
            return !x.Equals(y);
        }

        public override bool Equals(object other)
        {
            if (!(other is QueryBlock<TItem, TBlock, TOpEnum> obj)) return false;
            return Equals(obj);
        }

        public virtual bool Equals([AllowNull] QueryBlock<TItem, TBlock, TOpEnum> other)
        {
            return _equalComparer.Equals(this, other);
        }

        public override bool Equals([AllowNull] QueryNode other)
        {
            return Equals((QueryBlock<TItem, TBlock, TOpEnum>)other);
        }


        #endregion impliments interfaces

        //private void ClassTerminate()
        //{

        //}

        //~QueryParamBlock()
        //{
        //    ClassTerminate();
        //    //base.Finalize();
        //}
    }

    #endregion blockbase

}