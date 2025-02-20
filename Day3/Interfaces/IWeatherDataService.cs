// IWeatherDataService.cs
using Day3;
using Newtonsoft.Json;
public interface IWeatherDataService
{
    void SaveWeatherData(WeatherForecast weatherData);
    string GetSavedWeatherData();
    string GetLogData(); // New method for reading log data
}


// WeatherDataService.cs
public class WeatherDataService : IWeatherDataService
{
    private readonly string _filePath = "weatherData.txt";
    private readonly string _logFilePath = "requestLogs.txt"; // New log file

    public void SaveWeatherData(WeatherForecast weatherData)
    {
        string jsonString = JsonConvert.SerializeObject(weatherData, Formatting.Indented);
        System.IO.File.AppendAllText(_filePath, jsonString + Environment.NewLine);

        // Log the request
        string logEntry = $"[{DateTime.UtcNow}] Saved weather data: {jsonString}";
        System.IO.File.AppendAllText(_logFilePath, logEntry + Environment.NewLine);
    }

    public string GetSavedWeatherData()
    {
        return System.IO.File.Exists(_filePath) ? System.IO.File.ReadAllText(_filePath) : null;
    }

    public string GetLogData()
    {
        return System.IO.File.Exists(_logFilePath) ? System.IO.File.ReadAllText(_logFilePath) : null;
    }
}
