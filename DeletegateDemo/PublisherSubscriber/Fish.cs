using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeletegateDemo.PublisherSubscriber
{
    // 鱼的信息（事件传递的消息）
    public class Fish
    {
        public string Type { get; set; } // 鱼的类型（如鲫鱼、鲈鱼）
        public double WeightKg { get; set; } // 重量（千克）
        public DateTime BiteTime { get; set; } // 咬钩时间
    }
}
