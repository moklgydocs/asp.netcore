using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBusDemo
{
    // 事件基类（所有事件都继承它，便于统一处理）
    public abstract class BaseEvent
    {
        public DateTime EventTime { get; } = DateTime.Now; // 事件发生时间
    }

}
