using Abstractions;
using Models;

namespace Services
{
    public interface IDataService : ITransientDependencyResolver
    {
        IEnumerable<WeatherForecast> WeatherForecast(int days);
    }
}
