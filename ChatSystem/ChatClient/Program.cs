using System;
using MySocket;
using Tools;
using System.Threading;

namespace ChatClient
{
    class Program
    {
        static void Main(string[] args)
        {
            MyLog.Init(LogType.Text);
            Client Socket = new Client();
            Socket.Connect("127.0.0.1", 9999);
            Socket.Receive = (data) =>
            {
                Console.WriteLine(data);
            };

            if (!Socket.IsConnected)
            {
                MyLog.Error("服务器连接失败...");
                Console.ReadLine();
            }
            else
            {
                string msg = "";
                while (true)
                {
                    msg = Console.ReadLine();
                    if (msg == "exit")
                        break;

                    Socket.SendMessage(msg);
                }
            }
        }
    }
}
