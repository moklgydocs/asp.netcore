using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeletegateDemo.PublisherSubscriber
{
    // 订阅者2：旁观者（收到鱼咬钩事件后欢呼）
    public class Bystander
    {
        private string _name;

        public Bystander(string name)
        {
            _name = name;
        }

        // 处理鱼咬钩的方法
        public void HandleFishBitten(object sender, Fish fish)
        {
            Console.WriteLine($"旁观者【{_name}】：快看！{fish.Type}！这么大一条，厉害啊！\n");
        }
    }
}
