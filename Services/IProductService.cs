using Abstractions;

namespace Services
{
    public interface IProductService : IDependencyResolver
    {
        string GetProductName();
    }
}
