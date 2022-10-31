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
using Microsoft.VisualStudio.TestPlatform.TestHost;

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
                case "company3":
                    json.Add("email", "rep1@company3.example.com");
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
            var responseList = JsonConvert.DeserializeObject<List<StudentSessionApplicationDto>>((await response.Content.ReadAsStringAsync()));
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), response.StatusCode.ToString());
            Assert.True(responseList.Count == 3, "Number of applications: " + responseList.Count.ToString());

            var app3 = responseList.Find(r => r.Id == -3);

            Assert.True(app3.Motivation == "User experience is very important for me", app3.Motivation);
            Assert.True(app3.StudentYear == 3, app3.StudentYear.ToString());
            Assert.True(app3.StudentProgramme == Programme.Väg_och_vatttenbyggnad, app3.StudentProgramme.ToString());
            Assert.True(app3.StudentFirstName == "Gamma", app3.StudentFirstName);
            Assert.True(app3.StudentLastName == "Student", app3.StudentLastName);
        }

        [Fact]
        public async Task GetAllApplicationsAsCompanyWrongPath()
        {
            var application = new WebApplicationFactory<Nexpo.Program>();
            var client = application.CreateClient();
            var token = await Login("company", client);

            var response = await client.GetAsync("/api/applications/my/student");
            Assert.True(response.StatusCode.Equals(HttpStatusCode.Forbidden), response.StatusCode.ToString());
        }

        [Fact]
        public async Task GetAllApplicationsAsStudent()
        {
            var application = new WebApplicationFactory<Nexpo.Program>();
            var client = application.CreateClient();
            var token = await Login("", client);

            var response = await client.GetAsync("/api/applications/my/student");
            var responseList = JsonConvert.DeserializeObject<List<StudentSessionApplication>>((await response.Content.ReadAsStringAsync()));
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), response.StatusCode.ToString());
            Assert.True(responseList.Count == 2, "Number of applications: " + responseList.Count.ToString());

            var app3 = responseList.Find(r => r.Id == -4);

            Assert.True(app3.Motivation == "I would like to learn more about searching", app3.Motivation);
            Assert.True(app3.StudentId == -1, app3.StudentId.ToString());
            Assert.True(app3.CompanyId == -2, app3.CompanyId.ToString());
        }

        [Fact]
        public async Task GetAllApplicationsAsStudentWrongPath()
        {
            var application = new WebApplicationFactory<Nexpo.Program>();
            var client = application.CreateClient();
            var token = await Login("", client);

            var response = await client.GetAsync("/api/applications/my/company");
            Assert.True(response.StatusCode.Equals(HttpStatusCode.Forbidden), response.StatusCode.ToString());
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
            Assert.True(app.Motivation == "Hej, jag är jättebra och tror att ni vill träffa mig!", "Wrong motivation: " + app.Motivation);
            //Get someone elses application
            response = await client.GetAsync("/api/applications/-3");
            Assert.True(!response.StatusCode.Equals(HttpStatusCode.OK), "Inproper access");
        }

        [Fact]
        public async Task GetAnotherApplicationAsStudent()
        {
            var application = new WebApplicationFactory<Nexpo.Program>();
            var client = application.CreateClient();
            var token = await Login("", client);
            var response = await client.GetAsync("/api/applications/-3");
            Assert.True(response.StatusCode.Equals(HttpStatusCode.Forbidden), "Inproper access");
        }

        [Fact]
        public async Task GetApplicationAsCompany()
        {
            var application = new WebApplicationFactory<Nexpo.Program>();
            var client = application.CreateClient();
            var token = await Login("company", client);
            var response = await client.GetAsync("/api/applications/-1");
            var app = JsonConvert.DeserializeObject<StudentSessionApplication>((await response.Content.ReadAsStringAsync()));
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), response.StatusCode.ToString());
            Assert.True(app.Motivation == "Hej, jag är jättebra och tror att ni vill träffa mig!", "Wrong motivation: " + app.Motivation);
        }

        [Fact]
        public async Task GetAnotherCompanyApplication()
        {
            var application = new WebApplicationFactory<Nexpo.Program>();
            var client = application.CreateClient();
            var token = await Login("company", client);
            var response = await client.GetAsync("/api/applications/-4");
            var app = JsonConvert.DeserializeObject<StudentSessionApplication>((await response.Content.ReadAsStringAsync()));
            Assert.True(response.StatusCode.Equals(HttpStatusCode.Forbidden), response.StatusCode.ToString());
        }

        [Fact]
        public async Task GetCompanyApplicationNotExist()
        {
            var application = new WebApplicationFactory<Nexpo.Program>();
            var client = application.CreateClient();
            var token = await Login("company", client);
            var response = await client.GetAsync("/api/applications/-123");
            var app = JsonConvert.DeserializeObject<StudentSessionApplication>((await response.Content.ReadAsStringAsync()));
            Assert.True(response.StatusCode.Equals(HttpStatusCode.NotFound), response.StatusCode.ToString());
        }

        [Fact]
        public async Task RespondToApplicationAsCompany()
        {
            var application = new WebApplicationFactory<Nexpo.Program>();
            var client = application.CreateClient();
            var token = await Login("company", client);

            //Ensure seed data is correct
            var response = await client.GetAsync("/api/applications/-2");
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
        public async Task RespondToApplicationAsCompanyNotExist()
        {
            var application = new WebApplicationFactory<Nexpo.Program>();
            var client = application.CreateClient();
            var token = await Login("company", client);

            //Update status
            var json = new JsonObject();
            json.Add("status", 1);
            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await client.PutAsync("api/applications/-123", payload);
            Assert.True(response.StatusCode.Equals(HttpStatusCode.NotFound), "Put application error code: " + response.StatusCode);
        }

        [Fact]
        public async Task RespondToAnotherCompanyApplication()
        {
            var application = new WebApplicationFactory<Nexpo.Program>();
            var client = application.CreateClient();
            var token = await Login("company", client);

            //Update status
            var json = new JsonObject();
            json.Add("status", 1);
            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await client.PutAsync("api/applications/-4", payload);
            Assert.True(response.StatusCode.Equals(HttpStatusCode.Forbidden), "Put application error code: " + response.StatusCode);
        }

        [Fact]
        public async Task RespondToApplicationAsStudent()
        {
            var application = new WebApplicationFactory<Nexpo.Program>();
            var client = application.CreateClient();
            var token = await Login("", client);

            //Update status
            var json = new JsonObject();
            json.Add("status", 1);
            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await client.PutAsync("api/applications/-1", payload);
            Assert.True(response.StatusCode.Equals(HttpStatusCode.Forbidden), "Put application error code: " + response.StatusCode);
        }

        [Fact]
        public async Task PostApplicationAsStudentThenDelete()
        {
            var application = new WebApplicationFactory<Nexpo.Program>();
            var studentClient = application.CreateClient();
            var companyClient = application.CreateClient();
            await Login("", studentClient);
            await Login("company3", companyClient);
            
            //Post application
            var json = new JsonObject();
            json.Add("motivation", "Hej, jag är jättebra och tror att ni vill träffa mig!");
            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await studentClient.PostAsync("api/applications/company/-3", payload);
            Assert.True(response.StatusCode.Equals(HttpStatusCode.Created), "Post application error code: " + response.StatusCode);

            //Check application as company
            response = await companyClient.GetAsync("/api/applications/my/company");
            var appList = JsonConvert.DeserializeObject<List<StudentSessionApplicationDto>>((await response.Content.ReadAsStringAsync()));
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), "Get as company error");
            Assert.True(appList.Count == 2, "Application list length should be 2, count:" + appList.Count.ToString());
            Assert.True(appList[1].Motivation == "Hej, jag är jättebra och tror att ni vill träffa mig!", "Wrong motivation, got: " + appList[1].Motivation);
            int id = appList[1].Id.GetValueOrDefault();

            //Restore
            response = await studentClient.DeleteAsync("api/applications/" + id);
            Assert.True(response.StatusCode.Equals(HttpStatusCode.NoContent), "Delete application error code: " + response.StatusCode);
            response = await companyClient.GetAsync("/api/applications/my/company");
            appList = JsonConvert.DeserializeObject<List<StudentSessionApplicationDto>>((await response.Content.ReadAsStringAsync()));
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), "Get as company error");
            Assert.True(appList.Count == 1, "Application list length should be 1, count:" + appList.Count.ToString());
        }

        [Fact]
        public async Task PostApplicationAsStudentNoTimeslots()
        {
            var application = new WebApplicationFactory<Nexpo.Program>();
            var studentClient = application.CreateClient();
            var companyClient = application.CreateClient();
            await Login("", studentClient);

            //Post application
            var json = new JsonObject();
            json.Add("motivation", "Hej, jag är jättebra och tror att ni vill träffa mig!");
            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await studentClient.PostAsync("api/applications/company/-4", payload);
            Assert.True(response.StatusCode.Equals(HttpStatusCode.Conflict), "Post application error code: " + response.StatusCode);
        }

        [Fact]
        public async Task UpdateApplicationAsStudent()
        {
            var application = new WebApplicationFactory<Nexpo.Program>();
            var studentClient = application.CreateClient();
            var companyClient = application.CreateClient();
            await Login("", studentClient);
            await Login("company", companyClient);

            //Post application
            var json = new JsonObject();
            json.Add("motivation", "This is a test");
            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await studentClient.PostAsync("api/applications/company/-1", payload);
            Assert.True(response.StatusCode.Equals(HttpStatusCode.Created), "Post application error code: " + response.StatusCode);

            //Check application as company
            response = await companyClient.GetAsync("/api/applications/my/company");
            var appList = JsonConvert.DeserializeObject<List<StudentSessionApplicationDto>>((await response.Content.ReadAsStringAsync()));
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), "Get as company error");
            Assert.True(appList.Count == 3, "Application list length should be 3, count:" + appList.Count.ToString());
            Assert.True(appList[2].Motivation == "This is a test", "Wrong motivation, got: " + appList[2].Motivation);
            int id = appList[2].Id.GetValueOrDefault();

            //Restore
            json = new JsonObject();
            json.Add("motivation", "Hej, jag är jättebra och tror att ni vill träffa mig!");
            payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            response = await studentClient.PostAsync("api/applications/company/-1", payload);
            Assert.True(response.StatusCode.Equals(HttpStatusCode.Created), "Post application error code: " + response.StatusCode);

            response = await companyClient.GetAsync("/api/applications/my/company");
            appList = JsonConvert.DeserializeObject<List<StudentSessionApplicationDto>>((await response.Content.ReadAsStringAsync()));
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), "Get as company error");
            Assert.True(appList.Count == 3, "Application list length should be 3, count:" + appList.Count.ToString());
            Assert.True(appList[2].Motivation == "Hej, jag är jättebra och tror att ni vill träffa mig!", "Wrong motivation, got: " + appList[2].Motivation);
            id = appList[2].Id.GetValueOrDefault();
        }


        [Fact]
        public async Task GetApplicationAccepted()
        {
            var application = new WebApplicationFactory<Nexpo.Program>();
            var client = application.CreateClient();
            await Login("", client);

            var response = await client.GetAsync("/api/applications/accepted/-1");
            var status = JsonConvert.DeserializeObject<ApplicationStatusDto>((await response.Content.ReadAsStringAsync()));
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), "Get error code: ");
            Assert.True(!status.accepted, "Accepted should be false");
        }

        [Fact]
        public async Task GetApplicationAcceptedForNonAppliedCompany()
        {
            var application = new WebApplicationFactory<Nexpo.Program>();
            var client = application.CreateClient();
            await Login("", client);

            var response = await client.GetAsync("/api/applications/accepted/-4");
            Assert.True(response.StatusCode.Equals(HttpStatusCode.BadRequest), response.StatusCode.ToString());
        }

        [Fact]
        public async Task GetApplicationAcceptedWhenAccepted()
        {
            var application = new WebApplicationFactory<Nexpo.Program>();
            var studentClient = application.CreateClient();
            var companyClient = application.CreateClient();
            await Login("", studentClient);
            await Login("company", companyClient);

            //Update status
            var json = new JsonObject();
            json.Add("status", 1);
            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await companyClient.PutAsync("api/applications/-1", payload);
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), "Put application error code: " + response.StatusCode);

            response = await studentClient.GetAsync("/api/applications/accepted/-1");
            var status = JsonConvert.DeserializeObject<ApplicationStatusDto>((await response.Content.ReadAsStringAsync()));
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), "Get error code: ");
            Assert.True(status.accepted, "Accepted should be true");

            //Roll back
            json = new JsonObject();
            json.Add("status", 0);
            payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            response = await companyClient.PutAsync("api/applications/-1", payload);
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), "Get application error code: " + response.StatusCode);
        }

        [Fact]
        public async Task GetApplicationAcceptedWhenDeclined()
        {
            var application = new WebApplicationFactory<Nexpo.Program>();
            var studentClient = application.CreateClient();
            var companyClient = application.CreateClient();
            await Login("", studentClient);
            await Login("company", companyClient);

            //Update status
            var json = new JsonObject();
            json.Add("status", 2);
            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await companyClient.PutAsync("api/applications/-1", payload);
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), "Put application error code: " + response.StatusCode);

            response = await studentClient.GetAsync("/api/applications/accepted/-1");
            var status = JsonConvert.DeserializeObject<ApplicationStatusDto>((await response.Content.ReadAsStringAsync()));
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), "Get error code: ");
            Assert.True(!status.accepted, "Accepted should be false");

            //Roll back
            json = new JsonObject();
            json.Add("status", 0);
            payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            response = await companyClient.PutAsync("api/applications/-1", payload);
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), "Get application error code: " + response.StatusCode);
        }
    }
}
