using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MySocket
{
    public class AsyncSocketUserTokenPool
    {
        private Stack<AsyncSocketUserToken> pool;

        public int Count { get { return pool.Count; } }
        public AsyncSocketUserTokenPool(int capacity)
        {
            pool = new Stack<AsyncSocketUserToken>(capacity);
        }
        public void Push(AsyncSocketUserToken item)
        {
            if (item == null)
            {
                throw new ArgumentException("Items added to a AsyncSocketUserToken cannot be null");
            }
            lock (pool)
            {
                pool.Push(item);
            }
        }
        public AsyncSocketUserToken Pop()
        {
            lock (pool)
            {
                return pool.Pop();
            }
        }
        
    }

    public class AsyncSocketUserTokenList : Object
    {
        private List<AsyncSocketUserToken> list;

        public AsyncSocketUserToken this[int index]
        {
            get
            {
                if (list.Count > index) return list[index];
                return null;
            }
            //set
            //{
            //    if (list.Count > index)
            //        list[index] = value;
            //}
        }

        public int Count { get { return list.Count; } }
        public AsyncSocketUserTokenList()
        {
            list = new List<AsyncSocketUserToken>();
        }
        public void Add(AsyncSocketUserToken userToken)
        {
            lock (list)
            {
                list.Add(userToken);
            }
        }
        public void Remove(AsyncSocketUserToken userToken)
        {
            lock (list)
            {
                list.Remove(userToken);
            }
        }
        public void CopyList(ref AsyncSocketUserToken[] array)
        {
            lock (list)
            {
                array = new AsyncSocketUserToken[list.Count];
                list.CopyTo(array);
            }
        }
    }
}
