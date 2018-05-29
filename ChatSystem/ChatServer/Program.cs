using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools;
using MySocket;

namespace ChatServer
{
    class Program
    {
        static Server server;
        static void Main(string[] args)
        {
            Init();

            server = new Server(9999);
            server.Start();
            server.Receive += Receive;

            Console.ReadLine();
        }

        private static void Receive(AsyncSocketUserToken userSocket, string msg)
        {
            for (int i = 0; i < server.ClientCount; i++)
            {
                if (server.ClientList[i].IPEndPoint == userSocket.IPEndPoint)
                    continue;

                server.ClientList[i].SendMessage(userSocket.IPEndPoint + ": " + msg);
            }
        }

        private static void Init()
        {
            MyLog.InitToConsole();
        }
    }
}
