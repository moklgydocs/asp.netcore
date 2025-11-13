using MokAbp.DependencyInjection.Attributes;

namespace MokAbp.Demo.Services
{
    /// <summary>
    /// 日志服务接口
    /// </summary>
    public interface ILoggerService
    {
        void Log(string message);
        void LogInfo(string message);
        void LogWarning(string message);
        void LogError(string message);
    }

    /// <summary>
    /// 日志服务实现
    /// </summary>
    [SingletonDependency]
    public class ConsoleLoggerService : ILoggerService
    {
        public void Log(string message)
        {
            Console.WriteLine($"[LOG] {DateTime.Now:HH:mm:ss} - {message}");
        }

        public void LogInfo(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[INFO] {DateTime.Now:HH:mm:ss} - {message}");
            Console.ResetColor();
        }

        public void LogWarning(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"[WARN] {DateTime.Now:HH:mm:ss} - {message}");
            Console.ResetColor();
        }

        public void LogError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[ERROR] {DateTime.Now:HH:mm:ss} - {message}");
            Console.ResetColor();
        }
    }
}
