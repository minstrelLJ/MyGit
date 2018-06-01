using System;
using System.IO;
using System.Text;
using Data;

namespace Robot
{
    class Program
    {
        static void Main(string[] args)
        {
            NetManager.Instance.Init();
            NetManager.Instance.Connect();
            NetManager.Instance.SLogin();

            Console.ReadLine();
        }
    }
}
