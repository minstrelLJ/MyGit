using System;
using System.Collections.Generic;

namespace Robot
{
    public class DataPacketPool
    {
         private Stack<DataPacket> pool;

        public int Count { get { return pool.Count; } }
        public DataPacketPool(int capacity)
        {
            pool = new Stack<DataPacket>(capacity);

            for (int i = 0; i < capacity; i++)
            {
                DataPacket dp = new DataPacket();
                pool.Push(dp);
            }
        }
        public void Push(DataPacket item)
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
        public DataPacket Pop()
        {
            lock (pool)
            {
                return pool.Pop();
            }
        }
    }
}
