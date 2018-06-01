using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySocket;
using Tools;
using Data;

namespace Robot
{
    public class NetManager : Singleton<NetManager>
    {
        Client Socket;
        DataPacketPool dataPacketPool;
        BattleManager bm;

        public void Init()
        {
            dataPacketPool = new DataPacketPool(20);
            bm = new BattleManager();
        }

        public void Connect()
        {
            Socket = new Client();
            Socket.Connect("127.0.0.1", 9999);
            Socket.Receive = ReceiveData;
        }

        private void SendMessage(DataPacket data)
        {
            try
            {
                Socket.SendMessage(data.ToJson());
                dataPacketPool.Push(data);
            }
            catch (Exception e) 
            { 
                Console.WriteLine(e.Message + e.TargetSite);
            }
        }
        private void ReceiveData(string args)
        {
            JSONObject data = new JSONObject(args);
            CMD cmd = (CMD)data["cmd"].i;

            switch (cmd)
            {
                case CMD.Login: RLogin(data); break;
                //case CMD.GetRole: RGetRole(data); break;
                case CMD.CreateRole: RCreateRole(data); break;
                case CMD.EnterGame: REnterGame(data); break;
                //case CMD.SelectRole: RSelectRole(data); break;
                //case CMD.EnterBattleScene: REnterBattleScene(data); break;
                //case CMD.EnterNewRole: REnterNewRole(data); break;

                default: Console.WriteLine("未知 CMD " + cmd); break;
            }
        }

        private void RLogin(JSONObject data)
        {
            if (data["error"].i > 0)
            {
                Console.WriteLine("ERR: " + data["error"].i);
                return;
            }

            DataManager.Instance.userId = (int)data["userId"].i;
            if (data["roleInfo"].IsNull)
            {
                SCreateRole();
                return;
            }
            DataManager.Instance.role = new Role(data["roleInfo"]);
            SEnterGame();
        }
        private void RCreateRole(JSONObject data)
        {
            if (data["error"].i > 0)
            {
                Console.WriteLine("ERR: " + data["error"].i);
                return;
            }

            DataManager.Instance.role = new Role(data["roleInfo"]);
            SEnterGame();
        }
        private void REnterGame(JSONObject data)
        {
            if (data["error"].i > 0)
            {
                Console.WriteLine("ERR: " + data["error"].i);
                return;
            }
        }
        //private void RSelectRole(DataBase data)
        //{
        //    if (data.error > 0)
        //    {
        //        Console.WriteLine("ERR: " + data.error);
        //        return;
        //    }
        //    SEnterBattleScene(101);
        //}
        //private void REnterBattleScene(DataBase data)
        //{
        //    if (data.error > 0)
        //    {
        //        Console.WriteLine("ERR: " + data.error);
        //        return;
        //    }

        //    BattleManager.Instance.serverId = int.Parse(data.list[0]);
        //}
        //private void REnterNewRole(DataBase data)
        //{
        //    if (data.error > 0)
        //    {
        //        Console.WriteLine("ERR: " + data.error);
        //        return;
        //    }

        //    string roleJson = data.list[0];
        //    BattleManager.Instance.AddEntity(roleJson);

        //    BattleManager.Instance.Attack(1);
        //}

        public void SLogin()
        {
            DataPacket dp = dataPacketPool.Pop();
            dp.Reset(CMD.Login);
            dp.Data.AddField("acc", "test001");
            dp.Data.AddField("pw", "123");
            SendMessage(dp);

            Console.WriteLine("SLogin");
        }
        private void SCreateRole()
        {
            DataPacket dp = dataPacketPool.Pop();
            dp.Reset(CMD.CreateRole);
            dp.Data.AddField("userId", DataManager.Instance.userId);
            dp.Data.AddField("roleName", "机器人" + DataManager.Instance.userId);
            SendMessage(dp);

            Console.WriteLine("SCreateRole");
        }
        //private void SGetRole()
        //{
        //    DataBase db = DataPool.Instance.Pop(CMD.GetRole);
        //    db.Add(DataManager.Instance.userId);
        //    SendMessage(db);

        //    Console.WriteLine("SGetRole");
        //}
        private void SEnterGame()
        {
            DataPacket dp = dataPacketPool.Pop();
            dp.Reset(CMD.EnterGame);
            dp.Data.AddField("userId", DataManager.Instance.userId);
            SendMessage(dp);

            Console.WriteLine("SEnterGame");
        }
        //private void SSelectRole()
        //{
        //    DataBase db = DataPool.Instance.Pop(CMD.SelectRole);
        //    db.Add(DataManager.Instance.userId);
        //    SendMessage(db);

        //    Console.WriteLine("SelectRole");
        //}
        //private void SEnterBattleScene(int sceneId)
        //{
        //    DataBase db = DataPool.Instance.Pop(CMD.EnterBattleScene);
        //    db.Add(DataManager.Instance.userId);
        //    db.Add(sceneId);
        //    SendMessage(db);

        //    Console.WriteLine("StartBattle");
        //}
        //public void SAttack(int entityId)
        //{
        //    DataBase db = DataPool.Instance.Pop(CMD.Attack);
        //    db.Add(DataManager.Instance.userId);
        //    db.Add(BattleManager.Instance.serverId);
        //    db.Add(entityId);
        //    SendMessage(db);
        //}
    }
}
