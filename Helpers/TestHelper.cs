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
using VideoServerAPI.DTO.Video;

namespace XUnitTestVideoServerAPI
{
    struct ServerVideoGuid
    {
        public Guid ServerId { get; set; }
        public Guid VideoId { get; set; }
    }
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

        public static VideoDTO GetNewVideoDTO(string description, string videoDataBase64)
        {
            var video = new VideoDTO()
            {
                Description = description,
                VideoDataBase64 = videoDataBase64
            };
            return video;
        }

        
    }
}
