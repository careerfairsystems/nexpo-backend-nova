using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Xunit;
using System.IO;
using Newtonsoft.Json;
using Nexpo.Models;
using Nexpo.DTO;
using System.Collections.Generic;

namespace Nexpo.Tests.Controllers 
{
    public class CompaniesControllerTest
    {
        private async Task<String> Login(string role, HttpClient client)
        {
            var json = new JsonObject();
            switch (role)
            {
                case "company":
                    json.Add("email", "rep1@company1.example.com");
                    json.Add("password", "password");
                    break;
                case "admin":
                    json.Add("email", "admin@example.com");
                    json.Add("password", "password");
                    break;
                default:
                    json.Add("email", "student1@example.com");
                    json.Add("password", "password");
                    break;
            }

            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("/api/session/signin", payload);
            string token = new StreamReader(response.Content.ReadAsStream()).ReadToEnd();
            var parser = JObject.Parse(token);
            token = "Bearer " + parser.Value<String>("token");
            client.DefaultRequestHeaders.Add("Authorization", token);
            return token;
        }

        [Fact]
        public async Task GetAllCompanies()
        {
            var application = new WebApplicationFactory<Nexpo.Program>();
            var client = application.CreateClient();
            var response = await client.GetAsync("/api/companies/");

            string responseText = await response.Content.ReadAsStringAsync();
            var responseList = JsonConvert.DeserializeObject<List<PublicCompanyDto>>(responseText);
         
            Assert.True(responseList.Count == 4, responseText.ToString());
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), "Login failed, returned: " + response.StatusCode.ToString());
        }

        [Fact]
        public async Task GetCompanySuccesful()
        {
            var application = new WebApplicationFactory<Nexpo.Program>();
            var client = application.CreateClient();
            var response = await client.GetAsync("/api/companies/-1");

            string responseText = await response.Content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject<PublicCompanyDto>(responseText);
         
            Assert.True(responseObject.Name == "Apple", responseText);
            Assert.True(responseObject.Description == "A fruit company", responseText);
            Assert.True(responseObject.DidYouKnow == "Apples", responseText);
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), "Login failed, returned: " + response.StatusCode.ToString());
        }

        [Fact]
        public async Task GetCompanyFailure()
        {
            var application = new WebApplicationFactory<Nexpo.Program>();
            var client = application.CreateClient();
            var response = await client.GetAsync("/api/companies/-5");
         
            Assert.True(response.StatusCode.Equals(HttpStatusCode.NotFound), "Login failed, returned: " + response.StatusCode.ToString());
        }

        [Fact]
        public async Task PutCompany()
        {
            var application = new WebApplicationFactory<Nexpo.Program>();
            var client = application.CreateClient();
            var token = await Login("admin", client);
            var dto = new UpdateCompanyDto();
            dto.Description = "None";
            
            var json = new JsonObject();
            json.Add("dto", JsonConvert.SerializeObject(dto));
            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await client.PutAsync("/api/companies/-1", payload);

            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), response.StatusCode.ToString());

            string responseText = await response.Content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject<PublicCompanyDto>(responseText);
         
            Assert.True(responseObject.Description == "A fruit company", response.StatusCode.ToString());
        }

        [Fact]
        public async Task GetMe()
        {
            var application = new WebApplicationFactory<Nexpo.Program>();
            var client = application.CreateClient();
            var response = await client.GetAsync("/api/events/2/tickets");

            Assert.True(response.StatusCode.Equals(HttpStatusCode.Unauthorized), response.StatusCode.ToString());
        }

        [Fact]
        public async Task PutMe()
        {
            var application = new WebApplicationFactory<Nexpo.Program>();
            var client = application.CreateClient();
            var response = await client.GetAsync("/api/events/2/tickets");

            Assert.True(response.StatusCode.Equals(HttpStatusCode.Unauthorized), response.StatusCode.ToString());

        }

    }
}