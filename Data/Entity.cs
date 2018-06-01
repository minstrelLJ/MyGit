using System;

namespace Data
{
    [Serializable]
    public class Entity
    {
        public int type = 1;
        public int curHp;
        public int macHp;
        public int atk;
        public int def;

        public int id { get; set; }
        public string name { get; set; }
        public int level { get; set; }
        public long exp { get; set; }

        public Entity() { }
        public Entity(Monster monster)
        {
            type = 2;
            name = monster.monsterName;
            level = monster.level;
            macHp = monster.hp;
            atk = monster.atk;
            def = monster.def;
        }
    }
}
