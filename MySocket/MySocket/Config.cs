using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools;

namespace MySocket
{
    public class Config
    {
        /// <summary>
        /// 最大连接数
        /// </summary>
        public static int PARALLE_NUM = 3000;

        /// <summary>
        /// Socket超时设置为（毫秒）
        /// </summary>
        public static int SOCKET_TIME_OUT_MS = 20 * 1000;

        /// <summary>
        /// 解析命令初始缓存大小
        /// </summary>
        public static int INIT_BUFFER_SIZE = 1024 * 4;

        /// <summary>
        /// IOCP接收数据缓存大小，设置过小会造成事件响应增多，设置过大会造成内存占用偏多
        /// </summary>
        public static int RECEIVE_BUFFER_SIZE = 1024 * 4;
    }
}
