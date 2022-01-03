using NLog.Web;
using System;
using System.IO;

namespace App.Server
{
    public static class Logger
    {
        private static NLog.ILogger logger;

        public static void init()
        {
            logger = NLogBuilder.ConfigureNLog(String.Concat(Directory.GetCurrentDirectory(), "/nlog.config")).GetCurrentClassLogger();
        }

        public static void Debug(string msg)
        {
            if (logger == null)
            {
                init();
            }

            logger.Debug(msg);
        }

        public static void Error(string msg)
        {
            if (logger == null)
            {
                init();
            }

            logger.Error(msg);
        }

        public static void Info(string msg)
        {
            if (logger == null)
            {
                init();
            }

            logger.Info(msg);
        }

        public static void Warn(string msg)
        {
            if (logger == null)
            {
                init();
            }

            logger.Warn(msg);
        }
    }
}
