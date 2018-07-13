using System;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using System.Threading.Tasks;
using System.Net.Http;

namespace LaPlay.Access
{
    public class ControllerTest
    {
        private readonly TestServer _server;
        private readonly HttpClient _client;

        public ControllerTest()
        {
            _server = new TestServer(new WebHostBuilder().UseStartup<Startup>());
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