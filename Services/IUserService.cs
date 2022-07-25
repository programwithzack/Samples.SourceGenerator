using Abstractions;

namespace Services
{
    public interface IUserService : IDependencyResolver
    {
        string Hello();

        string Name();
    }
}
