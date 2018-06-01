using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Data;
using Tools;

namespace GameServer
{
    public class ConfigManager : Singleton<ConfigManager>
    {
        public static string CONFIG_PATH = System.IO.Directory.GetCurrentDirectory() + "/Configs/";

        public Dictionary<int, Role> roleDic = new Dictionary<int, Role>();
        public Dictionary<int, Monster> monsterDic = new Dictionary<int, Monster>();
        public Dictionary<int, LevelInfo> levelDic = new Dictionary<int, LevelInfo>();

        public void Init()
        {
            GetRoles();
            GetMonster();
            GetCheckPoint();
        }

        private void GetRoles()
        {
            var list = FileIO.ReadJson<Role>(CONFIG_PATH + "Role");
            foreach (var item in list)
            {
                roleDic[item.roleId] = item;
            }
        }
        private void GetMonster()
        {
            var list = FileIO.ReadJson<Monster>(CONFIG_PATH + "Monster");
            foreach (var item in list)
            {
                monsterDic[item.id] = item;
            }
        }
        private void GetCheckPoint()
        {
            var list = FileIO.ReadJson<LevelInfo>(CONFIG_PATH + "LevelInfo");
            foreach (var item in list)
            {
                levelDic[item.id] = item;
            }
        }

        public Role GetRole(int roleId)
        {
            Role ret;
            if (!roleDic.TryGetValue(roleId, out ret))
            {
                MyLog.Error("没有配置 Role " + roleId);
            }
            return ret;
        }
        public Monster GetMonster(int id)
        {
            Monster ret;
            if (!monsterDic.TryGetValue(id, out ret))
            {
                MyLog.Error("没有配置 Monster " + ret);
            }
            return ret;
        }
        public LevelInfo GetCheckPoint(int id)
        {
            LevelInfo ret;
            if (!levelDic.TryGetValue(id, out ret))
            {
                MyLog.Error("没有配置 Monster " + id);
            }
            return ret;
        }
    }
}
