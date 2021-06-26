using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json.Serialization;
using JsonSerializer = System.Text.Json.JsonSerializer;
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Simbirsoft_Weather.Models
{
    public class WheatherApi
    {
        private OpenWeatherForecast Root { get; set; }

        public WheatherApi(string city)
        { string  url=$"https://api.openweathermap.org/data/2.5/forecast?q={city}&appid=922b05ce49a15397ded5a54a17cad16d&cnt=50&lang=ru&units=metric";

            Root = json_to_list(Get_wheather_5d_json(url));
        }

        public WheatherApi(double lat, double lon)
        {      string  url=$"https://api.openweathermap.org/data/2.5/forecast?lat={lat}&lon={lon}&appid=922b05ce49a15397ded5a54a17cad16d&cnt=50&lang=ru&units=metric";
            Root = json_to_list(Get_wheather_5d_json(url));

        }


        public int Retrycount { get; set; } = 3;

        public class Main
        {
            [JsonPropertyName("temp")] public double Temp { get; set; }
            [JsonPropertyName("feels_like")] public double FeelsLike { get; set; }
            [JsonPropertyName("temp_min")] public double TempMin { get; set; }
            [JsonPropertyName("temp_max")] public double TempMax { get; set; }
            [JsonPropertyName("pressure")] public int Pressure { get; set; }
            [JsonPropertyName("humidity")] public int Humidity { get; set; }
        }

        public class WeatherDescription
        {  [JsonPropertyName("description")]
            public string Description { get; set; }
            [JsonPropertyName("icon")]
            public string Icon { get; set; }

        }

        public class Wind
        {
            [JsonPropertyName("speed")] public double Speed { get; set; }
            [JsonPropertyName("deg")] public int Deg { get; set; }
        }

        public class ForecastData
        {
            [JsonPropertyName("main")] public Main GeneralProperities { get; set; }
            [JsonPropertyName("wind")] public Wind Wind { get; set; }
            [JsonPropertyName("weather")] public List<WeatherDescription> WeatherDescriptions { get; set; }
            [JsonPropertyName("dt_txt")] public string DtTxt { get; set; }
        }

        public class OpenWeatherForecast
        {
            [JsonPropertyName("list")] public IList<ForecastData> ForecastList { get; set; }
        }

        private string Get_wheather_5d_json(string url)
        { HttpClient client = new HttpClient();
            string json = "";
            for (int tries = 0; tries < Retrycount; tries++)
            {
                try
                {
                    HttpResponseMessage response = client.GetAsync(url).Result;
                    HttpContent content = response.Content;
                    json = content.ReadAsStringAsync().Result;
                    var success = response.IsSuccessStatusCode;
                    if (success)
                        break;
                }
                catch (HttpRequestException e)
                {
                    throw new Exception(e.Message);
                }
            }

            return json;
        }

        private OpenWeatherForecast json_to_list(string json)
        {
            var forecastList = JsonSerializer.Deserialize<OpenWeatherForecast>(json);
            return forecastList;
        }

        public Dictionary<string, ForecastData> WheatherForTime(string date)
        {
            var wheather = Root.ForecastList
                .Where(forecastData => Convert.ToDateTime(forecastData.DtTxt).Day == Convert.ToDateTime(date).Day)
                .ToDictionary(x => x.DtTxt, x => x);
            return wheather;
        }


        public class ForecastView
        {
            public double Mintemp { get; set; }
            public double Maxtemp { get; set; }
            public string Main { get; set; }
            public DateTime Date { get; set; }
            public string Icon { get; set; }
            public double SpeedWind { get; set; }
        }

        public List<ForecastView> WheatherFor5Day()
        {
            DateTime day = DateTime.Today;
            Dictionary<int, Dictionary<string, ForecastData>> wheatherDict =
                new Dictionary<int, Dictionary<string, ForecastData>>();
            for (int i = 0; i <= 4; i++)
            {
                wheatherDict.Add(i, WheatherForTime(day.ToString()));
                day = day.AddDays(1);
            }

            List<ForecastView> result = new List<ForecastView>();
            foreach (var k in wheatherDict)
            {
                var dates = k.Value.Keys.ToList();
                var forecastData = k.Value.Values.ToList();

                ForecastView dayWheather = new ForecastView();
                dayWheather.Date = Convert.ToDateTime(dates[0]).Date;
                dayWheather.Mintemp = forecastData.Select(data => data.GeneralProperities.TempMin).Min();
                dayWheather.Maxtemp = forecastData.Select(data => data.GeneralProperities.TempMin).Max();
                dayWheather.Main = forecastData[0].WeatherDescriptions[0].Description;
                dayWheather.Icon = forecastData[0].WeatherDescriptions[0].Icon;
                dayWheather.SpeedWind = forecastData.Select(data => data.Wind.Speed).Average();
                result.Add(dayWheather);
            }

            return result;
        }
    }
}