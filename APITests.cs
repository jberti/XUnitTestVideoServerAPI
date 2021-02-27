
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using VideoServerAPI;
using System.Net;
using Microsoft.Extensions.Configuration;
using VideoServerAPI.DTO.Server;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace XUnitTestVideoServerAPI
{
    public class ServersControllerTest
    {
        private Guid _serverId;
        private readonly HttpClient _client;

        public ServersControllerTest()
        {
            var server = new TestServer(TestHelper.GetServerBuilder());
            _client = server.CreateClient();
        }


        [Theory]
        [InlineData("GET")]
        public async Task ListServers(string method)
        {
            var request = new HttpRequestMessage(new HttpMethod(method), "/api/servers/");
            var response = await _client.SendAsync(request);
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task AddServerTest()
        {
            var serverDTOResult = await AddServer("teste", "127.0.0.1", 80);

            _serverId = (Guid)serverDTOResult.ServerId;

            Assert.Equal("teste", serverDTOResult.Name);
        }

        [Fact]
        public async Task GetServerInfo()
        {
            Guid id = new Guid("b2b79d56-c11f-4bf1-a5fd-950c8568554f");
            var request = new HttpRequestMessage(new HttpMethod("GET"), "/api/servers/" + id);
            var response = await _client.SendAsync(request);
            Task<string> responseMessage = response.Content.ReadAsStringAsync();
            var serverDTOResult = JsonConvert.DeserializeObject<ServerDTO>(responseMessage.Result);

            Assert.Equal(id, serverDTOResult.ServerId);
        }

        [Fact]
        public async Task DeleteServer()
        {
            var serverDTOResult = await AddServer("testedeleteserver", "127.0.0.99", 8080);

            var request = new HttpRequestMessage(new HttpMethod("DELETE"), "/api/servers/" + serverDTOResult.ServerId);
            var response = await _client.SendAsync(request);

            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        }

        private async Task<ServerDTO> AddServer(string name, string ip, int port)
        {
            var request = new HttpRequestMessage(new HttpMethod("POST"), "/api/servers/");

            var server = TestHelper.GetNewServerDTO(name, ip, port);

            var body = TestHelper.ObjectToStringContent(server);


            var response = await _client.PostAsync(request.RequestUri, body);
            Task<string> responseMessage = response.Content.ReadAsStringAsync();

            var serverDTOResult = JsonConvert.DeserializeObject<ServerDTO>(responseMessage.Result);

            return serverDTOResult;
        }
    }
}
