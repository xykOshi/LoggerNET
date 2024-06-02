using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoggerNET.Objects
{
    public class LogStatistics
    {
        private long logsLogged;
        private long logsLogged24Hours;
        private long functionsTimed;
        private long totalFunctionExecutionTime;

        public long LogsLogged => logsLogged;
        public long LogsLogged24Hours => logsLogged24Hours;
        public long FunctionsTimed => functionsTimed;
        public long TotalFunctionExecutionTime => totalFunctionExecutionTime;

        public void IncrementLogsLogged()
        {
            Interlocked.Increment(ref logsLogged);
        }

        public void IncrementLogsLogged24Hours()
        {
            Interlocked.Increment(ref logsLogged24Hours);
        }

        public void IncrementFunctionsTimed()
        {
            Interlocked.Increment(ref functionsTimed);
        }

        public void AddFunctionExecutionTime(long milliseconds)
        {
            Interlocked.Add(ref totalFunctionExecutionTime, milliseconds);
        }
    }
}
