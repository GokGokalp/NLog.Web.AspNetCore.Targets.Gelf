using System;
using System.IO;
using System.Reflection;
using NLog;
using NLog.Config;

namespace NLog.Web.AspNetCore.Targets.Gelf.ConsoleRunner
{
    class Program
    {
        static Logger _logger = LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {
            LogManager.Configuration = new XmlLoggingConfiguration("nlog.config");

            _logger.Debug("---------------------Test .NET Core ---------------------");
        }
    }
}