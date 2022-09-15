using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json.Linq;
using Microsoft.OpenApi.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
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
            //Test get without auth
            var application = new WebApplicationFactory<Nexpo.Program>();
            var client = application.CreateClient();
            var response = await client.GetAsync("/api/studentsessions");
            Assert.True(response.StatusCode.Equals(HttpStatusCode.Unauthorized), response.StatusCode.ToString());

            //Login
            var json = new JsonObject();
            json.Add("email", "student1@example.com");
            json.Add("password", "password");

            var content = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            response = await client.PostAsync("/api/session/signin", content);
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), "Login failed");
            string token = new StreamReader(response.Content.ReadAsStream()).ReadToEnd();
            var parser = JObject.Parse(token);
            token = parser.Value<String>("token");
            client.DefaultRequestHeaders.Add("Authorization", "Bearer "+ token);

            //Test get with Student auth
            response = await client.GetAsync("/api/studentsessions");
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), token);
        }
    }
}
