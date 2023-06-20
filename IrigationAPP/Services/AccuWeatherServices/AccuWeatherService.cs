using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using IrigationAPP.Data;
using IrigationAPP.Models;
using System;
using System.Collections.Generic;

namespace IrigationAPP.AccuWeatherServices
{
    public class AccuWeatherService
    {
        private readonly HttpClient _httpClient;
        private readonly ApplicationDbContext _context; 
        private const string ApiKey = "DZRLn5QXR0Gjw2VsxD3GqMHGr9tsA1O5"; // Replace with your actual API key
        private const string LocationKey = "275692"; // Replace with your actual location key

       /* public AccuWeatherService(ApplicationDbContext context, HttpClient httpClient)
        {
            _context = context;
            _httpClient = httpClient;
        }*/
        public AccuWeatherService(ApplicationDbContext context, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _httpClient = httpClientFactory.CreateClient();
        }

        public async Task FetchDataAsync()
        {
            /*var response = await _httpClient.GetAsync($"http://dataservice.accuweather.com/currentconditions/v1/{LocationKey}?apikey={ApiKey}&details=true");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<List<CurrentCondition>>(content); // You need to create the CurrentCondition class to match the API response structure

            var temperature = data[0].Temperature.Imperial.Value;
            var rainProbability = data[0].PrecipitationSummary.Precipitation.Probability;

            var dataReadAccuWeather = new DataReadAccuWeather // Change this to your model class name
            {
                temperature = (int)temperature,
                rainProbability = rainProbability,
                time = DateTime.UtcNow
            };

            _context.DataReadAccuWeather.Add(dataReadAccuWeather); // Change this to your DbSet name
            await _context.SaveChangesAsync();*/
            var response = await _httpClient.GetAsync($"http://dataservice.accuweather.com/currentconditions/v1/{LocationKey}?apikey={ApiKey}&details=true");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<List<CurrentCondition>>(content);

            var temperature = data[0].Temperature.Metric.Value;
            var rainProbability = data[0].HasPrecipitation ? 1 : 0;

            var dataReadAccuWeather = new DataReadAccuWeather
            {
                temperature = (int)temperature,
                rainProbability = rainProbability,
                time = DateTime.UtcNow
            };

            _context.DataReadAccuWeather.Add(dataReadAccuWeather);
            await _context.SaveChangesAsync();
        }
       
        
    }
}
