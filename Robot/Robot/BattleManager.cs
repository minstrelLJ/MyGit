using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools;
using Data;

namespace Robot
{
    public class BattleManager : Singleton<BattleManager>
    {
        public int serverId;

        List<Role> entityList = new List<Role>();

        public void AddEntity(string json)
        {
            entityList = FileIO.JsonToObject<List<Role>>(json);
        }

        public void Attack(int entityId)
        {
            //NetManager.Instance.SAttack(entityId);
        }
    }
}
