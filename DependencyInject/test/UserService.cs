using DependencyInject.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInject.Business
{
    public class UserService: IUserService
    {
        private readonly IUserRepository _repository;

        public UserService(IUserRepository repository)
        {
            _repository = repository;
        }

        [Inject]
        public ILogger Logger { get; set; }

        public void RegisterUser(string username)
        {
            Logger?.Log($"Registering user: {username}");
            _repository.SaveUser(username);
        }
    }
}
