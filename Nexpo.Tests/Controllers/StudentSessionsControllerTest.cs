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
using Microsoft.Extensions.DependencyInjection;

namespace Nexpo.Tests.Controllers
{
    public class StudentSessionsControllerTest
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

        [Fact]
        public async Task TestGetStudentSessions()
        {
            //Test get with student auth
            var client = await StudentClient();
            var response = await client.GetAsync("/api/studentsessions");
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), "Auth didn't work");
            string content = new StreamReader(response.Content.ReadAsStream()).ReadToEnd();
            content = content.Replace("[", " ").Replace("]", " ").Trim();
            var parsedContent = JObject.Parse(content);
            var stringContent = parsedContent.Value<string>("id");
            Assert.Equal("1", stringContent);
        }

        [Fact]
        public async Task TestGetSingleStudentSessions()
        {
            var client = await StudentClient();
            var response = await client.GetAsync("/api/studentsessions/1");
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), response.StatusCode + "Token didn't work");
            string content = new StreamReader(response.Content.ReadAsStream()).ReadToEnd();
            var parsedContent = JObject.Parse(content);
            var stringContent = parsedContent.Value<string>("status");
            Assert.Equal("0", stringContent);
        }

        //[Fact]
        //public async Task TestPutSingleStudentSessions()
        //{
        //    //Test original value
        //    var client = await StudentClient();
        //    var response = await client.GetAsync("/api/studentsessions/1");
        //    string content = new StreamReader(response.Content.ReadAsStream()).ReadToEnd();
        //    var parsedContent = JObject.Parse(content);
        //    var stringContent = parsedContent.Value<string>("status");
        //    Assert.True(stringContent.Equals("1"), parsedContent.ToString());
            
        //    //Send DTO??
        //    var json = new JsonObject();
        //    json.Add("status", "1");
        //    var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
        //    response = await client.PutAsync("/api/studentsessions/1", payload);
        //    Assert.True(HttpStatusCode.OK.Equals(response.StatusCode), json.ToString());


        //    response = await client.GetAsync("/api/studentsessions/1");
        //    content = new StreamReader(response.Content.ReadAsStream()).ReadToEnd();
        //    parsedContent = JObject.Parse(content);
        //    stringContent = parsedContent.Value<string>("status");
        //    Assert.Equal("1", stringContent);


        //    json = new JsonObject();
        //    json.Add("status", "0");
        //    payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
        //    response = await client.PutAsync("/api/studentsessions/1", payload);
        //    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        //}
    }
}
