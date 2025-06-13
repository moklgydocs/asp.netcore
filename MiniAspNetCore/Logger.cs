using System;

namespace CustomAspNetCore
{
    /// <summary>
    /// 日志接口 - 定义日志记录的契约
    /// </summary>
    public interface ILogger
    {
        void Log(string message);
        void LogError(string message, Exception exception = null);
        void LogWarning(string message);
        void LogInformation(string message);
    }
    
    /// <summary>
    /// 控制台日志实现 - 将日志输出到控制台
    /// 模拟ASP.NET Core的日志系统
    /// </summary>
    public class ConsoleLogger : ILogger
    {
        public void Log(string message)
        {
            LogInformation(message);
        }
        
        public void LogError(string message, Exception exception = null)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[ERROR] {DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}");
            if (exception != null)
            {
                Console.WriteLine($"Exception: {exception}");
            }
            Console.ResetColor();
        }
        
        public void LogWarning(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"[WARN]  {DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}");
            Console.ResetColor();
        }
        
        public void LogInformation(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[INFO]  {DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}");
            Console.ResetColor();
        }
    }
}