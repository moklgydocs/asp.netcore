using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeletegateDemo.PublisherSubscriber
{
    // 委托：定义鱼咬钩事件的处理方法签名
    // 参数：sender（事件源，比如鱼竿）、fish（上钩的鱼）
    public delegate void FishBittenHandler(object sender, Fish fish);

    public class FishingMan
    {
        private string _name;

        public FishingMan(string name)
        {
            _name = name;
        }

        // 处理鱼咬钩的方法（签名必须与FishBittenHandler一致）
        public void HandleFishBitten(object sender, Fish fish)
        {
            Console.WriteLine($"钓鱼者【{_name}】：哇！有鱼上钩了！");
            Console.WriteLine($"赶紧收线！是一条{fish.WeightKg}千克的{fish.Type}！\n");
        }


    }
}
