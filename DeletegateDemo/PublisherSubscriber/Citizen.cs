using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeletegateDemo.PublisherSubscriber
{
    // 订阅者1：市民（收到预警后准备应对）
    public class Citizen
    {
        private string _name;

        public Citizen(string name)
        {
            _name = name;
        }

        // 处理预警的方法（签名必须与WeatherAlertHandler一致）
        public void OnAlertReceived(object sender, WeatherAlert alert)
        {
            Console.WriteLine($"市民【{_name}】收到天气预警：");
            Console.WriteLine($"类型：{alert.AlertType}");
            Console.WriteLine($"描述：{alert.AlertDescription}");
            Console.WriteLine($"时间：{alert.AlertTime:HH:mm:ss}\n");
        }
    }
}
