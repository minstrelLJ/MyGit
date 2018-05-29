using System;
using Tools;

namespace GameServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Init();

            string input = "";
            while (true)
            {
                input = Console.ReadLine();

                if (input == "exit")
                    break;
            }

            Console.ReadLine();
        }

        static void Init()
        {
            MyLog.Init(LogType.Text);
            NetWorkManager.Instance.Init();
        }
    }
}
