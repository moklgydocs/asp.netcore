using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInject.Business
{
    public class UserRepository : IUserRepository, IDisposable
    {
        private readonly ILogger _logger;

        public UserRepository(ILogger logger)
        {
            _logger = logger;
            _logger.Log("UserRepository created");
        }

        public void SaveUser(string username)
        {
            _logger.Log($"User '{username}' saved");
        }

        public void Dispose()
        {
            Console.WriteLine("UserRepository disposed");
        }
    }
}
