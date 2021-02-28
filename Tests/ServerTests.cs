
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using VideoServerAPI;
using VideoServerAPI.DTO.Server;
using Newtonsoft.Json;
using VideoServerAPI.DTO.Video;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace XUnitTestVideoServerAPI
{
    public class ServersControllerTest : IDisposable
    {
        
        private readonly HttpClient _client;
        private bool disposedValue;

       
        public ServersControllerTest()
        {
            
            var server = new TestServer(TestHelper.GetServerBuilder());
            var DbContext = server.Services.GetService<VideoServerAPI.Data.VideoServerDbContext>();
            DbContext.ApplyMigrations();
            _client = server.CreateClient();
        }

        /// <summary>
        /// Teste simples pra provar que a leitura dos servers foi feita.
        /// </summary>
        /// <returns></returns>
        [Fact]        
        public async Task ListServers()
        {
            var server = await AddServer("testeListServers", "127.0.0.1133", 8888);

            var request = new HttpRequestMessage(new HttpMethod("GET"), "/api/servers/");
            var response = await _client.SendAsync(request);
            Task<string> responseMessage = response.Content.ReadAsStringAsync();
            var serverDTOListResult = JsonConvert.DeserializeObject<List<ServerDTO>>(responseMessage.Result);

            //Fato que existem servers cadastrados, desde que o registro no banco não tenha sido apagado, conforme orientado.
            Assert.True(serverDTOListResult.Count > 0);

            await DeleteServer((Guid)server.ServerId);
        }

        /// <summary>
        /// Prova que um server, de nome teste, foi adicionado.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task AddServerTest()
        {
            var serverDTOResult = await AddServer("teste", "127.0.0.2", 80);

            var serverId = (Guid)serverDTOResult.ServerId;

            Assert.Equal("teste", serverDTOResult.Name);

            await DeleteServer(serverId);
        }

        /// <summary>
        /// Pova de que está sendo trazido dado do server de id informado.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetServerInfoTest()
        {
            var server = await AddServer("testeListServers", "127.0.0.1133", 8888);
            
            var request = new HttpRequestMessage(new HttpMethod("GET"), $"/api/servers/{server.ServerId}");
            var response = await _client.SendAsync(request);
            Task<string> responseMessage = response.Content.ReadAsStringAsync();
            var serverDTOResult = JsonConvert.DeserializeObject<ServerDTO>(responseMessage.Result);

            Assert.Equal(server.ServerId, serverDTOResult.ServerId);

            await DeleteServer((Guid)server.ServerId);
        }
        
        /// <summary>
        /// Fato que ums ervidor foi apagado (status code 200). Senão retornaria o codigo referente a Not Found.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task DeleteServerTest()
        {
            var serverDTOResult = await AddServer("testedeleteserver", "127.0.0.99", 8080);
            
            var response = await DeleteServer((Guid)serverDTOResult.ServerId);

            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        }

    
       

        
        /// <summary>
        /// Só para provar que estou fazendo requisiçao baseado no status da lista BlockingCollection usada no controle.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task RecyclerStatusTest()
        {
            var request = new HttpRequestMessage(new HttpMethod("GET"), $"/api/recycler/status");
            
            var response = await (await _client.GetAsync(request.RequestUri)).Content.ReadAsStringAsync();

            Assert.Equal("Not Running", response);
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

        private async Task<HttpResponseMessage> DeleteServer(Guid serverId)
        {
            var request = new HttpRequestMessage(new HttpMethod("DELETE"), "/api/servers/" + serverId);
            return await _client.SendAsync(request);
        }

        


        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~ServersControllerTest()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
