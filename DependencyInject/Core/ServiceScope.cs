using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInject.Core
{
    public class ServiceScope : IServiceScope
    {
        private readonly DIContainer _container;

        public ServiceScope(DIContainer container)
        {
            _container = container;
            ServiceProvider = container;
        }

        public IServiceProvider ServiceProvider { get; }

        public void Dispose()
        {
            _container.Dispose();
        }
    }
}
