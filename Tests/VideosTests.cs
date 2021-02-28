using Microsoft.AspNetCore.TestHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using VideoServerAPI.DTO.Server;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using VideoServerAPI.DTO.Video;
using Newtonsoft.Json;

namespace XUnitTestVideoServerAPI
{
    public class VideosTests : IDisposable
    {
        private bool disposedValue;
        private readonly HttpClient _client;

        public VideosTests()
        {
            var server = new TestServer(TestHelper.GetServerBuilder());
            var DbContext = server.Services.GetService<VideoServerAPI.Data.VideoServerDbContext>();
            DbContext.ApplyMigrations();
            _client = server.CreateClient();
        }

        [Fact]
        public async Task VideoAddTest()
        {
            var result = await CreateServerAndVideo();
            var serverId = result.ServerId;
            var videoId = result.VideoId;

            var videoDTOInfo = await GetVideoInfo(serverId, videoId);
            //fato que um video foi adicionado.
            Assert.Equal("video de teste", videoDTOInfo.Description);

            await DeleteVideo(serverId, videoId);
            await DeleteServer(serverId);
        }

        [Fact]
        public async Task GetVideoInfoTest()
        {
            var result = await CreateServerAndVideo();
            var serverId = result.ServerId;
            var videoId = result.VideoId;

            var videoDTOInfo = await GetVideoInfo(serverId, videoId);
            //fato que as informacoes de um video foram recuperadas.
            Assert.Equal(result.VideoId, videoDTOInfo.VideoId);

            await DeleteVideo(serverId, videoId);
            await DeleteServer(serverId);
        }

        [Fact]
        public async Task GetServerVideosTest()
        {
            var result = await CreateServerAndVideo();
            var serverId = result.ServerId;
            var videoId = result.VideoId;

            var serverVideos = await GetServerVideos(serverId);
            //Fato de que videos de um servidor estão sendo trazidos.
            Assert.True(serverVideos.Count == 1);

            await DeleteVideo(serverId, videoId);
            await DeleteServer(serverId);
        }

        [Fact]
        public async Task DeleteVideoTest()
        {
            var result = await CreateServerAndVideo();
            var serverId = result.ServerId;
            var videoId = result.VideoId;


            var response = await DeleteVideo(serverId, videoId);
            //Fato que um video foi apagado.
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);

            await DeleteServer(serverId);
        }


        /// <summary>
        /// Como eu preciso de um server e pelo menos um video para os testes de video, eu fiz esse método.
        /// </summary>
        /// <returns></returns>
        private async Task<ServerVideoGuid> CreateServerAndVideo()
        {
            var serverResultDTO = await AddServer("testeaddvideo", "127.0.0.10", 8081);
            var serverId = (Guid)serverResultDTO.ServerId;


            var videoResultDTO = await AddVideo(serverId, "video de teste", "QVdTIDUyLjY3LjkyLjEyMg0KQVdTMiA1Mi42Ny4zMS4yMDQNCjE4Ny40OS4yMzkuMjEyDQpHQzEgMzUuMTk5Ljg5LjE4Ng0KR0MyIDM1LjE5OS43My45OA==");

            var videoId = (Guid)videoResultDTO.VideoId;

            var result = new ServerVideoGuid
            {
                ServerId = serverId,
                VideoId = videoId
            };
            return result;
        }

        private async Task<VideoDTO> AddVideo(Guid serverId, string description, string videoDataBase64)
        {

            var videoDTO = TestHelper.GetNewVideoDTO(description, videoDataBase64);

            var body = TestHelper.ObjectToStringContent(videoDTO);
            var request = new HttpRequestMessage(new HttpMethod("GET"), $"/api/Servers/{serverId}/videos");

            var response = await _client.PostAsync(request.RequestUri, body);
            Task<string> responseMessage = response.Content.ReadAsStringAsync();

            var videoDTOResult = JsonConvert.DeserializeObject<VideoDTO>(responseMessage.Result);

            return videoDTOResult;
        }

        private async Task<VideoDTO> GetVideoInfo(Guid serverId, Guid videoId)
        {
            var request = new HttpRequestMessage(new HttpMethod("GET"), $"/api/servers/{serverId}/videos/{videoId}");
            var response = await _client.SendAsync(request);
            Task<string> responseMessage = response.Content.ReadAsStringAsync();
            var videoDTOResult = JsonConvert.DeserializeObject<VideoDTO>(responseMessage.Result);

            return videoDTOResult;
        }

        private async Task<List<VideoDTO>> GetServerVideos(Guid serverId)
        {
            var request = new HttpRequestMessage(new HttpMethod("GET"), $"/api/servers/{serverId}/videos/");
            var response = await _client.SendAsync(request);
            Task<string> responseMessage = response.Content.ReadAsStringAsync();
            var videoDTOResult = JsonConvert.DeserializeObject<List<VideoDTO>>(responseMessage.Result);

            return videoDTOResult;
        }

        private async Task<HttpResponseMessage> DeleteVideo(Guid serverId, Guid videoId)
        {
            var request = new HttpRequestMessage(new HttpMethod("DELETE"), $"/api/servers/{serverId}/videos/{videoId}");
            return await _client.SendAsync(request);
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
        // ~VideosControllerTest()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        void IDisposable.Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
