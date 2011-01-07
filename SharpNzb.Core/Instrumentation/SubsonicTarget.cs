using System;
using NLog;
using NLog.Targets;
using SubSonic.Repository;

namespace SharpNzb.Core.Instrumentation
{
    public class SubsonicTarget : Target
    {
        private readonly IRepository _repo;

        public SubsonicTarget(IRepository repo)
        {
            _repo = repo;
        }

        protected override void Write(LogEventInfo logEvent)
        {
            var log = new Log();
            log.Time = logEvent.TimeStamp;
            log.Message = logEvent.FormattedMessage;

            if (log.Stack != null)
            {
                log.Stack = logEvent.StackTrace.ToString();
            }

            log.Logger = logEvent.LoggerName;

            if (logEvent.Exception != null)
            {
                if (String.IsNullOrWhiteSpace(log.Message))
                    log.Message = logEvent.Exception.Message;

                log.ExceptionMessage = logEvent.Exception.Message;
                log.ExceptionString = logEvent.Exception.ToString();
                log.ExceptionType = logEvent.Exception.GetType().ToString();
            }

            switch (logEvent.Level.Name.ToLower())
            {
                case "trace":
                    {
                        log.Level = LogLevel.Trace;
                        break;
                    }
                case "debug":
                    {
                        log.Level = LogLevel.Debug;
                        break;
                    }
                case "info":
                    {
                        log.Level = LogLevel.Info;
                        break;
                    }
                case "warn":
                    {
                        log.Level = LogLevel.Warn;
                        break;
                    }
                case "error":
                    {
                        log.Level = LogLevel.Error;
                        break;
                    }
                case "fatal":
                    {
                        log.Level = LogLevel.Fatal;
                        break;
                    }
            }

            _repo.Add(log);
        }
    }
}