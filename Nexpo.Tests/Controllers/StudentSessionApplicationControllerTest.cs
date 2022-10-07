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

            //Ensure seed data is correct
            var response = await client.GetAsync("/api/applications/-1");
            var app = JsonConvert.DeserializeObject<StudentSessionApplication>((await response.Content.ReadAsStringAsync()));
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), "Get application error code: " + response.StatusCode);
            Assert.True(app.Status == StudentSessionApplicationStatus.NoResponse, "Incorrect seed data");

            //Update status
            var json = new JsonObject();
            json.Add("status", 1);
            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            response = await client.PutAsync("api/applications/-1", payload);
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), "Put application error code: " + response.StatusCode);

            //Check update worked
            response = await client.GetAsync("/api/applications/-1");
            app = JsonConvert.DeserializeObject<StudentSessionApplication>(await response.Content.ReadAsStringAsync());
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), "Get application error code: " + response.StatusCode);
            Assert.True(app.Status == StudentSessionApplicationStatus.Accepted, "Status didn't update, got: " + app.Status);

            //Roll back
            json = new JsonObject();
            json.Add("status", 0);
            payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            response = await client.PutAsync("api/applications/-1", payload);
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), "Get application error code: " + response.StatusCode);
        }

        [Fact]
        public async Task PostApplicationAsStudentThenDelete()
        {
            var application = new WebApplicationFactory<Nexpo.Program>();
            var studentClient = application.CreateClient();
            var companyClient = application.CreateClient();
            await Login("", studentClient);
            await Login("company", companyClient);
            
            //Ensure seed data is correct
            var response = await companyClient.GetAsync("/api/applications/my/company");
            var appList = JsonConvert.DeserializeObject<List<StudentSessionApplication>>((await response.Content.ReadAsStringAsync()));
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), response.ToString());
            Assert.True(appList.Count == 3, "Incorrect seed data");

            //Post application
            var json = new JsonObject();
            json.Add("motivation", "Hej, jag är jättebra och tror att ni vill träffa mig!");
            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            response = await studentClient.PostAsync("api/applications/company/-1", payload);
            Assert.True(response.StatusCode.Equals(HttpStatusCode.Created), "Post application error code: " + response.StatusCode);

            //Check application as company
            response = await companyClient.GetAsync("/api/applications/my/company");
            appList = JsonConvert.DeserializeObject<List<StudentSessionApplication>>((await response.Content.ReadAsStringAsync()));
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), "Get as company error");
            Assert.True(appList.Count == 4, "Application list length should be 4, count:" + appList.Count.ToString());
            Assert.True(appList[3].Motivation == "Hej, jag är jättebra och tror att ni vill träffa mig!", "Wrong motivation, got: " + appList[3].Motivation);
            int id = appList[3].Id.GetValueOrDefault();
            
            //Delete as company
            response = await companyClient.DeleteAsync("/api/applications/" + id.ToString());
            Assert.True(response.IsSuccessStatusCode, "Delete application error");
            
            //Check that delete works
            response = await companyClient.GetAsync("/api/applications/my/company");
            appList = JsonConvert.DeserializeObject<List<StudentSessionApplication>>((await response.Content.ReadAsStringAsync()));
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), "Get as company error");
            Assert.True(appList.Count == 3, "Application list length should be 4, count:" + appList.Count.ToString());
        }

        [Fact]
        public async Task GetApplicationAccepted()
        {
            var application = new WebApplicationFactory<Nexpo.Program>();
            var client = application.CreateClient();
            await Login("", client);

            var response = await client.GetAsync("/api/applications/accepted/-1");
            var accepted = JsonConvert.DeserializeObject<bool>((await response.Content.ReadAsStringAsync()));
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), "Get error code: ");
            Assert.True(accepted, "content should be true");
        }
    }
}
