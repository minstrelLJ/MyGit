using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Data;
using MySocket;

namespace GameServer
{
    public class PlayerInfo
    {
        public int userId { get; set; }
        public Entity role { get; set; }
        public AsyncSocketUserToken client;
    }
}
