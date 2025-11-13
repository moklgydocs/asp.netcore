using MokAbp.DependencyInjection.Attributes;

namespace MokAbp.Demo.Services
{
    /// <summary>
    /// 数据服务接口
    /// </summary>
    public interface IDataService
    {
        void SaveData(string key, string value);
        string GetData(string key);
    }

    /// <summary>
    /// 内存数据服务实现
    /// </summary>
    [ScopedDependency]
    [ExposeServices(typeof(IDataService))]
    public class InMemoryDataService : IDataService
    {
        private readonly Dictionary<string, string> _data = new();
        private readonly ILoggerService _logger;

        public InMemoryDataService(ILoggerService logger)
        {
            _logger = logger;
        }

        public void SaveData(string key, string value)
        {
            _data[key] = value;
            _logger.LogInfo($"数据已保存: {key} = {value}");
        }

        public string GetData(string key)
        {
            if (_data.TryGetValue(key, out var value))
            {
                _logger.LogInfo($"数据已读取: {key} = {value}");
                return value;
            }

            _logger.LogWarning($"数据不存在: {key}");
            return null!;
        }
    }
}
