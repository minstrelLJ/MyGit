using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace MySocket
{
    public class AsyncSocketUserToken : SocketBase
    {
        public Server serverSocket;

        public string IPEndPoint
        {
            get { return tcpClient.Client.RemoteEndPoint.ToString(); }
        }

        public virtual bool ProcessReceive(byte[] buffer, int offset, int count) //接收异步事件返回的数据，用于对数据进行缓存和分包
        {
            try
            {
                ActiveTime = DateTime.UtcNow;
                ReceiveBuffer.WriteBuffer(buffer, offset, count);

                int Len = BitConverter.ToInt32(buffer, offset); //取出信息长度
                offset += sizeof(int);

                // 收到的数据不全
                if (Len + sizeof(int) > count)
                    return true;

                string msg = Encoding.UTF8.GetString(buffer, offset, Len);
                ReceiveBuffer.Clear(Len + sizeof(int));
                serverSocket.Receive(this, msg);
                return true;
            }
            catch (Exception e)
            {
                Tools.MyLog.Error("Error: {0}\nStackTrace: {1}", e.Message, e.StackTrace);
                return false;
            }
        }

        public virtual bool SendCompleted()
        {
            return true;
        }
    }
}
