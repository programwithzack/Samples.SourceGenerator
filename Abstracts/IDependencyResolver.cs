namespace Abstractions
{
    public interface IDependencyResolver { }

    public interface ISingletonDependencyResolver : IDependencyResolver { }

    public interface ITransientDependencyResolver : IDependencyResolver { }

    public interface IScopedDependencyResolver : IDependencyResolver { }

}
