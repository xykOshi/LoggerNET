using LoggerNET.Objects;
using LoggerNET.Objects.Enums;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text.Json;

namespace LoggerNET
{
    public sealed class Logger
    {
        private static readonly Lazy<Logger> lazy = new Lazy<Logger>(() => new Logger());
        private ConcurrentQueue<LogEntry> logQueue;
        private Thread loggerThread;
        private string logDirectory;
        private int logFileRotationHours;
        private int logFilePurgeDays;
        private DateTime lastRotation;
        private LogStatistics statistics;

        public LogStatistics Statistics => statistics;
        private Logger()
        {
            logQueue = new ConcurrentQueue<LogEntry>();
            loggerThread = new Thread(ProcessLogQueue);
            logDirectory = "Logs";
            logFileRotationHours = 24;
            logFilePurgeDays = 30;
            lastRotation = DateTime.Now;
            statistics = new LogStatistics();
            loggerThread.Start();
        }

        public static Logger Instance => lazy.Value;

        public void Initialize(string directory, int rotationHours = 24, int purgeDays = 30)
        {
            logDirectory = directory;
            logFileRotationHours = rotationHours;
            logFilePurgeDays = purgeDays;
        }

        public void Log(string message, LogLevel level = LogLevel.Info)
        {
            LogEntry entry = new LogEntry
            {
                Timestamp = DateTime.Now,
                Message = message,
                Level = level,
                CallingClass = GetCallingClass(),
                CallingMethod = GetCallingMethod()
            };
            logQueue.Enqueue(entry);
            statistics.IncrementLogsLogged();
        }

        public async Task<T> TimeFunction<T>(Func<Task<T>> func)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            T result = await func();
            stopwatch.Stop();
            long elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
            LogEntry entry = new LogEntry
            {
                Timestamp = DateTime.Now,
                Message = $"Function execution time: {elapsedMilliseconds}ms",
                Level = LogLevel.Debug,
                CallingClass = GetCallingClass(),
                CallingMethod = GetCallingMethod()
            };
            logQueue.Enqueue(entry);
            statistics.IncrementFunctionsTimed();
            statistics.AddFunctionExecutionTime(elapsedMilliseconds);
            return result;
        }

        public T TimeFunction<T>(Func<T> func)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            T result = func();
            stopwatch.Stop();
            long elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
            LogEntry entry = new LogEntry
            {
                Timestamp = DateTime.Now,
                Message = $"Function execution time: {elapsedMilliseconds}ms",
                Level = LogLevel.Debug,
                CallingClass = GetCallingClass(),
                CallingMethod = GetCallingMethod()
            };
            logQueue.Enqueue(entry);
            statistics.IncrementFunctionsTimed();
            statistics.AddFunctionExecutionTime(elapsedMilliseconds);
            return result;
        }

        private void ProcessLogQueue()
        {
            while (true)
            {
                if (logQueue.TryDequeue(out LogEntry entry))
                {
                    RotateLogFileIfNeeded();
                    WriteLogEntryToFile(entry);
                    UpdateStatistics();
                }
                else
                {
                    Thread.Sleep(100);
                }
            }
        }

        private void RotateLogFileIfNeeded()
        {
            TimeSpan elapsedTime = DateTime.Now - lastRotation;
            if (elapsedTime.TotalHours >= logFileRotationHours)
            {
                lastRotation = DateTime.Now;
                PurgeOldLogFiles();
            }
        }

        private void WriteLogEntryToFile(LogEntry entry)
        {
            string logFilePath = Path.Combine(logDirectory, $"log_{lastRotation:yyyyMMdd}.json");
            string json = JsonSerializer.Serialize(entry);
            File.AppendAllText(logFilePath, json + Environment.NewLine);
        }

        private void PurgeOldLogFiles()
        {
            string[] logFiles = Directory.GetFiles(logDirectory, "*.json");
            foreach (string logFile in logFiles)
            {
                FileInfo fileInfo = new FileInfo(logFile);
                if (fileInfo.LastWriteTime < DateTime.Now.AddDays(-logFilePurgeDays))
                {
                    File.Delete(logFile);
                }
            }
        }

        private void UpdateStatistics()
        {
            statistics.IncrementLogsLogged24Hours();
        }

        private string GetCallingClass()
        {
            StackTrace stackTrace = new StackTrace();
            StackFrame frame = stackTrace.GetFrame(2);
            return frame.GetMethod().DeclaringType.Name;
        }

        private string GetCallingMethod()
        {
            StackTrace stackTrace = new StackTrace();
            StackFrame frame = stackTrace.GetFrame(2);
            return frame.GetMethod().Name;
        }
    }
}
