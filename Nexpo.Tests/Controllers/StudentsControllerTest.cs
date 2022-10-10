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
using System.Collections.Generic;

namespace Nexpo.Tests.Controllers
{
    public class StudentsControllerTest
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
        public async Task GetSpecificStudentAsAdmin()
        {
            var application = new WebApplicationFactory<Nexpo.Program>();
            var client = application.CreateClient();
            var token = await Login("admin", client);

            var response = await client.GetAsync("/api/students/-1");
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), response.StatusCode.ToString());

            var responseObject = JsonConvert.DeserializeObject<Student>((await response.Content.ReadAsStringAsync()));
            Assert.True(responseObject.Id == -1, responseObject.Id.ToString());
            Assert.True(responseObject.Guild == Guild.D, responseObject.Guild.ToString());
            Assert.True(responseObject.Year == 4, responseObject.Year.ToString());
            Assert.True(responseObject.UserId == -2, responseObject.UserId.ToString());
        }

        [Fact]
        public async Task GetSpecificStudentAsCompanyRep()
        {
            var application = new WebApplicationFactory<Nexpo.Program>();
            var client = application.CreateClient();
            var token = await Login("company", client);

            var response = await client.GetAsync("/api/students/-1");
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), response.StatusCode.ToString());

            var responseObject = JsonConvert.DeserializeObject<Student>((await response.Content.ReadAsStringAsync()));
            Assert.True(responseObject.Id == -1, responseObject.Id.ToString());
            Assert.True(responseObject.Guild == Guild.D, responseObject.Guild.ToString());
            Assert.True(responseObject.Year == 4, responseObject.Year.ToString());
            Assert.True(responseObject.UserId == -2, responseObject.UserId.ToString());
        }

        [Fact]
        public async Task GetSpecificStudentInvalidId()
        {
            var application = new WebApplicationFactory<Nexpo.Program>();
            var client = application.CreateClient();
            var token = await Login("admin", client);

            var response = await client.GetAsync("/api/students/-123");
            Assert.True(response.StatusCode.Equals(HttpStatusCode.NotFound), response.StatusCode.ToString());
        }


        [Fact]
        public async Task GetSpecificStudentAsStudent()
        {
            var application = new WebApplicationFactory<Nexpo.Program>();
            var client = application.CreateClient();
            var token = await Login("", client);

            var response = await client.GetAsync("/api/students/-1");
            Assert.True(response.StatusCode.Equals(HttpStatusCode.Forbidden), response.StatusCode.ToString());
        }

        [Fact]
        public async Task GetSpecificStudentNotLoggedIn()
        {
            var application = new WebApplicationFactory<Nexpo.Program>();
            var client = application.CreateClient();

            var response = await client.GetAsync("/api/students/-1");
            Assert.True(response.StatusCode.Equals(HttpStatusCode.Unauthorized), response.StatusCode.ToString());
        }

    }
}
