using System;

namespace CameraAnalyzer.bl.Utils
{
    public static class Logger
    {
        private static readonly object _lock = new object();

        public static void LogInfo(string message)
        {
            lock (_lock)
            {
                Console.Write($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] ");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("[INFO]");
                Console.ResetColor();
                Console.WriteLine($" {message}");
            }
        }

        public static void LogError(string message)
        {
            lock (_lock)
            {
                Console.Write($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] ");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("[ERROR]");
                Console.ResetColor();
                Console.WriteLine($" {message}");
            }
        }

        public static void LogWarning(string message)
        {
            lock (_lock)
            {
                Console.Write($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] ");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("[WARNING]");
                Console.ResetColor();
                Console.WriteLine($" {message}");
            }
        }

        public static void LogDebug(string message)
        {
            lock (_lock)
            {
                Console.Write($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] ");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("[DEBUG]");
                Console.ResetColor();
                Console.WriteLine($" {message}");
            }
        }
    }
}
