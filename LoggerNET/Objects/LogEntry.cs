using LoggerNET.Objects.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoggerNET.Objects
{
    public class LogEntry
    {
        public DateTime Timestamp { get; set; }
        public string Message { get; set; }
        public LogLevel Level { get; set; }
        public string CallingClass { get; set; }
        public string CallingMethod { get; set; }
    }
}
