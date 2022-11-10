using System;
using System.Net;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
namespace Nexpo.Tests.API
{
    public class SimpleTest
    {
        [Fact]
        public async Task GET_random_incorrect_route()
        {
            await using var application = new WebApplicationFactory<Nexpo.Program>();
            using var client = application.CreateClient();

            var response = await client.GetAsync("/ahshashas");
            Console.WriteLine(response);
            Assert.True(!response.StatusCode.Equals(HttpStatusCode.OK), "Should not be OK. Does not exist");
        }
        [Fact]
        public async Task GET_Existing_route()
        {
            var application = new WebApplicationFactory<Nexpo.Program>();
            var client = application.CreateClient();

            var response = await client.GetAsync("/api/companies");
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), "Does exist. Should be ok");

            var response_text = await response.Content.ReadAsStringAsync();
            foreach (var item in response_text)
            {
                Console.WriteLine(item);
            }

        }
    }
}
