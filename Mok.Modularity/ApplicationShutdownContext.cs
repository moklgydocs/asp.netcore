using System;
using System.Collections.Generic;
using System.Text;

namespace Mok.Modularity
{
    public class ApplicationShutdownContext
    {
        public IServiceProvider ServiceProvider { get; set; }

        public ApplicationShutdownContext(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider)); ;
        }
    }
}
