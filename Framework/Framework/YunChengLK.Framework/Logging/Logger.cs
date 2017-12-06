using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YunChengLK.Framework.Logging
{
    public class Logger
    {
        private static ILogProvider log = new Log4NetLogProvider("FileLogger");

        public static void Error(string appName, string appIp, string message, Exception exception = null)
        { 
            message = string.Format("{0}({1},{2})",message, appName, appIp);
            Error(message, exception);
        }

        public static void Error(string message, Exception exception = null)
        {
            log.Log(LogLevel.Error, message, exception);
        }

        public static void Debug(string message)
        {
            log.Log(LogLevel.Debug, message);
        }

        public static void Info(string message)
        {
            log.Log(LogLevel.Info, message);
            //log.Log("FileLogger", LogLevel.Info, message);
        }
    }
}
