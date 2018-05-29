using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tools
{
    public delegate void MyDelegate(string _format, params object[] _args);
    public class MyLog
    {
        public static MyDelegate log;
        public static MyDelegate warning;
        public static MyDelegate error;

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

        #region ConsoleLog

        public static void InitToConsole()
        {
            log += ConsoleLog;
            warning += ConsoleWarning;
            error += ConsoleError;
        }
        private static void ConsoleLog(string _format, params object[] _args)
        {
            if (Console.ForegroundColor != ConsoleColor.Gray) Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine(_format, _args);
        }
        private static void ConsoleWarning(string _format, params object[] _args)
        {
            if (Console.ForegroundColor != ConsoleColor.Yellow) Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(_format, _args);
        }
        private static void ConsoleError(string _format, params object[] _args)
        {
            if (Console.ForegroundColor != ConsoleColor.Red) Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(_format, _args);
        }

        #endregion ConsoleLog
    }
}
