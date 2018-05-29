using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MySocket
{
    struct SendBufferPacket
    {
        public int Offset;
        public int Count;
    }

    //由于是异步发送，有可能接收到两个命令，写入了两次返回，发送需要等待上一次回调才发下一次的响应
    public class BufferSend : BufferBase
    {
        private List<SendBufferPacket> sendBufferList;
        private SendBufferPacket sendBufferPacket;

        public BufferSend(int bufferSize) : base (bufferSize)
        {
            sendBufferList = new List<SendBufferPacket>();
            sendBufferPacket.Offset = 0;
            sendBufferPacket.Count = 0;
        }

        public void StartPacket()
        {
            sendBufferPacket.Offset = DataCount;
            sendBufferPacket.Count = 0;
        }

        public void EndPacket()
        {
            sendBufferPacket.Count = DataCount - sendBufferPacket.Offset;
            sendBufferList.Add(sendBufferPacket);
        }

        public bool GetFirstPacket(ref int offset, ref int count)
        {
            if (sendBufferList.Count <= 0)
                return false;
            offset = 0;//m_sendBufferList[0].Offset;清除了第一个包后，后续的包往前移，因此Offset都为0
            count = sendBufferList[0].Count;
            return true;
        }

        public bool ClearFirstPacket()
        {
            if (sendBufferList.Count <= 0)
                return false;
            int count = sendBufferList[0].Count;
            Clear(count);
            sendBufferList.RemoveAt(0);
            return true;
        }

        public void ClearPacket()
        {
            sendBufferList.Clear();
            Clear(DataCount);
        }
    }
}
