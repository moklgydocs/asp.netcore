namespace MiddleWare
{
    public class UserService : IUserService
    {
        /// <summary>
        /// 这个写法完全错误，这个是方法行为，并不受生命周期的控制
        /// </summary>
        /// <returns></returns>
        public string PrintLifeTime()
        {
            return Guid.NewGuid().ToString();
            Console.WriteLine($"[UserService] {Guid.NewGuid()}");
        }
        public Guid ID { get;  }
        public UserService()
        {
            ID = Guid.NewGuid();
        }
        public string PrintLifeTimeHasLife() => ID.ToString();
        public string PrintLifeTimeHasScopeLife() => ID.ToString();

    }

}
