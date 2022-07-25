namespace Services
{
    public class UserService : IUserService
    {
        public string Hello()
        {
            return "Hello";
        }

        public string Name()
        {
            return "Peter";
        }
    }
}
