using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeletegateDemo
{
    public delegate void SendEmailHandler(object sender, Email email);
    /// <summary>
    /// 发布者 邮件服务器
    /// </summary>
    public class EmailServer
    {
        public event SendEmailHandler SendEmailHandler;

        public void SendEmail(Email email)
        {
            if (email.Content.Contains("色")) {
                Console.WriteLine("[服务器]检测到邮件内容非法,不允许发送");
                return;
            }
            Console.WriteLine($"[服务器] === 开始发送邮件到 {email.EmailAddress} ===");

            // 触发事件：通知所有订阅者（必须在类内部触发）
            // 先判断是否有订阅者，避免空引用异常
            SendEmailHandler?.Invoke(this, email);

            Console.WriteLine("[服务器]邮件发送流程结束\n");
        }
    }
}
