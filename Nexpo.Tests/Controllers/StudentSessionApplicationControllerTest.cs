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

namespace Nexpo.Tests.Controllers
{
    public class StudentSessionApplicationControllerTest
    {
        public static async Task<HttpClient> StudentClient()
        {
            //Create client and login
            var application = new WebApplicationFactory<Program>();
            var client = application.CreateClient();
            var response = await client.GetAsync("/api/studentsessions");
            var json = new JsonObject();
            json.Add("email", "student1@example.com");
            json.Add("password", "password");
            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            response = await client.PostAsync("/api/session/signin", payload);
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), "Login failed");
            string token = new StreamReader(response.Content.ReadAsStream()).ReadToEnd();
            var parser = JObject.Parse(token);
            token = "Bearer " + parser.Value<string>("token");
            client.DefaultRequestHeaders.Add("Authorization", token);
            return client;
        }

        public static async Task<HttpClient> CompanyClient()
        {
            //Create client and login
            var application = new WebApplicationFactory<Program>();
            var client = application.CreateClient();
            var response = await client.GetAsync("/api/studentsessions");
            var json = new JsonObject();
            json.Add("email", "rep1@company1.example.com");
            json.Add("password", "password");
            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            response = await client.PostAsync("/api/session/signin", payload);
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), "Login failed");
            string token = new StreamReader(response.Content.ReadAsStream()).ReadToEnd();
            var parser = JObject.Parse(token);
            token = "Bearer " + parser.Value<string>("token");
            client.DefaultRequestHeaders.Add("Authorization", token);
            return client;
        }

        [Fact]
        public async Task GetApplicationAsStudent()
        {
            var client = await StudentClient();
            var response = await client.GetAsync("/api/applications/1");

            string responseText = await response.Content.ReadAsStringAsync();
            var application = JsonConvert.DeserializeObject<StudentSessionApplication>(responseText);
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK));

            Assert.True(application.Motivation == "I think you are an interesting company", application.Motivation);

            response = await client.GetAsync("/api/applications/5");
            Assert.True(response.StatusCode.Equals(HttpStatusCode.Forbidden));
        }

        [Fact]
        public async Task GetApplicationAsCompany()
        {
            var client = await CompanyClient();
            var response = await client.GetAsync("/api/applications/1");

            string responseText = await response.Content.ReadAsStringAsync();
            var application = JsonConvert.DeserializeObject<StudentSessionApplication>(responseText);
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK));
            Assert.True(application.Motivation == "I think you are an interesting company", application.Motivation);

            response = await client.GetAsync("/api/applications/7");
            Assert.True(response.StatusCode.Equals(HttpStatusCode.Forbidden));
        }

        [Fact]
        public async Task GetAllApplicationAsCompany()
        {
            var client = await CompanyClient();
            var response = await client.GetAsync("/api/applications/my/company");

            string responseText = await response.Content.ReadAsStringAsync();
            var responseList = JsonConvert.DeserializeObject<List<StudentSessionApplication>>(responseText);
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK));
            Assert.True(responseList.Count == 3, responseText.ToString());

            var application1 = responseList.Find(r => r.Id == 1);
            var application2 = responseList.Find(r => r.Id == 2);
            var application3 = responseList.Find(r => r.Id == 3);


            Assert.True(application1.Motivation == "I think you are an interesting company", application1.Motivation);
            Assert.True(application2.Motivation == "I love my MacBook", application2.Motivation);
            Assert.True(application3.Motivation == "User experience is very important for me", application3.Motivation);
        }
    }
}
