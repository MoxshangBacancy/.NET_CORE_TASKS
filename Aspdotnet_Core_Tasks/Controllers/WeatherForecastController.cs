using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Day1;
using OpenWeather;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;
        private readonly HttpClient _httpClient;
        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
            _httpClient = new HttpClient();
        }
        //.
        /// <summary>
        /// Save Weather Data to a text file
        /// </summary>
        [HttpPost("SaveWeatherData")]
        public IActionResult SaveWeatherData([FromBody] WeatherForecast weatherData)
        {
            try
            {
                if (weatherData == null)
                {
                    return BadRequest("Invalid data.");
                }

                // Convert WeatherForecast object to JSON string
                string jsonString = JsonConvert.SerializeObject(weatherData, Formatting.Indented);

                // Save JSON string to a text file
                string filePath = "weatherData.txt";
                System.IO.File.AppendAllText(filePath, jsonString + Environment.NewLine);

                return Ok(true);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error saving WeatherForecast data: {ex.Message}");
                return BadRequest(false);
            }
        }

        /// <summary>
        /// Retrieve saved weather data from text file
        /// </summary>
        [HttpGet("GetSavedWeather")]
        public IActionResult GetSavedWeather()
        {
            try
            {
                string filePath = "weatherData.txt";
                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound("No saved weather data found.");
                }

                string getweather = System.IO.File.ReadAllText(filePath);
                return Ok(getweather);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error reading saved weather data: {ex.Message}");
                return BadRequest(false);
            }
        }

        /// <summary>
        /// Fetch real-time weather data from OpenWeather API
        /// </summary>

        [HttpGet("GetWeather/{lat}/{lon}")]
        public async Task<IActionResult> GetWeather(double lat, double lon)
        {
            if (!OpenWeather.StationDictionary.TryGetClosestStation(lat, lon, out var stationInfo))
            {
                Console.WriteLine(@"Could not find a station.");
                return NotFound("Station not found");
            }

            // Log the station info to the console
            Console.WriteLine($@"Name: {stationInfo.Name}");
            Console.WriteLine($@"ICAO: {stationInfo.ICAO}");
            Console.WriteLine($@"Lat/Lon: {stationInfo.Latitude}, {stationInfo.Longitude}");
            Console.WriteLine($@"Elevation: {stationInfo.Elevation}m");
            Console.WriteLine($@"Country: {stationInfo.Country}");
            Console.WriteLine($@"Region: {stationInfo.Region}");

            // Get the lookup URL from the station data
            string lookupUrl = "https://aviationweather.gov/cgi-bin/data/dataserver.php?requestType=retrieve&dataSource=metars&stationString=" + stationInfo.ICAO + "&hoursBeforeNow=24&format=xml";

            // Fetch weather data using the lookup URL
            using (var httpClient = new HttpClient())
            {
                try
                {
                    var response = await httpClient.GetStringAsync(lookupUrl);
                    //XDocument xmlDoc = XDocument.Parse(response);
                    //var temperatureElement = xmlDoc.Descendants("temp_c").FirstOrDefault();
                    //double? temperature = temperatureElement != null ? double.Parse(temperatureElement.Value) : (double?)null;

                    // Log the raw XML response for debugging
                    Console.WriteLine("Raw XML Response:");
                    Console.WriteLine(response);

                    // Extract relevant data directly from the API's raw response
                    var rawData = new
                    {
                        Latitude = stationInfo.Latitude,
                        Longitude = stationInfo.Longitude,
                        Country = stationInfo.Country,
                        Region = stationInfo.Region,
                        // Temperature = temperature
                    };

                    // Log the raw data to the console
                    Console.WriteLine($"Raw Data:, {rawData.Latitude}, {rawData.Longitude},{rawData.Country}, {rawData.Region}");  //,Temp: { rawData.Temperature}°C

                    // Return the raw data to the client
                    return Ok(rawData);  // Return the raw data
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error fetching weather data: {ex.Message}");
                    return StatusCode(500, "Internal server error while fetching weather data.");
                }
            }
        }



    }
}
