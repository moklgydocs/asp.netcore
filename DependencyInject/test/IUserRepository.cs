using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInject.Business
{
    public interface IUserRepository
    {
        void SaveUser(string username);
    }
}
