using System.Runtime.CompilerServices;

namespace Mok.AspNetCore
{
    public static class MokConfigurationExtensions
    {
        // 配置对象缓存键
        private const string ConfigurationCacheKey = "MokModuleConfiguration";

        // 注册配置对象到服务集合
        public static IServiceCollection AddConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton(configuration);
            services.AddSingleton<IConfiguration>(configuration);

            // 使用属性袋存储配置对象以便在模块初始化过程中访问
            services.SetProperty(ConfigurationCacheKey, configuration);

            return services;
        }

        // 从服务集合获取配置对象
        public static IConfiguration GetConfiguration(this IServiceCollection services)
        {
            // 首先尝试从属性袋获取
            if (services.TryGetProperty(ConfigurationCacheKey, out object configuration))
            {
                return configuration as IConfiguration;
            }

            // 回退：尝试从服务集合解析
            var provider = services.BuildServiceProvider();
            return provider.GetService<IConfiguration>();
        }

        // 属性袋扩展方法
        private static readonly ConditionalWeakTable<IServiceCollection, Dictionary<string, object>> Properties =
            new ConditionalWeakTable<IServiceCollection, Dictionary<string, object>>();

        public static void SetProperty(this IServiceCollection services, string key, object value)
        {
            var properties = Properties.GetOrCreateValue(services);
            properties[key] = value;
        }

        public static bool TryGetProperty(this IServiceCollection services, string key, out object value)
        {
            if (Properties.TryGetValue(services, out var properties))
            {
                return properties.TryGetValue(key, out value);
            }

            value = null;
            return false;
        }

    }
}
