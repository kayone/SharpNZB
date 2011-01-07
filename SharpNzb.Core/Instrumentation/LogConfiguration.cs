using System.Diagnostics;
using System.IO;
using NLog;
using NLog.Config;
using Ninject;

namespace SharpNzb.Core.Instrumentation
{
    public static class LogConfiguration
    {
        public static void Setup()
        {
            if (Debugger.IsAttached)
            {
                LogManager.ThrowExceptions = true;
            }

            LogManager.Configuration = new XmlLoggingConfiguration(Path.Combine(CentralDispatch.AppPath, "log.config"), false);
            LogManager.ConfigurationReloaded += ((s, e) => BindCustomLoggers());
            BindCustomLoggers();
        }

        private static void BindCustomLoggers()
        {
#if Release
            var exTarget = new ExceptioneerTarget();
            LogManager.Configuration.AddTarget("Exceptioneer", exTarget);
            LogManager.Configuration.LoggingRules.Add(new LoggingRule("*", NLog.LogLevel.Error, exTarget));
#endif


            var sonicTarget = CentralDispatch.NinjectKernel.Get<SubsonicTarget>();
            LogManager.Configuration.AddTarget("DbLogger", sonicTarget);
            LogManager.Configuration.LoggingRules.Add(new LoggingRule("*", NLog.LogLevel.Info, sonicTarget));

            LogManager.Configuration.Reload();
        }

    }
}



