using System;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using Moq;

using LaPlay.Business;
using LaPlay.Business.Model;

namespace LaPlay.Access
{
    public class ControllerTest
    {
        private readonly TestServer _server;
        private readonly HttpClient _client;

        public ControllerTest()
        {
            IStorageSpaceContract mockedStorageSpaceContract = Mock.Of<IStorageSpaceContract>();
            //mockedStorageSpaceContract.Setup(m => m.CreateStorageSpace()).Returns(null);
            
            var webHost = new WebHostBuilder()
                .ConfigureTestServices(s => s.AddSingleton<IStorageSpaceContract>(mockedStorageSpaceContract))
                .UseStartup<Startup>();

            _server = new TestServer(webHost);
            _client = _server.CreateClient();
        }

        [Fact]
        public async Task ReturnOK()
        {
            // Act
            var response = await _client.GetAsync("/api/v1/fixedString");
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal("fixedString", responseString);
        }


    }
}