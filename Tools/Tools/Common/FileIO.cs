using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using Newtonsoft.Json;

namespace Tools
{
    public class FileIO
    {
        /// <summary>
        /// 保存成文本
        /// </summary>
        public static void SaveText(string filePath, string text)
        {
            using (FileStream fs = new FileStream(filePath, FileMode.Create))
            {
                using (StreamWriter sw = new StreamWriter(fs, Encoding.UTF8))
                {
                    sw.WriteLine(text);
                }
            }
        }

        /// <summary>
        /// 追加文本
        /// </summary>
        public static void AddText(string filePath, string text)
        {
            if (!File.Exists(filePath))
            {
                using (FileStream fs = new FileStream(filePath, FileMode.Create))
                {
                    using (StreamWriter sw = new StreamWriter(fs, Encoding.UTF8))
                    {
                        sw.WriteLine(text);
                    }
                }
            }
            else
            {
                using (StreamWriter sw = new StreamWriter(filePath, true, Encoding.UTF8))
                {
                    sw.WriteLine(text);
                }
            }
        }

        /// <summary>
        /// 从文本中读取
        /// </summary>
        public static string ReadTxt(string filePath)
        {
            using (FileStream fs = new FileStream(filePath, FileMode.Open))
            {
                using (StreamReader sw = new StreamReader(fs, Encoding.UTF8))
                {
                    return sw.ReadLine();
                }
            }
        }
        public static string ReadAllTxt(string filePath)
        {
            using (FileStream fs = new FileStream(filePath, FileMode.Open))
            {
                using (StreamReader sw = new StreamReader(fs, Encoding.UTF8))
                {
                    return sw.ReadToEnd();
                }
            }
        }

        /// <summary>
        /// 保存成二进制文件
        /// </summary>
        public static void SaveBin(string filePath, object sender)
        {
            using (FileStream fs = new FileStream(filePath, FileMode.Create))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, sender);
            }
        }

        /// <summary>
        /// 读取二进制文件
        /// </summary>
        public static T ReadBin<T>(string filePath) where T : class
        {
            T p;
            using (FileStream fs = new FileStream(filePath, FileMode.Open))
            {
                BinaryFormatter bf = new BinaryFormatter();
                p = bf.Deserialize(fs) as T;
                return p;
            }
        }

        /// <summary>
        /// 读取 json
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static List<T> ReadJson<T>(string filePath) where T : class
        {
            try
            {
                string content = ReadAllTxt(filePath + ".json");
                return JsonConvert.DeserializeObject<List<T>>(content);
            }
            catch (Exception e)
            {
                MyLog.Error(e.Message);
                MyLog.Error(e.TargetSite.ToString());
                return null;
            }
        }

        /// <summary>
        /// 转换为 json
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToJson(object obj)
        {
            string json = JsonConvert.SerializeObject(obj);
            return json;
        }
        /// <summary>
        /// Json 转为对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        //public static List<T> JsonToObject<T>(string json) where T: class
        //{
        //    return JsonConvert.DeserializeObject<List<T>>(json);
        //}
        public static T JsonToObject<T>(string json) where T : class
        {
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}
