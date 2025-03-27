using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Services;

namespace WebApi.Api.WeatherForecast.Integration.v1
{
    [ApiController]
    [Route("api/v1/integration/weatherforcasts")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IWeatherForcastService _weatherForcastService;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IWeatherForcastService weatherForcastService)
        {
            _logger = logger;
            _weatherForcastService = weatherForcastService;
        }

        /// <summary>
        /// Accepting Bearer authentication scheme
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var summaries = _weatherForcastService.GetSummaries();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = summaries[Random.Shared.Next(summaries.Length)]
            })
            .ToArray();
        }
    }
}
