using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Simbirsoft_Weather.Models;

namespace Simbirsoft_Weather
{
    public class Program
    {
        public   static void Main(string[] args)
        {
            var test = new Wheather();
            test.retrycount = 3;
           var result=  test.Get_wheather("hjjkkl", "Budapest");
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }
}