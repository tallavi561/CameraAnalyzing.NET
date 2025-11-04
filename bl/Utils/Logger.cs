using System;

namespace CameraAnalyzer.bl.Utils
{
    public sealed class Logger
    {
        private static readonly Lazy<Logger> _instance = new Lazy<Logger>(() => new Logger());
        public static Logger Instance => _instance.Value;

        private Logger() { } // prevent external creation

        public void LogInfo(string message)
        {
            Console.Write($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] ");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("[INFO]");
            Console.ResetColor();
            Console.WriteLine($" {message}");
        }

        public void LogError(string message)
        {
            Console.Write($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] ");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("[ERROR]");
            Console.ResetColor();
            Console.WriteLine($" {message}");
        }

        public void LogWarning(string message)
        {
            Console.Write($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("[WARNING]");
            Console.ResetColor();
            Console.WriteLine($" {message}");
        }

        public void LogDebug(string message)
        {
            Console.Write($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] ");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("[WARNING]");
            Console.ResetColor();
            Console.WriteLine($" {message}");
        }
    }
}
