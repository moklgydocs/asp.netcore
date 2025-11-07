using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeletegateDemo
{
    public class Server
    {
        /// <summary>
        /// 检查是否允许发送邮件
        /// </summary>
        public void OnSendBefore(object sender, Email email)
        {
            if (email != null && email.Content.Contains("色"))
            {
                Console.WriteLine("检测到邮件内容非法,不允许发送");
            }
            email.EmailFrom = "127.0.0.1";
            email.EmailTo = "localhost";
            email.Date = DateTime.Now;
            Console.WriteLine("合法邮件");
            return;
        }
    }
}
