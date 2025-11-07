using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBusDemo
{
    // 事件总线：管理所有事件的订阅和发布
    public class EventBus
    {
        // 存储订阅关系：Key=事件类型，Value=该事件的所有处理方法（委托）
        // 用ConcurrentDictionary保证线程安全（多线程场景下）
        private readonly ConcurrentDictionary<Type, List<Delegate>> _subscribers = new();

        public void Subscribe<T>(Action<T> handler) where T : BaseEvent
        {
            Type eventType = typeof(T);
            // 若事件类型未注册，先初始化一个空列表
            _subscribers.TryAdd(eventType, new List<Delegate>());
        }
    }
}
