using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInject.Business
{
    public interface IUserService
    {
        public void RegisterUser(string username);
    }
}
