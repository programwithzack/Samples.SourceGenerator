using Microsoft.AspNetCore.Mvc;
using Models;
using Services;

namespace WebApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IDataService _dataService;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IDataService dataService)
        {
            _logger = logger;
            _dataService = dataService;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get([FromQuery] int days)
        {
            return _dataService.WeatherForecast(days);
        }
    }
}