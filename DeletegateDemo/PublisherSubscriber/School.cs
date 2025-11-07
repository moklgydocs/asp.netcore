using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeletegateDemo.PublisherSubscriber
{
    // 订阅者2：学校（收到预警后决定是否停课）
    public class School
    {
        private string _schoolName;

        public School(string schoolName)
        {
            _schoolName = schoolName;
        }

        // 处理预警的方法
        public void OnAlertReceived(object sender, WeatherAlert alert)
        {
            Console.WriteLine($"学校【{_schoolName}】通知：");
            if (alert.AlertType == "暴雨红色预警")
            {
                Console.WriteLine("因暴雨红色预警，明日停课一天！");
            }
            else
            {
                Console.WriteLine($"收到{alert.AlertType}，正常上课，请带好雨具。");
            }
            Console.WriteLine($"预警时间：{alert.AlertTime:HH:mm:ss}\n");
        }
    }
}
