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
            dto.Description = "New description";
            
            var json = new JsonObject();
            json.Add("description", dto.Description);
            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await client.PutAsync("/api/companies/-1", payload);

            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), response.StatusCode.ToString());

            string responseText = await response.Content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject<PublicCompanyDto>(responseText);
         
            Assert.True(responseObject.Description == "New description", $"Description was actually ({responseObject.Description})");
            json.Remove("description");
            json.Add("description", "A fruit company");
            await client.PutAsync("/api/companies/-1", new StringContent(json.ToString(), Encoding.UTF8, "application/json"));
        }

        [Fact]
        public async Task PutCompanyForbidden()
        {
            var application = new WebApplicationFactory<Nexpo.Program>();
            var client = application.CreateClient();
            var token = await Login("company", client);
            var dto = new UpdateCompanyDto();
            dto.Description = "None";
            
            var json = new JsonObject();
            json.Add("description", dto.Description);
            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await client.PutAsync("/api/companies/-1", payload);

            Assert.True(response.StatusCode.Equals(HttpStatusCode.Forbidden), response.StatusCode.ToString());

            string responseText = await response.Content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject<PublicCompanyDto>(responseText);
         
            Assert.True(responseObject == null, "Object was not null");
        }

        [Fact]
        public async Task PutNonExistingCompany()
        {
            var application = new WebApplicationFactory<Nexpo.Program>();
            var client = application.CreateClient();
            var token = await Login("admin", client);
            var dto = new UpdateCompanyDto();
            dto.Description = "None";
            
            var json = new JsonObject();
            json.Add("description", dto.Description);
            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await client.PutAsync("/api/companies/-5", payload);

            Assert.True(response.StatusCode.Equals(HttpStatusCode.NotFound), response.StatusCode.ToString());
        }

        [Fact]
        public async Task GetMe()
        {
            var application = new WebApplicationFactory<Nexpo.Program>();
            var client = application.CreateClient();
            var token = await Login("company", client);
            var response = await client.GetAsync("/api/companies/me");

            string responseText = await response.Content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject<PublicCompanyDto>(responseText);

            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), "Login failed, returned: " + response.StatusCode.ToString());
            Assert.True(responseObject.Name == "Apple", responseText);
            Assert.True(responseObject.Description == "A fruit company", responseText);
            Assert.True(responseObject.DidYouKnow == "Apples", responseText);
        }

        [Fact]
        public async Task GetIllegalMe()
        {
            var application = new WebApplicationFactory<Nexpo.Program>();
            var client = application.CreateClient();
            var token = await Login("admin", client);
            var response = await client.GetAsync("/api/companies/me");

            string responseText = await response.Content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject<PublicCompanyDto>(responseText);

            Assert.True(response.StatusCode.Equals(HttpStatusCode.Forbidden), "Login failed, returned: " + response.StatusCode.ToString());
            Assert.True(responseObject == null, "Object was not null");
        }

        [Fact]
        public async Task PutMe()
        {
            var application = new WebApplicationFactory<Nexpo.Program>();
            var client = application.CreateClient();
            var token = await Login("company", client);
            var dto = new UpdateCompanyDto();
            dto.Description = "New description";
            
            var json = new JsonObject();
            json.Add("description", dto.Description);
            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await client.PutAsync("/api/companies/me", payload);

            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), response.StatusCode.ToString());

            string responseText = await response.Content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject<PublicCompanyDto>(responseText);
         
            Assert.True(responseObject.Description == "New description", $"Description was actually ({responseObject.Description})");
            json.Remove("description");
            json.Add("description", "A fruit company");
            await client.PutAsync("/api/companies/me", new StringContent(json.ToString(), Encoding.UTF8, "application/json"));
        }

        [Fact]
        public async Task PutMeForbidden()
        {
            var application = new WebApplicationFactory<Nexpo.Program>();
            var client = application.CreateClient();
            var token = await Login("admin", client);
            var dto = new UpdateCompanyDto();
            dto.Description = "None";
            
            var json = new JsonObject();
            json.Add("description", dto.Description);
            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await client.PutAsync("/api/companies/me", payload);

            Assert.True(response.StatusCode.Equals(HttpStatusCode.Forbidden), response.StatusCode.ToString());

            string responseText = await response.Content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject<PublicCompanyDto>(responseText);
         
            Assert.True(responseObject == null, "Object was not null");
        }

    }
}