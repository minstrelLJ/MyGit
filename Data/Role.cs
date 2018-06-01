using System;
using Tools;

namespace Data
{
    public class Role
    {
        public int roleId
        {
            get { return GetInt("roleId"); }
            set { data.SetField("roleId", value); }
        }
        public string roleName
        {
            get { return GetStr("roleName"); }
            set { data.SetField("roleName", value); }
        }
        public int level
        {
            get { return GetInt("level"); }
            set { data.SetField("level", value); }
        }
        public long exp
        {
            get { return GetLong("exp"); }
            set { data.SetField("exp", value); }
        }

        public float fixedSTR
        {
            get { return GetFloat("fixedSTR"); }
            set { data.SetField("fixedSTR", value); }
        }
        public float fixedDEX
        {
            get { return GetFloat("fixedDEX"); }
            set { data.SetField("fixedDEX", value); }
        }
        public float fixedMAG
        {
            get { return GetFloat("fixedMAG"); }
            set { data.SetField("fixedMAG", value); }
        }
        public float fixedCON
        {
            get { return GetFloat("fixedCON"); }
            set { data.SetField("fixedCON", value); }
        }

        public float potentialSTR
        {
            get { return GetFloat("potentialSTR"); }
            set { data.SetField("potentialSTR", value); }
        }
        public float potentialDEX
        {
            get { return GetFloat("potentialDEX"); }
            set { data.SetField("potentialDEX", value); }
        }
        public float potentialMAG
        {
            get { return GetFloat("potentialMAG"); }
            set { data.SetField("potentialMAG", value); }
        }
        public float potentialCON
        {
            get { return GetFloat("potentialCON"); }
            set { data.SetField("potentialCON", value); }
        }

        JSONObject data = new JSONObject();
        public Role() { }
        public Role(JSONObject data) { this.data = data; }

        public JSONObject ToJson()
        {
            JSONObject data = new JSONObject();

            data.AddField("roleId", roleId);
            data.AddField("roleName", roleName);
            data.AddField("level", level);
            data.AddField("exp", exp);

            data.AddField("fixedSTR", fixedSTR);
            data.AddField("fixedDEX", fixedDEX);
            data.AddField("fixedMAG", fixedMAG);
            data.AddField("fixedCON", fixedCON);
            data.AddField("potentialSTR", potentialSTR);
            data.AddField("potentialDEX", potentialDEX);
            data.AddField("potentialMAG", potentialMAG);
            data.AddField("potentialCON", potentialCON);

            return data;
        }

        private int GetInt(string key)
        {
            if (data != null && data.keys.Contains(key))
                return (int)data[key].i;
            return -1;
        }
        private long GetLong(string key)
        {
            if (data != null && data.keys.Contains(key))
                return data[key].i;
            return -1;
        }
        private float GetFloat(string key)
        {
            if (data != null && data.keys.Contains(key))
                return data[key].f;
            return -1;
        }
        private double GetDouble(string key)
        {
            if (data != null && data.keys.Contains(key))
                return data[key].n;
            return -1;
        }
        private string GetStr(string key)
        {
            if (data != null && data.keys.Contains(key))
                return data[key].str;
            return "-1";
        }
    }
}
