using System;

namespace CameraAnalyzer.bl.Utils
{
    public class Logger
    {
        // Prints an INFO message with only the tag colored
        public void LogInfo(string message)
        {
            Console.Write($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] ");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("[INFO]");
            Console.ResetColor();
            Console.WriteLine($" {message}");
        }

        // Prints an ERROR message with only the tag colored
        public void LogError(string message)
        {
            Console.Write($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] ");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("[ERROR]");
            Console.ResetColor();
            Console.WriteLine($" {message}");
        }

        // Optional: add a warning method too
        public void LogWarning(string message)
        {
            Console.Write($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("[WARNING]");
            Console.ResetColor();
            Console.WriteLine($" {message}");
        }
    }
}
