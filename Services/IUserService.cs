using Abstractions;

namespace Services
{
    public interface IUserService : ISingletonDependencyResolver
    {
        string Hello();

        string Name();
    }
}
