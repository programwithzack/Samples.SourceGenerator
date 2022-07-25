using Abstractions;

namespace Services
{
    public interface IValuesService : IDependencyResolver
    {
        int Age();
    }
}
