using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace ExcelToJson
{
    public class ExcelTools
    {
        /// <summary>
        /// Excel文件列表
        /// </summary>
        private static List<string> excelList;

        public static void Start()
        {
            LoadExcel();
            Convert();
        }

        private static void LoadExcel()
        {
            if (excelList == null) excelList = new List<string>();
            excelList.Clear();

            string path = AppDomain.CurrentDomain.BaseDirectory;
            DirectoryInfo dir = new DirectoryInfo(path);
            FileInfo[] fil = dir.GetFiles();

            string fiPath = "";
            foreach (var fi in fil)
            {
                if (fi.Extension == ".xlsx")
                {
                    fiPath = fi.FullName.Replace("\\", "/");
                    excelList.Add(fiPath);
                }
            }
        }

         //<summary>
         //转换Excel文件
         //</summary>
        private static void Convert()
        {
            foreach (string assetsPath in excelList)
            {
                //构造Excel工具类
                ExcelUtility excel = new ExcelUtility(assetsPath);

                //编码类型
                Encoding encoding = Encoding.GetEncoding("utf-8");

                string output = assetsPath.Replace(".xlsx", ".json");
                excel.ConvertToJson(output, encoding);
            }
        }
    }
}
