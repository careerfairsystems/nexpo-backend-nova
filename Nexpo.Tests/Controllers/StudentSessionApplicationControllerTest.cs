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

namespace Nexpo.Tests.Controllers
{
    public class StudentSessionApplicationControllerTest
    {
        public static async Task<HttpClient> StudentClient()
        {
            //Create client and login
            var application = new WebApplicationFactory<Nexpo.Program>();
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
    }
}
