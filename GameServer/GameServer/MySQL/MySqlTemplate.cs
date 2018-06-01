using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Reflection;
using MySql.Data;
using MySql.Data.MySqlClient;
using Tools;

namespace GameServer
{
    public class MySqlTemplate
    {
        public static List<T> IList<T>(DataSet ds)
        {
            T model = default(T);

            List<T> list = new List<T>();
            if (ds.Tables[0].Columns[0].ColumnName == "RowId")
            {
                ds.Tables[0].Columns.Remove("RowId");
            }

            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                model = Activator.CreateInstance<T>();
                foreach (DataColumn dc in dr.Table.Columns)
                {
                    var ss = model.GetType();

                    PropertyInfo pi = ss.GetProperty(dc.ColumnName);
                    if (dr[dc.ColumnName] != DBNull.Value)
                        pi.SetValue(model, dr[dc.ColumnName], null);
                    else
                        pi.SetValue(model, null, null);

                }
                list.Add(model);
            }
            return list;
        }

        public static int DROP(string tableName)
        {
            string cmd = string.Format("DROP TABLE IF EXISTS {0};", tableName);

            object obj = MySqlHelper.ExecuteScalar(MySqlHelper.Conn, CommandType.Text, cmd, null);
            return Convert.ToInt32(obj);
        }

        /// <summary>
        /// 插入数据
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="keys">键</param>
        /// <param name="values">值</param>
        /// <returns></returns>
        public static int INSERT(string tableName, string[] keys, object[] values)
        {
            string key = "(" + keys[0];
            for (int i = 1; i < keys.Length; i++)
            {
                if (values[i] != null)
                    key += "," + keys[i];
            }
            key += ")";

            string value = "(" + GetValueByType(values[0]);
            for (int i = 1; i < values.Length; i++)
            {
                if (values[i] == null)
                    continue;

                value += "," + GetValueByType(values[i]);
            }
            value += ")";

            string cmd = string.Format("INSERT INTO {0} {1} VALUES {2}", tableName, key, value);
            if (cmd == null)
                return 0;

            object obj = MySqlHelper.ExecuteScalar(MySqlHelper.Conn, CommandType.Text, cmd, null);
            return Convert.ToInt32(obj);
        }

        /// <summary>
        /// 查询数据库
        /// </summary>
        /// <param name="tableNames"></param>
        /// <param name="keys"></param>
        /// <param name="asc">排序 true 升序 false 降序</param>
        /// <param name="where"></param>
        /// <returns></returns>
        public static DataSet SELECT(string[] tableNames, string[] keys, string where = "", bool asc = true, string[] sortKeys = null)
        {
            string key = keys[0];
            for (int i = 1; i < keys.Length; i++)
            {
                key += "," + keys[i];
            }

            string tableName = tableNames[0];
            for (int i = 1; i < tableNames.Length; i++)
            {
                tableName += "," + tableNames[i];
            }

            string cmd = string.Format("SELECT {0}" +
                " FROM {1}", key, tableName);

            if (!string.IsNullOrEmpty(where))
            {
                cmd += " WHERE " + where;
            }

            if (sortKeys != null)
            {
                string sortKey = sortKeys[0];
                for (int i = 1; i < sortKeys.Length; i++)
                {
                    sortKey += ", " + sortKeys[i];
                }
                cmd += " ORDER BY " + sortKey + (asc ? " ASC" : " DESC");
            }

            try
            {
                object obj = MySqlHelper.GetDataSet(MySqlHelper.Conn, CommandType.Text, cmd, null);
                return obj as DataSet;
            }
            catch (Exception e)
            {
                MyLog.Error(e.Message + e.TargetSite);
                return null;
            }

        }
        public static List<T> SELECT<T>(string[] tableNames, string[] keys, string where = "", bool asc = true, string[] sortKeys = null)
        {
            DataSet data = SELECT(tableNames, keys, where, asc, sortKeys);
            return MySqlTemplate.IList<T>(data);
        }
        public static void SELECT_GROUP_BY(string tableName, string selectName, string byName, GroupType type)
        {
            // http://www.runoob.com/mysql/mysql-group-by-statement.html 说明地址
            // SELECT name, COUNT(*) FROM   employee_tbl GROUP BY name;
            // SELECT name, SUM(singin) as singin_count FROM  employee_tbl GROUP BY name WITH ROLLUP;
            // SELECT coalesce(name, '总数'), SUM(singin) as singin_count FROM  employee_tbl GROUP BY name WITH ROLLUP;
        }
        public static int UPDATE(string tableName, string[] keys, object[] values, string where = "")
        {
            string value = "";
            Type type;
            string changeValue = "";
            for (int i = 0; i < keys.Length; i++)
            {
                type = values[i].GetType();

                if (type == typeof(int)) value = string.Format("{0}", values[i]);
                else if (type == typeof(string)) value = string.Format("'{0}'", values[i]);

                changeValue += (i == 0) ? string.Format("{0}={1}", keys[i], value) : ", " + string.Format("{0}={1}", keys[i], value);
            }

            string cmd = string.Format("UPDATE {0} SET {1}", tableName, changeValue);
            if (!string.IsNullOrEmpty(where))
            {
                cmd += " WHERE " + where;
            }

            object obj = MySqlHelper.ExecuteScalar(MySqlHelper.Conn, CommandType.Text, cmd, null);
            return Convert.ToInt32(obj);
        }
        public static int DELETE(string tableName, string where = "")
        {
            string cmd = string.Format("DELETE FROM {0}", tableName);
            if (!string.IsNullOrEmpty(where))
            {
                cmd += " WHERE " + where;
            }

            object obj = MySqlHelper.ExecuteScalar(MySqlHelper.Conn, CommandType.Text, cmd, null);
            return Convert.ToInt32(obj);
        }
        public static int ALTER(string tableName, string key, AlterType alterType, DataType dataType = DataType.INT, string key2 = "", bool isFirst = false)
        {
            string dataTypeStr = "";
            string afterStr = "";
            switch (alterType)
            {
                case AlterType.DROP:
                    break;

                case AlterType.ADD:
                    dataTypeStr = GetDataTypeCmd(dataType);
                    if (!string.IsNullOrEmpty(key2))
                    {
                        afterStr = isFirst ? "FIRST" : "AFTER " + key2;
                    }
                    break;

                case AlterType.MODIFY:
                    dataTypeStr = GetDataTypeCmd(dataType);
                    break;

                case AlterType.CHANGE:
                    dataTypeStr = GetDataTypeCmd(dataType);
                    key = key + " " + key2;
                    break;

                case AlterType.RENAME:
                    break;

                default:
                    break;
            }

            string cmd = string.Format("ALTER TABLE {0} {1} {2} {3} {4}", tableName, GetAlterTypeCmd(alterType), key, dataTypeStr, afterStr);

            object obj = MySqlHelper.ExecuteScalar(MySqlHelper.Conn, CommandType.Text, cmd, null);
            return Convert.ToInt32(obj);
        }
        public static int INDEX(string tableName, string indexName, string columnName = "", bool isAdd = true)
        {
            string cmd = "";

            if (isAdd) cmd = string.Format("ALTER TABLE {0} ADD INDEX {1}({2})", tableName, indexName, columnName);
            else cmd = string.Format("ALTER TABLE {0} DROP INDEX {1}", tableName, indexName);

            object obj = MySqlHelper.ExecuteScalar(MySqlHelper.Conn, CommandType.Text, cmd, null);
            return Convert.ToInt32(obj);
        }

        public static string GetAlterTypeCmd(AlterType type)
        {
            string cmd = "";
            switch (type)
            {
                case AlterType.DROP: cmd = "DROP"; break;
                case AlterType.ADD: cmd = "ADD"; break;
                case AlterType.MODIFY: cmd = "MODIFY"; break;
                case AlterType.CHANGE: cmd = "CHANGE"; break;
                case AlterType.RENAME: cmd = "RENAME TO"; break;

                default:
                    break;
            }
            return cmd;
        }
        public static string GetDataTypeCmd(DataType type)
        {
            string cmd = "";
            switch (type)
            {
                case DataType.INT: cmd = " INT"; break;
                case DataType.LONG: cmd = " BIGINT"; break;
                case DataType.FLOAT: cmd = " FLOAT"; break;
                case DataType.DOUBLE: cmd = " DOUBLE"; break;
                case DataType.CHAR1: cmd = " CHAR(1)"; break;
                case DataType.CHAR10: cmd = " CHAR(10)"; break;
                case DataType.VARCHAR40: cmd = " VARCHAR(40)"; break;
                default:
                    break;
            }
            return cmd;
        }
        public static string GetEncodingCmd(Encoding encoding)
        {
            string cmd = "";
            switch (encoding)
            {
                case Encoding.UTF8: cmd = " DEFAULT CHARSET=utf8"; break;
                default:
                    break;
            }
            return cmd;
        }

        private static string GetValueByType(object obj)
        {
            Type type = obj.GetType();
            string value = "";

            if (type == typeof(string)) value = "'" + obj + "'";
            else if (type == typeof(int) || type == typeof(float) || type == typeof(Int64))
                value = obj.ToString();
            else MyLog.Error("未知数据类型" + type);
            return value;
        }

        #region CreateTable

        public static void CreateSetting()
        {
            MySqlTemplate.DROP("setting");
            CreateTableElement ct = new CreateTableElement("setting");
            ct.Add("nextUserId", DataType.INT, true);
            ct.Add("nextRoleId", DataType.INT, true);
            ct.Add("levelUpAddAttributePoint", DataType.INT, true);
            ct.Create();

            MySqlTemplate.INSERT("setting", new string[] { "nextUserId", "nextRoleId", "levelUpAddAttributePoint" }, new object[] { 1000, 1000, 5 });
        }
        public static void CreateUser()
        {
            MySqlTemplate.DROP("user");
            CreateTableElement ct = new CreateTableElement("user");
            ct.Add("userId", DataType.INT, true, true);
            ct.Add("accountNumber", DataType.VARCHAR40, true, true);
            ct.Add("password", DataType.VARCHAR40);
            ct.Add("roleId", DataType.INT);
            ct.Create();
        }
        public static void CreateRole()
        {
            MySqlTemplate.DROP("role");
            CreateTableElement ct = new CreateTableElement("role");
            ct.Add("roleId", DataType.INT, true, true);
            ct.Add("roleName", DataType.VARCHAR40);
            ct.Add("level", DataType.INT);
            ct.Add("exp", DataType.LONG);
            ct.Add("fixedSTR", DataType.FLOAT);
            ct.Add("fixedDEX", DataType.FLOAT);
            ct.Add("fixedMAG", DataType.FLOAT);
            ct.Add("fixedCON", DataType.FLOAT);
            ct.Add("potentialSTR", DataType.FLOAT);
            ct.Add("potentialDEX", DataType.FLOAT);
            ct.Add("potentialMAG", DataType.FLOAT);
            ct.Add("potentialCON", DataType.FLOAT);
            ct.Create();
        }

        #endregion

        #region other

        public static void AddUser()
        {
            DataManager.Instance.AddNewUser("test001", "123");
        }

        #endregion

        #region Test


        public static void TestJOIN()
        {
            MySqlTemplate.DROP("Test1");
            CreateTableElement ct = new CreateTableElement("Test1");
            ct.Add("ID", DataType.INT, true);
            ct.Add("Name", DataType.VARCHAR40, true);
            ct.Add("Age", DataType.INT);
            ct.Create();

            MySqlTemplate.INSERT("Test1", new string[] { "ID", "Name", "Age" }, new object[] { 100, "张三", 16 });
            MySqlTemplate.INSERT("Test1", new string[] { "ID", "Name", "Age" }, new object[] { 101, "王五", 26 });
            MySqlTemplate.INSERT("Test1", new string[] { "ID", "Name", "Age" }, new object[] { 102, "李四", 18 });
            MySqlTemplate.INSERT("Test1", new string[] { "ID", "Name", "Age" }, new object[] { 103, "赵六", 55 });

            MySqlTemplate.DROP("Test2");
            ct.Clear();
            ct = new CreateTableElement("Test2");
            ct.Add("Name", DataType.VARCHAR40, true);
            ct.Add("Level", DataType.INT);
            ct.Create();

            MySqlTemplate.INSERT("Test2", new string[] { "Name", "Level" }, new object[] { "张三", 6 });
            MySqlTemplate.INSERT("Test2", new string[] { "Name", "Level" }, new object[] { "王五", 7 });
            MySqlTemplate.INSERT("Test2", new string[] { "Name", "Level" }, new object[] { "李四", 12 });
            MySqlTemplate.INSERT("Test2", new string[] { "Name", "Level" }, new object[] { "xx", 99 });

            // 普通 join
            //string cmd = "SELECT a.Name, a.Age, b.Level FROM Test1 a INNER JOIN Test2 b ON a.Name = b.Name";

            // LEFT join
            //string cmd = "SELECT a.Name, a.Age, b.Level FROM Test1 a LEFT JOIN Test2 b ON a.Name = b.Name";

            // RIGHT join
            string cmd = "SELECT a.Name, a.Age, b.Level FROM Test1 a RIGHT JOIN Test2 b ON a.Name = b.Name";

            DataSet obj = MySqlHelper.GetDataSet(MySqlHelper.Conn, CommandType.Text, cmd, null);
            List<Test2> list = MySqlTemplate.IList<Test2>(obj);
            foreach (var item in list)
            {
                Console.WriteLine("Name:{0} Age:{1} Level:{2}", item.Name, item.Age, item.Level);
            }
        }
        public static void TestNULL()
        {
            MySqlTemplate.DROP("Test1");
            CreateTableElement ct = new CreateTableElement("Test1");
            ct.Add("ID", DataType.INT, true);
            ct.Add("Name", DataType.VARCHAR40, true);
            ct.Add("Age", DataType.INT);
            ct.Create();

            MySqlTemplate.INSERT("Test1", new string[] { "ID", "Name", "Age" }, new object[] { 100, "张三", 16 });
            MySqlTemplate.INSERT("Test1", new string[] { "ID", "Name", "Age" }, new object[] { 101, "王五", null });
            MySqlTemplate.INSERT("Test1", new string[] { "ID", "Name", "Age" }, new object[] { 102, "李四", null });
            MySqlTemplate.INSERT("Test1", new string[] { "ID", "Name", "Age" }, new object[] { 103, "赵六", 55 });

            //string cmd = "SELECT * FROM Test1 WHERE Age IS NULL;";

            string cmd = "SELECT * FROM Test1 WHERE Age IS NOT NULL;";

            DataSet obj = MySqlHelper.GetDataSet(MySqlHelper.Conn, CommandType.Text, cmd, null);
            List<Test2> list = MySqlTemplate.IList<Test2>(obj);
            foreach (var item in list)
            {
                Console.WriteLine("ID:{0} Name:{1} Age:{2}", item.ID, item.Name, item.Age);
            }
        }
        public static void TestALTER()
        {
            MySqlTemplate.DROP("Test1");
            CreateTableElement ct = new CreateTableElement("Test1");
            ct.Add("ID", DataType.INT, true);
            ct.Add("Name", DataType.CHAR10, true);
            ct.Add("Age", DataType.INT);
            ct.Create();

            MySqlTemplate.INSERT("Test1", new string[] { "ID", "Name", "Age" }, new object[] { 100, "张三", 16 });
            MySqlTemplate.INSERT("Test1", new string[] { "ID", "Name", "Age" }, new object[] { 101, "王五", 17 });
            MySqlTemplate.INSERT("Test1", new string[] { "ID", "Name", "Age" }, new object[] { 102, "李四", 18 });
            MySqlTemplate.INSERT("Test1", new string[] { "ID", "Name", "Age" }, new object[] { 103, "赵六", 55 });

            MySqlTemplate.ALTER("Test1", "Age", AlterType.DROP);

            string cmd = "SELECT * FROM Test1";
            DataSet obj = MySqlHelper.GetDataSet(MySqlHelper.Conn, CommandType.Text, cmd, null);
            List<Test2> list = MySqlTemplate.IList<Test2>(obj);
            foreach (var item in list)
            {
                Console.WriteLine("ID:{0} Name:{1} Age:{2}", item.ID, item.Name, item.Age);
            }
            Console.WriteLine();

            MySqlTemplate.ALTER("Test1", "Gold", AlterType.ADD, DataType.INT, "ID");
            MySqlTemplate.UPDATE("Test1", new string[] { "Gold" }, new object[] { 2000 });

            cmd = "SELECT * FROM Test1";
            obj = MySqlHelper.GetDataSet(MySqlHelper.Conn, CommandType.Text, cmd, null);
            list = MySqlTemplate.IList<Test2>(obj);
            foreach (var item in list)
            {
                Console.WriteLine("ID:{0} Name:{1} Gold:{2}", item.ID, item.Name, item.Gold);
            }
            Console.WriteLine();

            MySqlTemplate.ALTER("Test1", "Name", AlterType.MODIFY, DataType.VARCHAR40);

            MySqlTemplate.ALTER("Test1", "Name", AlterType.CHANGE, DataType.VARCHAR40, "Nickname");

            MySqlTemplate.ALTER("Test1", "MyTest", AlterType.RENAME);
        }
        public static void TestINDEX()
        {
            //MySqlTemplate.DROP("Test1");
            //CreateTableElement ct = new CreateTableElement("Test1");
            //ct.Add("ID", DataType.INT, true);
            //ct.Add("Name", DataType.CHAR10, true);
            //ct.Add("Age", DataType.INT);
            //ct.Create();

            //for (int i = 0; i < 800; i++)
            //{
            //    MySqlTemplate.INSERT("Test1", new string[] { "ID", "Name", "Age" }, new object[] { 1000 + i, "张三", 16 });
            //}

            //MySqlTemplate.INDEX("Test1", "ByID", "ID");
            //MySqlTemplate.INDEX("Test1", "ByID", "", false);

            List<Test2> list = MySqlTemplate.SELECT<Test2>(new string[] { "Test1" }, new string[] { "ID" }, "ID>1556");
            foreach (var item in list)
            {
                Console.WriteLine("ID:{0} Name:{1} Age:{2}", item.ID, item.Name, item.Age);
            }
            Console.WriteLine();
        }

        public class Test
        {
            public string Name { get; set; }
            public int Age { get; set; }
        }
        public class Test2
        {
            public int ID { get; set; }
            public string Name { get; set; }
            public int Age { get; set; }
            public int Level { get; set; }
            public int Gold { get; set; }
        }

        #endregion
    }

    #region enum

    public enum DataType
    {
        INT,
        LONG,
        FLOAT,
        DOUBLE,
        CHAR1,
        CHAR10,
        VARCHAR40,
    }

    public enum AlterType
    {
        DROP,
        ADD,
        MODIFY,
        CHANGE,
        RENAME,
    }

    public enum GroupType
    {
        COUNT,
    }

    public enum Encoding
    {
        UTF8,
    }

    #endregion
}