using System;
using SubSonic.SqlGeneration.Schema;

namespace SharpNzb.Core.Instrumentation
{
    public class Log
    {
        public int LogId { get; set; }
        public string Message { get; set; }
        public DateTime Time { get; set; }
        public string Logger { get; set; }

        [SubSonicNullString]
        public string Stack { get; set; }
        [SubSonicNullString]
        public string ExceptionMessage { get; set; }
        [SubSonicNullString][SubSonicLongString]
        public string ExceptionString { get; set; }
        [SubSonicNullString]
        public string ExceptionType { get; set; }

        public LogLevel Level { get; set; }

        //This is needed for telerik grid binding
        [SubSonicIgnore]
        public string DisplayLevel{
            get { return Level.ToString(); }
        }
    }
}
