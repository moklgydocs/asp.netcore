using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBusDemo
{
    // 具体事件2：用户注册事件
    public class UserRegisteredEvent : BaseEvent
    {
        public int UserId { get; set; }
        public string Username { get; set; }
    }
}
