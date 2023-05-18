using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Nexpo.Models;
using Nexpo.DTO;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Xunit;

namespace Nexpo.Tests.Controllers
{
    public class FAQControllerTest
    {

        [Fact]
        public async Task GetAll()
        {


        }

        [Fact]
        public async Task Get()
        {

        }

        [Fact]
        public async Task GetNonExisting()
        {

        }

        [Fact]
        public async Task GetUnautherized()
        {

        }


        [Fact]
        public async Task PostAsAdmin()
        {

        }

        [Fact]
        public async Task PostUnautherized()
        {
            var client = await TestUtils.Login("student1");

            var updateFAQDto = new UpdateFAQDTO
            {
                Question = ""
            };

            var json = JsonConvert.SerializeObject(updateFAQDto);

            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await client.PutAsync("api/faq/-1", payload);

            Assert.True(response.StatusCode.Equals(HttpStatusCode.Forbidden), "Wrong status code. Expected: Forbidden. Received: " + response.StatusCode.ToString());

        }

        [Fact]
        public async Task PostAndDeleteAsAdmin()
        {

        }

        [Fact]
        public async Task PostAndDeleteUnautherized()
        {

        }


    }
}