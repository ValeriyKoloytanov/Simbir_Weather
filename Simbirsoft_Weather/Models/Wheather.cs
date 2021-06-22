using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Simbirsoft_Weather.Models
{
    public class Wheather
    {       private  int _count;

        public int retrycount
        {
            get
            {
                return _count;
            }
            set { _count = value; }
        }

        public  async Task<Dictionary<string, Wheather>>  Get_wheather(string date, string city)
     {
         string url =   $"https://api.openweathermap.org/data/2.5/weather?q={city}&appid=922b05ce49a15397ded5a54a17cad16d&units=metric";
         HttpClient client = new HttpClient();
         for(int tries = 0; tries < retrycount; tries++) {
             try {
                HttpResponseMessage response = client.GetAsync(url).Result;
                         HttpContent content = response.Content;    
                         var json = content.ReadAsStringAsync().Result;
                        var success = response.IsSuccessStatusCode;
                         if (success)
                             break;

             } catch(HttpRequestException  e) 
             {
                 Console.WriteLine("Ошибка");
             }
         }
         
     return null;
     }
    }
}