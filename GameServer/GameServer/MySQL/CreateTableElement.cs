using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Reflection;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace GameServer
{
    public class CreateTableElement
    {
        public class Element
        {
            public string keyName;
            public DataType dataType;
            public bool isNotNull;
            public bool isPrimarkKey;
        }

        private string tableName;
        private Encoding encoding;
        private List<Element> elementList = new List<Element>();

        public CreateTableElement(string tableName, Encoding encoding = Encoding.UTF8)
        {
            this.tableName = tableName;
            this.encoding = encoding;
        }

        private string Cmd
        {
            get { return GetHeadCmd() + GetElementCmd(); }
        }

        public void Add(string keyName, DataType dataType, bool isNotNull = false, bool isPrimarkKey = false)
        {
            Element e = new Element();
            e.keyName = keyName;
            e.dataType = dataType;
            e.isNotNull = isNotNull;
            e.isPrimarkKey = isPrimarkKey;

            elementList.Add(e);
        }
        public void Create()
        {
            object obj = MySqlHelper.ExecuteScalar(MySqlHelper.Conn, CommandType.Text, Cmd, null);
            int err = Convert.ToInt32(obj);
        }
        public void Clear()
        {
            elementList.Clear();
        }

        private string GetElementCmd()
        {
            string element = "";
            string primark = " PRIMARY KEY ( `RowId`";

            foreach (Element e in elementList)
            {
                element += string.Format(" `{0}`", e.keyName);
                element += MySqlTemplate.GetDataTypeCmd(e.dataType);
                if (e.isNotNull) element += " NOT NULL";
                element += ",";
                if (e.isPrimarkKey) primark += string.Format(", `{0}`", e.keyName);
            }
            primark += "))ENGINE=InnoDB" + MySqlTemplate.GetEncodingCmd(encoding);
            return element + primark;
        }

        private string GetHeadCmd()
        {
            return string.Format("CREATE TABLE IF NOT EXISTS `{0}`(`RowId` INT UNSIGNED AUTO_INCREMENT NOT NULL,", tableName);
        }
    }
}