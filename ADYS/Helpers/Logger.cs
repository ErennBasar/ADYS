using log4net;
using System;

namespace ADYS.Helpers
{
    public static class Logger
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Logger));

        public static void Info(string message)
        {
            log.Info(message);
        }

        public static void Error(string message, Exception ex = null)
        {
            log.Error(message, ex);
        }

        public static void Warn(string message)
        {
            log.Warn(message);
        }
    }
}
