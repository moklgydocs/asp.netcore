using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MokPermissions.Domain.Shared
{
    /// <summary>
    /// 内存中的事件发布器实现
    /// </summary>
    public class InMemoryEventPublisher : IEventPublisher
    {
        private readonly IServiceProvider _serviceProvider;

        public InMemoryEventPublisher(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task PublishAsync<TEvent>(TEvent eventData)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var handlers = scope.ServiceProvider.GetServices<IEventHandler<TEvent>>();

                foreach (var handler in handlers)
                {
                    await handler.HandleAsync(eventData);
                }
            }
        }
    }
}
