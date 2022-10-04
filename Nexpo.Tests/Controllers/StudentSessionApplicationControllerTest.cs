using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Xunit;
using Newtonsoft.Json;
using Nexpo.Models;
using Nexpo.DTO;

namespace Nexpo.Tests.Controllers
{
    public class StudentSessionApplicationControllerTest
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
        public async Task GetAllApplicationsAsCompany()
        {
            var application = new WebApplicationFactory<Nexpo.Program>();
            var client = application.CreateClient();
            var token = await Login("company", client);

            var response = await client.GetAsync("/api/applications/my/company");
            var responseList = JsonConvert.DeserializeObject<List<StudentSessionApplication>>((await response.Content.ReadAsStringAsync()));
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), response.StatusCode.ToString());
            Assert.True(responseList.Count == 3, "Number of applications: " + responseList.Count.ToString());

            var app1 = responseList.Find(r => r.Id == -1);
            var app3 = responseList.Find(r => r.Id == -3);

            Assert.True(app1.Motivation == "I think you are an interesting company", "Wrong motivation: " + app1.Motivation);
            Assert.True(app3.Motivation == "User experience is very important for me", app3.Motivation);
        }

        [Fact]
        public async Task GetApplicationAsStudent()
        {
            var application = new WebApplicationFactory<Nexpo.Program>();
            var client = application.CreateClient();
            var token = await Login("", client);
            //Get students own application
            var response = await client.GetAsync("/api/applications/-1");
            var app = JsonConvert.DeserializeObject<StudentSessionApplication>((await response.Content.ReadAsStringAsync()));
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), response.StatusCode.ToString());
            Assert.True(app.Motivation == "I think you are an interesting company", "Wrong motivation: " + app.Motivation);
            //Get someone elses application
            response = await client.GetAsync("/api/applications/-3");
            Assert.True(!response.StatusCode.Equals(HttpStatusCode.OK), "Inproper access");
        }

        [Fact]
        public async Task RespondToApplicationAsCompany()
        {
            var application = new WebApplicationFactory<Nexpo.Program>();
            var client = application.CreateClient();
            var token = await Login("company", client);

            var json = new JsonObject();
            json.Add("status", 1);

            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await client.PutAsync("api/applications/-1", payload);
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), response.ToString());

            response = await client.GetAsync("/api/applications/-1");
            var app = JsonConvert.DeserializeObject<StudentSessionApplication>((await response.Content.ReadAsStringAsync()));

        }
    }
}
