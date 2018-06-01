using System;
using Tools;

namespace Robot
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

        public string ToJson()
        {
            return Data.ToString();
        }

        public void Reset(long cmd) 
        {
            Reset((CMD)cmd);
        }
        public void Reset(CMD cmd)
        {
            Data.Clear();
            Data.AddField("cmd", (int)cmd);
        }
    }
}
