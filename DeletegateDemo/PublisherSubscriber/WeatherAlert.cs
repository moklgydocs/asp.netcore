using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeletegateDemo.PublisherSubscriber
{
    /// <summary>
    /// 天气预报
    /// </summary>
    public class WeatherAlert
    {
        public string AlertType { get; set; }

        public string AlertDescription { get; set; }

        public DateTime AlertTime { get; set; }
    }
}
