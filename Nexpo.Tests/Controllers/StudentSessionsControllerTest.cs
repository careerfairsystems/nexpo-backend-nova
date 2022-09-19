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

namespace Nexpo.Tests.Controllers
{
    public class StudentSessionsControllerTest
    {
        [Fact]
        public async Task TestGetStudentSessions()
        {
            //Test get without proper auth
            var application = new WebApplicationFactory<Nexpo.Program>();
            var client = application.CreateClient();
            var response = await client.GetAsync("/api/studentsessions");
            Assert.True(response.StatusCode.Equals(HttpStatusCode.Unauthorized), response.StatusCode.ToString());

            //Login with email and password from seeded DB
            var json = new JsonObject();
            json.Add("email", "student1@example.com");
            json.Add("password", "password");

            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            response = await client.PostAsync("/api/session/signin", payload);
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), "Login failed");
            
            //Parse token and add "Bearer " for some reason
            string token = new StreamReader(response.Content.ReadAsStream()).ReadToEnd();
            var parser = JObject.Parse(token);
            token = "Bearer " + parser.Value<String>("token");
            //Update default header
            client.DefaultRequestHeaders.Add("Authorization", token);

            //Test get with Student auth
            response = await client.GetAsync("/api/studentsessions");
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), "Token didn't work: " + token);
        }

        [Fact]
        public async Task TestGetSingleStudentSessions()
        {
            //Test get without proper auth
            var application = new WebApplicationFactory<Nexpo.Program>();
            var client = application.CreateClient();
            var response = await client.GetAsync("/api/studentsessions/1");
            Assert.True(response.StatusCode.Equals(HttpStatusCode.Unauthorized), response.StatusCode.ToString());

            //Login with email and password from seeded DB
            var json = new JsonObject();
            json.Add("email", "student1@example.com");
            json.Add("password", "password");

            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            response = await client.PostAsync("/api/session/signin", payload);
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), "Login failed");

            //Parse token and add "Bearer " for some reason
            string token = new StreamReader(response.Content.ReadAsStream()).ReadToEnd();
            var parser = JObject.Parse(token);
            token = "Bearer " + parser.Value<String>("token");
            //Update default header
            client.DefaultRequestHeaders.Add("Authorization", token);

            //Test get with Student auth
            response = await client.GetAsync("/api/studentsessions/1");
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), response.StatusCode + "Token didn't work: " + token);
        }

        [Fact]
        public async Task TestPutSingleStudentSessions()
        {
            var application = new WebApplicationFactory<Nexpo.Program>();
            var client = application.CreateClient();

            //Login with email and password from seeded DB
            var json = new JsonObject();
            json.Add("email", "student1@example.com");
            json.Add("password", "password");
            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("/api/session/signin", payload);
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), "Login failed");
            string token = new StreamReader(response.Content.ReadAsStream()).ReadToEnd();
            var parser = JObject.Parse(token);
            token = "Bearer " + parser.Value<String>("token");
            client.DefaultRequestHeaders.Add("Authorization", token);

            //Test put with Student auth
            json = new JsonObject();
            json.Add("Id", "1");
            payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            response = await client.PutAsync("/api/studentsessions/1", payload);
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), response.StatusCode.ToString());
        }
    }
}
