using Abstractions;

namespace Services
{
    public interface IValuesService : IScopedDependencyResolver
    {
        int Age();
    }
}
