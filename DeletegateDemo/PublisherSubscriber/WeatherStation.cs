using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeletegateDemo.PublisherSubscriber
{
    // 发布者：气象局
    public class WeatherStation
    {

        // 自定义委托：用于发布订阅的消息传递
        // 签名：接收发布者对象（sender）和消息（WeatherAlert）
        public delegate void WeatherAlertHandler(object sender, WeatherAlert alert);

        // 声明事件： 基于自定义的WeatherAlertHandler委托
        public event WeatherAlertHandler AlertPublished;

        // 发布预警的方法(触发事件)
        public void PublishAlert(string type, string desctiption)
        {
            // 构建消息
            var alert = new WeatherAlert()
            {
                AlertDescription = desctiption,
                AlertType = type,
                AlertTime = DateTime.Now,
            };

            // 触发事件(通知所有订阅者)
            // 先判断是否有订阅者，避免空引用异常
            AlertPublished?.Invoke(this, alert);
        }
    }
}
