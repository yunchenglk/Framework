using log4net;
using System;

[assembly: log4net.Config.XmlConfigurator(ConfigFile = "Configs\\Log4Net.config", Watch = true)]
namespace YunChengLK.Framework.Logging
{
    public sealed class Log4NetLogProvider : ILogProvider
    {
        private static object _objLock = new object();
        private string _LoggerName = "";
        private ILog curLog;

        public Log4NetLogProvider() {
            curLog = null;
        }

        public Log4NetLogProvider(string loggerName) {
            curLog = null;
            this._LoggerName = loggerName;
        }

        public string LoggerName {
            get { return _LoggerName; }
        }

        public ILog GetLog(string logCategory) {
            ILog resultLog = null;
            if (!string.IsNullOrEmpty(logCategory)) {
                resultLog = LogManager.GetLogger(logCategory);
            }

            if (resultLog == null) {
                if (curLog == null) {
                    lock (_objLock) {
                        if (curLog == null && !string.IsNullOrEmpty(_LoggerName)) {
                            curLog = LogManager.GetLogger(_LoggerName);
                        }
                    }
                }

                return curLog;
            }
            else {
                return resultLog;
            }
        }

        public void Log(LogLevel level, string message, Exception ex= null)
        {
            ILog localLog = GetLog(this._LoggerName);

            switch (level)
            {
                case LogLevel.Info:
                    {
                        if (localLog.IsInfoEnabled)
                        {
                            localLog.Info(message);
                        }
                        break;
                    }

                case LogLevel.Warn:
                    {
                        if (localLog.IsWarnEnabled)
                        {
                            localLog.Warn(message);
                        }

                        break;
                    }

                case LogLevel.Debug:
                    {
                        if (localLog.IsDebugEnabled)
                        {
                            localLog.Debug(message, ex);
                        }

                        break;
                    }

                case LogLevel.Error:
                    {
                        if (localLog.IsErrorEnabled)
                        {
                            localLog.Error(message, ex);
                        }

                        break;
                    }

                case LogLevel.Fatal:
                    {
                        if (localLog.IsFatalEnabled)
                        {
                            localLog.Fatal(message);
                        }
                        break;
                    }
            }

            if (localLog != curLog)
            {
                localLog = null;
            }
        }

        public void Log(string logCategory, LogLevel level, string message) {
            ILog localLog = GetLog(logCategory);
            if (localLog == null || string.IsNullOrEmpty(message)) return;

            switch (level) {
                case LogLevel.Info: {
                        if (localLog.IsInfoEnabled) {
                            localLog.Info(message);
                        }
                        break;
                    }

                case LogLevel.Warn: {
                        if (localLog.IsWarnEnabled) {
                            localLog.Warn(message);
                        }

                        break;
                    }

                case LogLevel.Debug: {
                        if (localLog.IsDebugEnabled) {
                            localLog.Debug(message);
                        }

                        break;
                    }

                case LogLevel.Error: {
                        if (localLog.IsErrorEnabled) {
                            localLog.Error(message);
                        }

                        break;
                    }

                case LogLevel.Fatal: {
                        if (localLog.IsFatalEnabled) {
                            localLog.Fatal(message);
                        }

                        break;
                    }
            }

            if (localLog != curLog) {
                localLog = null;
            }
        }
    }
}