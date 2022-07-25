using Abstractions;
using Models;

namespace Services
{
    public interface IDataService : IDependencyResolver
    {
        IEnumerable<WeatherForecast> WeatherForecast(int days);
    }
}
