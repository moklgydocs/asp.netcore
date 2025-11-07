using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBusDemo
{
    // 具体事件1：邮件发送事件
    public class EmailSentEvent : BaseEvent
    {
        public string EmailAddress { get; set; }

        public string Content { get; set; }
    }
}
