using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeletegateDemo
{
    /// <summary>
    /// 事件源
    /// </summary>
    public class Email
    {
        public string EmailAddress { get; set; }
        public string EmailFrom { get; set; }
        public string EmailTo { get; set; }
        public string Content { get; set; }

        public DateTime Date { get; set; }

    }
}
