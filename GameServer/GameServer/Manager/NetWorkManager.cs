using System;
using Tools;
using MySocket;

namespace GameServer
{
    public class NetWorkManager : Singleton<NetWorkManager>
    {
        private static Server server;
        public void Init()
        {
            server = new Server(9999);
            server.Start();
            server.Receive += Receive;

            Console.ReadLine();
        }

        private void Receive(AsyncSocketUserToken userSocket, string msg)
        {
            for (int i = 0; i < server.ClientCount; i++)
            {
                if (server.ClientList[i].IPEndPoint == userSocket.IPEndPoint)
                    continue;

                server.ClientList[i].SendMessage(userSocket.IPEndPoint + ": " + msg);
                MyLog.Log(userSocket.IPEndPoint + ": " + msg);
            }
        }
    }
}
