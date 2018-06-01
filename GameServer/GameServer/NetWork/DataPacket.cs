using System;
using Tools;

namespace GameServer
{
    public class DataPacket
    {
        public JSONObject Data
        {
            get { return data; }
            set { data = value; }
        }
        private JSONObject data;

        public DataPacket()
        {
            Data = new JSONObject();
        }

        public string Tojson()
        {
            return Data.ToString();
        }

        public void Add(long cmd, int error) 
        {
            Add((CMD)cmd, error);
        }
        public void Add(CMD cmd, int error)
        {
            Data.AddField("cmd", (int)cmd);
            Data.AddField("error", error);
        }
    }
}
