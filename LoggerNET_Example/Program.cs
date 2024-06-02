using LoggerNET;
using LoggerNET.Objects;
using LoggerNET.Objects.Enums;

namespace LoggerNET_Example
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            // Initialize the logger with the desired configuration
            Logger.Instance.Initialize("", rotationHours: 12, purgeDays: 15);

            // Log messages with different log levels
            Logger.Instance.Log("This is an info log.");
            Logger.Instance.Log("This is a warning log.", LogLevel.Warning);
            Logger.Instance.Log("This is an error log.", LogLevel.Error);


            // Time a synchronous function
            int syncResult = Logger.Instance.TimeFunction(() =>
            {
                // Simulating some work
                System.Threading.Thread.Sleep(500);
                return 42;
            });
            Console.WriteLine($"Synchronous function result: {syncResult}");

            // Time an asynchronous function
            string asyncResult = await Logger.Instance.TimeFunction(async () =>
            {
                // Simulating some asynchronous work
                await Task.Delay(1000);
                return "Async result";
            });
            Console.WriteLine($"Asynchronous function result: {asyncResult}");

            // Log messages from different classes and methods
            MyClass.DoSomething();
            OtherClass.DoSomethingElse();

            // Retrieve and display log statistics
            LogStatistics stats = Logger.Instance.Statistics;
            Console.WriteLine($"Total logs logged: {stats.LogsLogged}");
            Console.WriteLine($"Logs logged in the last 24 hours: {stats.LogsLogged24Hours}");
            Console.WriteLine($"Functions timed: {stats.FunctionsTimed}");
            Console.WriteLine($"Total function execution time: {stats.TotalFunctionExecutionTime}ms");

            Console.ReadLine();
        }
    }

    class MyClass
    {
        public static void DoSomething()
        {
            Logger.Instance.Log("Doing something in MyClass.");
        }
    }

    class OtherClass
    {
        public static void DoSomethingElse()
        {
            Logger.Instance.Log("Doing something else in OtherClass.");
        }
    }
}
