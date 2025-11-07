using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeletegateDemo.PublisherSubscriber
{
    public  class CallEvent
    {
        public static void FishStory()
        {
            // 1. 创建发布者（鱼竿）
            var rod = new FishingRod();

            // 2. 创建订阅者（钓鱼者和旁观者）
            var angler = new FishingMan("老李"); // 钓鱼者老李
            var bystander1 = new Bystander("小王"); // 旁观者小王
            var bystander2 = new Bystander("小张"); // 旁观者小张

            // 3. 订阅事件（注册处理方法）
            rod.FishBitten += angler.HandleFishBitten; // 钓鱼者订阅
            rod.FishBitten += bystander1.HandleFishBitten; // 小王订阅
            rod.FishBitten += bystander2.HandleFishBitten; // 小张订阅

            // 4. 第一次鱼上钩（鲫鱼）
            var crucian = new Fish
            {
                Type = "鲫鱼",
                WeightKg = 0.8,
                BiteTime = DateTime.Now.AddMinutes(-5)
            };
            rod.OnFishBitten(crucian);

            // 5. 小张离开，取消订阅
            rod.FishBitten -= bystander2.HandleFishBitten;
            Console.WriteLine("=== 旁观者小张离开了 ===");

            // 6. 第二次鱼上钩（鲈鱼）
            var bass = new Fish
            {
                Type = "鲈鱼",
                WeightKg = 1.5,
                BiteTime = DateTime.Now
            };
            rod.OnFishBitten(bass);
        }

        public static void WeatherReport()
        {
            // 1. 创建发布者，气象局
            var weatherStation = new WeatherStation();

            // 2. 创建订阅者(市民和学校)
            var citizen = new Citizen("Jackie");
            var school = new School("光明小学");

            // 3. 订阅事件(注册处理方法)
            weatherStation.AlertPublished += citizen.OnAlertReceived;
            weatherStation.AlertPublished += school.OnAlertReceived;

            // 4. 发布第一条预警（暴雨蓝色）
            Console.WriteLine("=== 发布暴雨蓝色预警 ===");
            weatherStation.PublishAlert("暴雨蓝色预警", "未来12小时将有大雨，请注意防范");


            // 5. 取消市民的订阅
            weatherStation.AlertPublished -= citizen.OnAlertReceived;

            // 6. 发布第二条预警（暴雨红色）
            Console.WriteLine("=== 发布暴雨红色预警（市民已取消订阅） ===");
            weatherStation.PublishAlert("暴雨红色预警", "未来6小时将有特大暴雨，建议停工停课");

            // 5. 取消学校的订阅
            weatherStation.AlertPublished -= school.OnAlertReceived;
            weatherStation.AlertPublished += citizen.OnAlertReceived;

            // 6. 发布第二条预警（暴雨红色）
            Console.WriteLine("=== 发布暴雨红色预警（学校已取消订阅） ===");
            weatherStation.PublishAlert("暴雨红色预警", "未来6小时将有特大暴雨，建议居家办公");
        }
    }
}
