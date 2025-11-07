using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeletegateDemo.PublisherSubscriber
{
    // 鱼竿（发布者）：负责检测鱼咬钩并触发事件
    public class FishingRod
    {
        // 声明“鱼咬钩事件”（基于上面定义的委托）
        public event FishBittenHandler FishBitten;

        // 模拟鱼咬钩的方法（内部触发事件）
        public void OnFishBitten(Fish fish)
        {
            Console.WriteLine($"=== 鱼竿检测到鱼咬钩！ ===");
            // 触发事件：通知所有订阅者（先判断是否有订阅者，避免空引用）
            FishBitten?.Invoke(this, fish);
        }
    }
}
