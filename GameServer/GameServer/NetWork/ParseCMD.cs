using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySocket;
using Tools;
using Data;

namespace GameServer
{
    public class ParseCMD
    {
        public static void Parse(AsyncSocketUserToken client, string args)
        {
            MyLog.Log("<---{0}", args);

            JSONObject data = new JSONObject(args);
            CMD cmd = (CMD)data["cmd"].i;

            switch (cmd)
            {
                case CMD.Heartbeat: Heartbeat(client, data); break;
                case CMD.Login: Login(client, data); break;
                case CMD.Register: Register(client, data); break;
                //case CMD.GetRole: GetRole(client, data); break;
                case CMD.CreateRole: CreateRole(client, data); break;
                case CMD.EnterGame: EnterGame(client, data); break;
                //case CMD.SelectRole: SelectRole(client, data); break;
                case CMD.EnterBattleScene: EnterBattleScene(client, data); break;
                //case CMD.Attack: Attack(client, data); break;

                default: MyLog.Error("未知 CMD " + cmd); break;
            }
        }
        private static void Send(AsyncSocketUserToken client, DataPacket dp)
        {
            MyLog.Log("--->{0}", dp.Tojson());

            client.SendMessage(dp.Tojson());
            NetWorkManager.Instance.dataPacketPool.Push(dp);
        }

        // 心跳
        private static void Heartbeat(AsyncSocketUserToken client, JSONObject data)
        {
            DataPacket dp = NetWorkManager.Instance.dataPacketPool.Pop();
            dp.Add(data["cmd"].i, 0);
            Send(client, dp);
        }
        // 注册
        private static void Register(AsyncSocketUserToken client, JSONObject data)
        {
            string acc = data["acc"].str;
            string pw = data["pw"].str;

            DataPacket dp = NetWorkManager.Instance.dataPacketPool.Pop();
            int error = DataManager.Instance.CheckUser(acc);
            dp.Add(data["cmd"].i, error);
            Send(client, dp);
        }
        // 登录
        private static void Login(AsyncSocketUserToken client, JSONObject data)
        {
            string acc = data["acc"].str;
            string pw = data["pw"].str;

            DataPacket dp = NetWorkManager.Instance.dataPacketPool.Pop();
            int error = DataManager.Instance.CheckUserPassword(acc, pw);
            if (error == 0)
            {
                User user = DataManager.Instance.ReadUser(acc);
                dp.Data.AddField("userId", user.userId);

                Role role = DataManager.Instance.ReadRole(user.roleId);
                if (role != null) { dp.Data.AddField("roleInfo", role.ToJson()); }
            }
            dp.Add(data["cmd"].i, error);
            Send(client, dp);
        }
        // 创建角色
        private static void CreateRole(AsyncSocketUserToken client, JSONObject data)
        {
            int userId = (int)data["userId"].i;
            string roleName = data["roleName"].str;

            DataPacket dp = NetWorkManager.Instance.dataPacketPool.Pop();
            int error = 0;
            Role role = null;

            if (string.IsNullOrEmpty(roleName))
                error = 1004;

            if (error == 0 && DataManager.Instance.RoleIsExisting(roleName))
                error = 1003;

            if (error == 0)
            {
                role = ConfigManager.Instance.GetRole(1000);
                role.roleName = roleName;
                error = DataManager.Instance.AddNewRole(userId, role);
            }
            if (role != null) { dp.Data.AddField("roleInfo", role.ToJson()); }
            dp.Add(data["cmd"].i, error);
            Send(client, dp);
        }
        // 进入游戏
        private static void EnterGame(AsyncSocketUserToken client, JSONObject data)
        {
            int userId = (int)data["userId"].i;

            User user = DataManager.Instance.ReadUser(userId);
            int error = 0;
            DataPacket dp = NetWorkManager.Instance.dataPacketPool.Pop();
            dp.Add(data["cmd"].i, error);
            Send(client, dp);
        }
        // 进入战斗
        private static void EnterBattleScene(AsyncSocketUserToken client, JSONObject data)
        {
            int userId = (int)data["userId"].i;
            int levelId = (int)data["levelId"].i;
            User user = DataManager.Instance.ReadUser(userId);
            Entity role = DataManager.Instance.ReadRole(user.roleId);
            PlayerInfo player = new PlayerInfo();
            player.userId = userId;
            player.role = role;
            player.client = client;

            BattleServer bs = ServerManager.Instance.CreateBattleScene(levelId);
            bs.EnterPlayer(player);

            JSONObject db = DataPool.Instance.Pop(CMD.EnterBattleScene, 0);
            db.Add(bs.serverId);
            client.SendMessage(db);
        }
       
        //private static void GetRole(AsyncSocketUserToken client, JSONObject data)
        //{
        //    int userId = int.Parse(data.list[0]);
        //    User user = DataManager.Instance.ReadUser(userId);
        //    Role role = DataManager.Instance.ReadRole(user.roleId);

        //    JSONObject db;
        //    if (role != null)
        //    {
        //        db = DataPool.Instance.Pop(data.cmd, 0);
        //        db.Add(role.id);
        //        db.Add(role.name);
        //        db.Add(role.level);
        //        db.Add(role.exp);
        //        db.Add(role.fixedSTR);
        //        db.Add(role.fixedDEX);
        //        db.Add(role.fixedMAG);
        //        db.Add(role.fixedCON);
        //        db.Add(role.potentialSTR);
        //        db.Add(role.potentialDEX);
        //        db.Add(role.potentialMAG);
        //        db.Add(role.potentialCON);
        //        client.SendMessage(db);
        //    }
        //    else
        //    {
        //        db = DataPool.Instance.Pop(data.cmd, 1);
        //        client.SendMessage(db);
        //    }
        //}
        //private static void SelectRole(AsyncSocketUserToken client, JSONObject data)
        //{
        //    int userId = int.Parse(data.list[0]);
        //    User user = DataManager.Instance.ReadUser(userId);
        //    Entity role = DataManager.Instance.ReadRole(user.roleId);
        //    PlayerInfo player = new PlayerInfo();
        //    player.userId = userId;
        //    player.role = role;
        //    player.client = client;

        //    ServerManager.Instance.EnterNewPlayer(player);

        //    JSONObject db = DataPool.Instance.Pop(data.cmd, 0);
        //    client.SendMessage(db);
        //}
     
        //private static void Attack(AsyncSocketUserToken client, JSONObject data)
        //{
        //    int userId = int.Parse(data.list[0]);
        //    int serverId = int.Parse(data.list[1]);
        //    int entityId = int.Parse(data.list[2]);

        //    BattleServer bs = ServerManager.Instance.GetServer(serverId);
        //    if (bs != null)
        //    {
        //        bs.Attack(userId, entityId);
        //    }
        //}
    }
}
