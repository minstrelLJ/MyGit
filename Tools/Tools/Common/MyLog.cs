using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text;
using System.IO;

namespace Tools
{
    public delegate void MyDelegate(string _format, params object[] _args);
    public class MyLog
    {
        public static MyDelegate log;
        public static MyDelegate warning;
        public static MyDelegate error;

        public static void Init(LogType logType)
        {
            switch (logType)
            {
                case LogType.Console:
                    log += ConsoleLog;
                    warning += ConsoleWarning;
                    error += ConsoleError;
                    break;

                case LogType.Text:
                    log += TextDebug;
                    warning += TextWarning;
                    error += TextError;
                    break;

                case LogType.Custom:
                    break;
                default:
                    break;
            }
        }

        #region public Function

        public static void Log(string _format, params object[] _args)
        {
            if (log != null) log(_format, _args);
        }
        public static void Warning(string _format, params object[] _args)
        {
            if (warning != null) warning(_format, _args);
        }
        public static void Error(string _format, params object[] _args)
        {
            if (error != null) error(_format, _args);
        }

        #endregion public Function
        #region ConsoleLog

        private static void ConsoleLog(string _format, params object[] _args)
        {
            ConsoleWriteLine(LogLevel.Debug, _format, _args);
        }
        private static void ConsoleWarning(string _format, params object[] _args)
        {
            ConsoleWriteLine(LogLevel.Warning, _format, _args);
        }
        private static void ConsoleError(string _format, params object[] _args)
        {
            ConsoleWriteLine(LogLevel.Error, _format, _args);
        }
        private static void ConsoleWriteLine(LogLevel level, string format, params object[] args)
        {
            switch (level)
            {
                case LogLevel.Debug: if (Console.ForegroundColor != ConsoleColor.Gray) Console.ForegroundColor = ConsoleColor.Gray; break;
                case LogLevel.Warning: if (Console.ForegroundColor != ConsoleColor.Yellow) Console.ForegroundColor = ConsoleColor.Yellow; break;
                case LogLevel.Error: if (Console.ForegroundColor != ConsoleColor.Red) Console.ForegroundColor = ConsoleColor.Red; break;
            }

            string msg = string.Format(format, args);
            Console.WriteLine(msg);
        }

        #endregion ConsoleLog
        #region Text

        private static void TextDebug(string format, params object[] args)
        {
            Set2Text(LogLevel.Debug, "[Debug]:" + format, args);
        }
        private static void TextWarning(string format, params object[] args)
        {
            ConsoleWarning(format, args);
            Set2Text(LogLevel.Warning, "[Warning]:" + format, args);
        }
        private static void TextError(string format, params object[] args)
        {
            ConsoleError(format, args);
            Set2Text(LogLevel.Error, "[Error]:" + format, args);
        }
        private static void Set2Text(LogLevel level, string format, params object[] args)
        {
            string msg = string.Format(format, args);

            string FileDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Logs");
            if (!Directory.Exists(FileDirectory))
                Directory.CreateDirectory(FileDirectory);

            string fileName = DateTime.Now.ToString("yyyy-MM-dd") + ".txt";
            FileIO.AddText(FileDirectory + "/" + fileName, DateTime.Now.ToString("HH:mm:ss ") + msg);
        }

        #endregion Text
    }

    public enum LogType
    {
        Console,            // 控制台
        Text,                  // 文本
        Custom,             // 自定义
    }

    public enum LogLevel
    {
        Debug,
        Warning,
        Error
    }
}
