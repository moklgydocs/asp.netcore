using System;
using System.Collections.Generic;

namespace CustomAspNetCore
{
    /// <summary>
    /// 配置接口 - 管理应用程序配置
    /// </summary>
    public interface IConfiguration
    {
        string GetValue(string key);
        void SetValue(string key, string value);
    }
    
    /// <summary>
    /// 配置实现 - 简单的键值对配置系统
    /// 模拟ASP.NET Core的IConfiguration
    /// </summary>
    public class Configuration : IConfiguration
    {
        private readonly Dictionary<string, string> _values = new();
        
        public Configuration()
        {
            // 默认配置值
            _values["Environment"] = "Development";
            _values["ApplicationName"] = "Custom ASP.NET Core";
            _values["Version"] = "1.0.0";
        }
        
        public string GetValue(string key)
        {
            return _values.TryGetValue(key, out var value) ? value : null;
        }
        
        public void SetValue(string key, string value)
        {
            _values[key] = value;
        }
    }
}