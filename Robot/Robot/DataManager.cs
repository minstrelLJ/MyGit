using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools;
using Data;

namespace Robot
{
    public class DataManager : Singleton<DataManager>
    {
        public int userId;
        public Role role;
    }
}
