using System;
using Tools;
using MySocket;

namespace GameServer
{
    public class NetWorkManager : Singleton<NetWorkManager>
    {
        private static Server listenerServer;
        public DataPacketPool dataPacketPool;

        public void Init()
        {
            dataPacketPool = new DataPacketPool(3000);

            listenerServer = new Server(9999);
            listenerServer.Start();
            listenerServer.Receive += ParseCMD.Parse;
        }
    }
}
