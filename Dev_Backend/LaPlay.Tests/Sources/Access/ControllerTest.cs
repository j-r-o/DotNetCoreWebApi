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
        private readonly TestServer _Server;
        private readonly HttpClient _Client;
        //private readonly IStorageSpaceContract _MockedStorageSpaceContract;
        private readonly Mock<IStorageSpaceContract> _MockedStorageSpaceContract;

        public ControllerTest()
        {
            _MockedStorageSpaceContract = new Mock<IStorageSpaceContract>();
            //mockedStorageSpaceContract.Setup(m => m.CreateStorageSpace()).Returns(null);
            
            // var webHost = new WebHostBuilder()
            //     .ConfigureTestServices(s => s.AddSingleton<IStorageSpaceContract>(mockedStorageSpaceContract))
            //     .UseStartup<Startup>();

            // _server = new TestServer(webHost);

            _Server = new TestServer(
                new WebHostBuilder()
                .ConfigureTestServices(services => 
                    services.AddSingleton<IStorageSpaceContract>(_MockedStorageSpaceContract.Object)
                )
                .UseStartup<Startup>()            
            );

            _Client = _Server.CreateClient();
        }

        [Fact]
        public async Task ReturnOK()
        {
            // Act
            var response = await _Client.GetAsync("/api/v1/fixedString");
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal("fixedString", responseString);
        }

        /* ========================= LaPlay API ========================= */

        [Fact]
        public async Task CreateStorageSpace_ShouldSucceed()
        {
            StorageSpace storageSpace = new StorageSpace{ Id = Guid.NewGuid(), Name = "TestName", MainFolder = "TestMainFolder", MirrorFolder = "TestMirrorFolder" };

            StorageSpace capturedStorageSpace = null;
            _MockedStorageSpaceContract.Setup(m => m.CreateStorageSpace(It.IsAny<StorageSpace>())).Callback((StorageSpace s) =>  capturedStorageSpace = s);

            HttpResponseMessage response = await _Client.PostAsJsonAsync("/api/v1/storagespace", storageSpace);
            response.EnsureSuccessStatusCode();

            Assert.Equal(storageSpace.Id, capturedStorageSpace.Id);
            Assert.Equal(storageSpace.Name, capturedStorageSpace.Name);
            Assert.Equal(storageSpace.MainFolder, capturedStorageSpace.MainFolder);
            Assert.Equal(storageSpace.MirrorFolder, capturedStorageSpace.MirrorFolder);
        }


    }
}