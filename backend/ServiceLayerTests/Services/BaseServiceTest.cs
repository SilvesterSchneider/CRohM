using System.Net.Http;
using System.Runtime.CompilerServices;
using WebApi;
using Xunit;

namespace ServiceLayerTests.Services
{
    public abstract class BaseServiceTest : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        protected readonly HttpClient Client;

        public BaseServiceTest(CustomWebApplicationFactory<Startup> factory)
        {
            Client = factory.CreateClient();
        }

        public static string GetActualAsyncMethodName([CallerMemberName]string name = null) => name;
    }
}