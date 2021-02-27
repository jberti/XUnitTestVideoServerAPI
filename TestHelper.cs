using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using VideoServerAPI.DTO.Server;

namespace XUnitTestVideoServerAPI
{
    static class TestHelper
    {
        public static IWebHostBuilder GetServerBuilder()
        {
            IWebHostBuilder serverBuilder = new WebHostBuilder()
             .UseStartup<VideoServerAPI.Startup>()
             .UseKestrel(options => options.Listen(IPAddress.Any, 44333))
             .UseConfiguration(new ConfigurationBuilder()
              .AddJsonFile("appsettings.json")
              .Build()
             );

            return serverBuilder;
        }

        public static ServerDTO GetNewServerDTO(string name, string ip, int port)
        {
            var server = new ServerDTO()
            {
                Name = name,
                Ip = ip,
                Port = port
            };
            return server;
        }

        public static StringContent ObjectToStringContent(object obj)
        {
            var content = JsonConvert.SerializeObject(obj);
            return new StringContent(content, Encoding.UTF8, "application/json");
        }

    }
}
