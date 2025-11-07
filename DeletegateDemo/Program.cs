
using DeletegateDemo.PublisherSubscriber;

namespace DeletegateDemo
{


    internal class Program
    {
        static void Main(string[] args)
        {
            // 1. 创建发布者（邮件服务器）
            var emailServer = new EmailServer();

            // 2. 创建订阅者
            Server server = new Server();
            Client client = new Client();

            // 3. 先订阅事件（关键：必须在发送邮件前订阅）
            emailServer.SendEmailHandler += server.OnSendBefore; // 服务器订阅
            emailServer.SendEmailHandler += client.OnReceiveBefore; // 客户端订阅

            // 4. 发送第一封邮件（此时已订阅，会触发处理方法）
            Email email1 = new Email()
            {
                EmailAddress = "1341564815@qq.com",
                Content = "测试邮件"
            };
            emailServer.SendEmail(email1);

            // 5. 发送第二封邮件（已订阅，直接触发）
            Email email2 = new Email()
            {
                EmailAddress = "1341564815@qq.com",
                Content = "测试邮件色情"
            };
            emailServer.SendEmail(email2);

            // （可选）取消订阅（比如客户端不需要再接收）
            emailServer.SendEmailHandler -= client.OnReceiveBefore;
            Console.WriteLine("=== 客户端已取消订阅 ===");

            // 6. 发送第三封邮件（此时只有服务器会处理）
            Email email3 = new Email()
            {
                EmailAddress = "1341564815@qq.com",
                Content = "最后一封测试邮件"
            };
            emailServer.SendEmail(email3);
        }


    }
}
