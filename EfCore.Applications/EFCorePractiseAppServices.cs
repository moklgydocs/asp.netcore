using EfCore.Application.Contracts;
using EfCore.Application.Contracts.Dtos;
using EfCore.Domain.Entitys;
using EfCore.Pgsql;
using Mapster;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace EfCore.Applications
{
    public class EFCorePractiseAppServices : IEFCorePractiseAppServices
    {
        private PgDbContext _dbContext;
        public EFCorePractiseAppServices(PgDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<List<CustomerDto>> PgSql_SelectDemo()
        {
            var customersQuery = await _dbContext.Customers
                .Select(o => new { o.FirstName, o.LastName, o.Active, o.CustomerId, o.AddressId, o.CreateDate, o.LastUpdate }).ToListAsync();
            var customers = customersQuery
            .Select((o, index) => new CustomerDto
            {
                FirstName = o.FirstName,
                LastName = o.LastName,
                Active = o.Active,
                CustomerId = o.CustomerId,
                AddressId = o.AddressId,
                Index = index + 1,
                LastUpdate = o.AddressId < 30 ? DateTime.Now.AddDays(-1) : DateTime.Now
            })
            .Skip(1)
            .Take(30)
            .ToList();
            //var data = customers.Adapt<List<CustomerDto>>();
            var actors = await _dbContext.Actors.AllAsync(x => x.FirstName.Contains("A"));
            return customers;
        }

        public async Task SetDemo()
        {
            // Union - 合并两个集合并去除重复项
            var categoryNames1 = new[] { "电子产品", "配件", "办公用品" };
            var categoryNames2 = new[] { "配件", "图书", "家居" };
            var allCategories = categoryNames1.Union(categoryNames2);
            // 结果: "电子产品", "配件", "办公用品", "图书", "家居"

            // Intersect - 返回两个集合的交集
            var commonCategories = categoryNames1.Intersect(categoryNames2);
            // 结果: "配件"

            // Except - 返回在第一个集合但不在第二个集合的元素
            var exclusiveCategories = categoryNames1.Except(categoryNames2);
            // 结果: "电子产品", "办公用品"

            // Concat - 简单合并两个集合(不去除重复项)
            var allCategoriesDuplicates = categoryNames1.Concat(categoryNames2);
            // 结果: "电子产品", "配件", "办公用品", "配件", "图书", "家居"

            // SequenceEqual - 检查两个集合是否包含相同的元素，顺序也相同
            bool areEqual = new[] { 1, 2, 3 }.SequenceEqual(new[] { 1, 2, 3 }); // true
            bool areNotEqual = new[] { 1, 2, 3 }.SequenceEqual(new[] { 3, 2, 1 }); // false
        }

        public async Task<List<MonthDayDto>> Partition()
        {
            int[] numbers1 = { 1, 2, 3, 4 };
            string[] words = { "one", "two", "three" };

            var numbersAndWords = numbers1.Zip(words, (first, second) => first + " " + second);

            foreach (var item in numbersAndWords)
                Console.WriteLine(item);

            // Range - 生成一系列数字
            var numbers = Enumerable.Range(1, 10); // 1到10的数字

            // Repeat - 生成重复元素的序列
            var fiveAs = Enumerable.Repeat('A', 5); // 五个'A'字符

            // Empty - 创建空集合
            var emptyIntList = Enumerable.Empty<int>(); // 空的整数集合

            // 创建初始化集合
            var weekDays = Enumerable.Range(1, 7)
                .Select(day => new
                {
                    DayNumber = day,
                    Name = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat
                        .GetDayName((DayOfWeek)((day - 1) % 7))
                });

            // 生成随机数据集合
            Random rand = new Random(123); // 使用固定种子以获得可重现的结果
            var randomPrices = Enumerable.Range(1, 10)
                .Select(_ => Math.Round(rand.NextDouble() * 1000, 2));

            // 生成日期范围
            var dateRange = Enumerable.Range(0, 30)
                .Select(offset => DateTime.Today.AddDays(offset))
                .Select(date => new MonthDayDto
                {
                    Date = date,
                    DayOfWeek = date.DayOfWeek.ToString(),
                    IsWeekend = date.DayOfWeek == DayOfWeek.Saturday ||
                               date.DayOfWeek == DayOfWeek.Sunday
                });
            return await Task.FromResult(dateRange.ToList());
        }

    }
}
