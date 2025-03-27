namespace WebApi.Services
{
    public interface IWeatherForcastService
    {
        string[] GetSummaries();
    }
    public class WeatherForcastService : IWeatherForcastService
    {
        public string[] GetSummaries()
        {
            return
            [
                "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
            ];
        }
    }
}
