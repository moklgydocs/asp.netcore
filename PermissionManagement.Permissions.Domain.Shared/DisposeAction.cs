using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MokPermissions.Domain.Shared
{
    /// <summary>
    /// 用于资源释放的Action包装类
    /// </summary>
    public class DisposeAction : IDisposable
    {
        private readonly Action _action;

        public DisposeAction(Action action)
        {
            _action = action;
        }

        public void Dispose()
        {
            _action();
        }
    }
}
