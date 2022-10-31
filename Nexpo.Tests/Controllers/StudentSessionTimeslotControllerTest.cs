using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
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
    public class StudentSessionTimeslotControllerTest
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
                case "student2":
                    json.Add("email", "student2@example.com");
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
        public async Task GetAllTimeslotsByCompanyId()
        {
            var application = new WebApplicationFactory<Nexpo.Program>();
            var client = application.CreateClient();
            await Login("", client);

            var response = await client.GetAsync("/api/timeslots/company/-1");
            var responseList = JsonConvert.DeserializeObject<List<StudentSessionTimeslot>>((await response.Content.ReadAsStringAsync()));
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), response.StatusCode.ToString());
            Assert.True(responseList.Count == 3, "Wrong number of timeslots, got: " + responseList.Count.ToString());
        }

        [Fact]
        public async Task GetAllTimeslotsByCompanyIdNotExist()
        {
            var application = new WebApplicationFactory<Nexpo.Program>();
            var client = application.CreateClient();
            await Login("", client);

            var response = await client.GetAsync("/api/timeslots/company/-4");
            var responseList = JsonConvert.DeserializeObject<List<StudentSessionTimeslot>>((await response.Content.ReadAsStringAsync()));
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), response.StatusCode.ToString());
            Assert.True(responseList.Count == 0, "Wrong number of timeslots, got: " + responseList.Count.ToString());
        }

        [Fact]
        public async Task GetSingleTimeslot()
        {
            var application = new WebApplicationFactory<Nexpo.Program>();
            var client = application.CreateClient();
            await Login("", client);

            var response = await client.GetAsync("/api/timeslots/-1");
            var app = JsonConvert.DeserializeObject<StudentSessionTimeslot>((await response.Content.ReadAsStringAsync()));
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), response.StatusCode.ToString());
            Assert.True(app.Start == DateTime.Parse("2021-11-21 10:00"), "Wrong time, got: " + app.Start);
        }

        [Fact]
        public async Task GetNonExistingTimeslot()
        {
            var application = new WebApplicationFactory<Nexpo.Program>();
            var client = application.CreateClient();
            await Login("", client);

            var response = await client.GetAsync("/api/timeslots/-123");
            var app = JsonConvert.DeserializeObject<StudentSessionTimeslot>((await response.Content.ReadAsStringAsync()));
            Assert.True(response.StatusCode.Equals(HttpStatusCode.NotFound), response.StatusCode.ToString());
        }

        [Fact]
        public async Task GetAllCompaniesThatHaveTimeslots()
        {
            var application = new WebApplicationFactory<Nexpo.Program>();
            var client = application.CreateClient();
            await Login("", client);

            var response = await client.GetAsync("/api/timeslots/companies");
            var responseList = JsonConvert.DeserializeObject<List<PublicCompanyDto>>((await response.Content.ReadAsStringAsync()));
            var app3 = responseList.Find(r => r.Id == -1);

            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), response.StatusCode.ToString());
            Assert.True(responseList.Count == 3, "Wrong count, got: " + responseList.Count);
            Assert.True(app3.Name == "Apple", "Wrong name! Got: " + app3.Name);
        }

        [Fact]
        public async Task BookTimeslotForNonExistingCompany()
        {
            var application = new WebApplicationFactory<Nexpo.Program>();
            var client = application.CreateClient();
            await Login("", client);

            var payload = new StringContent("", Encoding.UTF8, "application/json");
            var response = await client.PutAsync("/api/timeslots/book/-123", payload);
            Assert.True(response.StatusCode.Equals(HttpStatusCode.NotFound), response.StatusCode.ToString());
        }

        [Fact]
        public async Task BookTimeslotNoApplication()
        {
            var application = new WebApplicationFactory<Nexpo.Program>();
            var client = application.CreateClient();
            await Login("", client);

            var payload = new StringContent("", Encoding.UTF8, "application/json");
            var response = await client.PutAsync("/api/timeslots/book/-3", payload);
            Assert.True(response.StatusCode.Equals(HttpStatusCode.BadRequest), response.StatusCode.ToString());
        }

        [Fact]
        public async Task BookTimeslotNotAccepted()
        {
            var application = new WebApplicationFactory<Nexpo.Program>();
            var client = application.CreateClient();
            await Login("", client);

            var payload = new StringContent("", Encoding.UTF8, "application/json");
            var response = await client.PutAsync("/api/timeslots/book/-1", payload);
            Assert.True(response.StatusCode.Equals(HttpStatusCode.BadRequest), response.StatusCode.ToString());
        }

        [Fact]
        public async Task UnbookTimeslotNotExisting()
        {
            var application = new WebApplicationFactory<Nexpo.Program>();
            var client = application.CreateClient();
            await Login("", client);

            var payload = new StringContent("", Encoding.UTF8, "application/json");
            var response = await client.PutAsync("/api/timeslots/unbook/-123", payload);
            Assert.True(response.StatusCode.Equals(HttpStatusCode.NotFound), response.StatusCode.ToString());
        }

        [Fact]
        public async Task UnbookTimeslotNoApplication()
        {
            var application = new WebApplicationFactory<Nexpo.Program>();
            var client = application.CreateClient();
            await Login("", client);

            var payload = new StringContent("", Encoding.UTF8, "application/json");
            var response = await client.PutAsync("/api/timeslots/book/-7", payload);
            Assert.True(response.StatusCode.Equals(HttpStatusCode.BadRequest), response.StatusCode.ToString());
        }

        [Fact]
        public async Task UnbookTimeslotNotAccepted()
        {
            var application = new WebApplicationFactory<Nexpo.Program>();
            var client = application.CreateClient();
            await Login("", client);

            var payload = new StringContent("", Encoding.UTF8, "application/json");
            var response = await client.PutAsync("/api/timeslots/book/-1", payload);
            Assert.True(response.StatusCode.Equals(HttpStatusCode.BadRequest), response.StatusCode.ToString());
        }

        [Fact]
        public async Task BookAndUnbookTimeslot()
        {
            var application = new WebApplicationFactory<Nexpo.Program>();
            var studentClient = application.CreateClient();
            var companyClient = application.CreateClient();
            await Login("", studentClient);
            await Login("company", companyClient);

            //Accept student application
            var json = new JsonObject();
            json.Add("status", 1);
            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await companyClient.PutAsync("api/applications/-1", payload);
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), "Put application error code: " + response.StatusCode);

            //Book studentsession as student
            payload = new StringContent("", Encoding.UTF8, "application/json");
            response = await studentClient.PutAsync("/api/timeslots/book/-1", payload);
            var responseObject = JsonConvert.DeserializeObject<StudentSessionTimeslot>((await response.Content.ReadAsStringAsync()));
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), response.StatusCode.ToString());
            Assert.True(responseObject.Location == "Zoom", responseObject.Location.ToString());
            Assert.True(responseObject.StudentId == -1, responseObject.StudentId.ToString());

            //Unbook studentsession as student
            payload = new StringContent("", Encoding.UTF8, "application/json");
            response = await studentClient.PutAsync("/api/timeslots/unbook/-1", payload);
            responseObject = JsonConvert.DeserializeObject<StudentSessionTimeslot>((await response.Content.ReadAsStringAsync()));
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), response.StatusCode.ToString());
            Assert.True(responseObject.Location == "Zoom", responseObject.Location.ToString());
            Assert.True(responseObject.StudentId == null, responseObject.StudentId.ToString());

            //Restore student application
            json = new JsonObject();
            json.Add("status", 0);
            payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            response = await companyClient.PutAsync("api/applications/-1", payload);
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), "Put application error code: " + response.StatusCode);
        }

        [Fact]
        public async Task BookMultipleTimeslots()
        {
            var application = new WebApplicationFactory<Nexpo.Program>();
            var studentClient = application.CreateClient();
            var companyClient = application.CreateClient();
            await Login("", studentClient);
            await Login("company", companyClient);

            //Accept student application
            var json = new JsonObject();
            json.Add("status", 1);
            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await companyClient.PutAsync("api/applications/-1", payload);
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), "Put application error code: " + response.StatusCode);

            //Book studentsession as student
            payload = new StringContent("", Encoding.UTF8, "application/json");
            response = await studentClient.PutAsync("/api/timeslots/book/-1", payload);
            var responseObject = JsonConvert.DeserializeObject<StudentSessionTimeslot>((await response.Content.ReadAsStringAsync()));
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), response.StatusCode.ToString());
            Assert.True(responseObject.Location == "Zoom", responseObject.Location.ToString());
            Assert.True(responseObject.StudentId == -1, responseObject.StudentId.ToString());

            //Book another studentsession as student
            payload = new StringContent("", Encoding.UTF8, "application/json");
            response = await studentClient.PutAsync("/api/timeslots/book/-2", payload);
            Assert.True(response.StatusCode.Equals(HttpStatusCode.BadRequest), response.StatusCode.ToString());

            //Unbook studentsession as student
            payload = new StringContent("", Encoding.UTF8, "application/json");
            response = await studentClient.PutAsync("/api/timeslots/unbook/-1", payload);
            responseObject = JsonConvert.DeserializeObject<StudentSessionTimeslot>((await response.Content.ReadAsStringAsync()));
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), response.StatusCode.ToString());
            Assert.True(responseObject.Location == "Zoom", responseObject.Location.ToString());
            Assert.True(responseObject.StudentId == null, responseObject.StudentId.ToString());

            //Restore student application
            json = new JsonObject();
            json.Add("status", 0);
            payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            response = await companyClient.PutAsync("api/applications/-1", payload);
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), "Put application error code: " + response.StatusCode);
        }

        [Fact]
        public async Task UnbookTimeslotNoStudent()
        {
            var application = new WebApplicationFactory<Nexpo.Program>();
            var studentClient = application.CreateClient();
            var companyClient = application.CreateClient();
            await Login("", studentClient);
            await Login("company", companyClient);

            //Accept student application
            var json = new JsonObject();
            json.Add("status", 1);
            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await companyClient.PutAsync("api/applications/-1", payload);
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), "Put application error code: " + response.StatusCode);

            //Unbook studentsession as student
            payload = new StringContent("", Encoding.UTF8, "application/json");
            response = await studentClient.PutAsync("/api/timeslots/unbook/-1", payload);
            var responseObject = JsonConvert.DeserializeObject<StudentSessionTimeslot>((await response.Content.ReadAsStringAsync()));
            Assert.True(response.StatusCode.Equals(HttpStatusCode.BadRequest), response.StatusCode.ToString());

            //Restore student application
            json = new JsonObject();
            json.Add("status", 0);
            payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            response = await companyClient.PutAsync("api/applications/-1", payload);
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), "Put application error code: " + response.StatusCode);
        }

        [Fact]
        public async Task UnbookTimeslotNotBooked()
        {
            var application = new WebApplicationFactory<Nexpo.Program>();
            var studentClient = application.CreateClient();
            var studentClient2 = application.CreateClient();
            var companyClient = application.CreateClient();
            await Login("", studentClient);
            await Login("student2", studentClient2);
            await Login("company", companyClient);

            //Accept student applications
            var json = new JsonObject();
            json.Add("status", 1);
            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await companyClient.PutAsync("api/applications/-1", payload);
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), "Put application error code: " + response.StatusCode);
            response = await companyClient.PutAsync("api/applications/-2", payload);
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), "Put application error code: " + response.StatusCode);

            //Book studentsession as student1
            payload = new StringContent("", Encoding.UTF8, "application/json");
            response = await studentClient.PutAsync("/api/timeslots/book/-1", payload);
            var responseObject = JsonConvert.DeserializeObject<StudentSessionTimeslot>((await response.Content.ReadAsStringAsync()));
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), response.StatusCode.ToString());
            Assert.True(responseObject.Location == "Zoom", responseObject.Location.ToString());
            Assert.True(responseObject.StudentId == -1, responseObject.StudentId.ToString());

            //Unbook same studentsession as student2
            payload = new StringContent("", Encoding.UTF8, "application/json");
            response = await studentClient.PutAsync("/api/timeslots/unbook/-2", payload);
            Assert.True(response.StatusCode.Equals(HttpStatusCode.BadRequest), response.StatusCode.ToString());

            //Unbook studentsession as student
            payload = new StringContent("", Encoding.UTF8, "application/json");
            response = await studentClient.PutAsync("/api/timeslots/unbook/-1", payload);
            responseObject = JsonConvert.DeserializeObject<StudentSessionTimeslot>((await response.Content.ReadAsStringAsync()));
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), response.StatusCode.ToString());
            Assert.True(responseObject.Location == "Zoom", responseObject.Location.ToString());
            Assert.True(responseObject.StudentId == null, responseObject.StudentId.ToString());

            //Restore student application
            json = new JsonObject();
            json.Add("status", 0);
            payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            response = await companyClient.PutAsync("api/applications/-1", payload);
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), "Put application error code: " + response.StatusCode);
            response = await companyClient.PutAsync("api/applications/-2", payload);
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), "Put application error code: " + response.StatusCode);
        }

        [Fact]
        public async Task BookTimeslotOnAlreadyBooked()
        {
            var application = new WebApplicationFactory<Nexpo.Program>();
            var studentClient = application.CreateClient();
            var studentClient2 = application.CreateClient();
            var companyClient = application.CreateClient();
            await Login("", studentClient);
            await Login("student2", studentClient2);
            await Login("company", companyClient);

            //Accept student applications
            var json = new JsonObject();
            json.Add("status", 1);
            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await companyClient.PutAsync("api/applications/-1", payload);
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), "Put application error code: " + response.StatusCode);
            response = await companyClient.PutAsync("api/applications/-2", payload);
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), "Put application error code: " + response.StatusCode);

            //Book studentsession as student1
            payload = new StringContent("", Encoding.UTF8, "application/json");
            response = await studentClient.PutAsync("/api/timeslots/book/-1", payload);
            var responseObject = JsonConvert.DeserializeObject<StudentSessionTimeslot>((await response.Content.ReadAsStringAsync()));
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), response.StatusCode.ToString());
            Assert.True(responseObject.Location == "Zoom", responseObject.Location.ToString());
            Assert.True(responseObject.StudentId == -1, responseObject.StudentId.ToString());

            //Unbook same studentsession as student2
            payload = new StringContent("", Encoding.UTF8, "application/json");
            response = await studentClient.PutAsync("/api/timeslots/book/-2", payload);
            Assert.True(response.StatusCode.Equals(HttpStatusCode.BadRequest), response.StatusCode.ToString());

            //Unbook studentsession as student
            payload = new StringContent("", Encoding.UTF8, "application/json");
            response = await studentClient.PutAsync("/api/timeslots/unbook/-1", payload);
            responseObject = JsonConvert.DeserializeObject<StudentSessionTimeslot>((await response.Content.ReadAsStringAsync()));
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), response.StatusCode.ToString());
            Assert.True(responseObject.Location == "Zoom", responseObject.Location.ToString());
            Assert.True(responseObject.StudentId == null, responseObject.StudentId.ToString());

            //Restore student application
            json = new JsonObject();
            json.Add("status", 0);
            payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            response = await companyClient.PutAsync("api/applications/-1", payload);
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), "Put application error code: " + response.StatusCode);
            response = await companyClient.PutAsync("api/applications/-2", payload);
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), "Put application error code: " + response.StatusCode);
        }

        [Fact]
        public async Task CreateNewTimeslotAndDelete()
        {
            var application = new WebApplicationFactory<Nexpo.Program>();
            var client = application.CreateClient();
            await Login("admin", client);

            //Add new timeslot
            var json = new JsonObject();
       //   json.Add("start", "2021-11-15 12:45");
       //   json.Add("end", "2021-11-15 13:15");
            json.Add("companyid", "-1");
            json.Add("location", "At home");

            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("api/timeslots/add", payload);
            var responseObject = JsonConvert.DeserializeObject<StudentSessionTimeslot>(await response.Content.ReadAsStringAsync());
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), "Post application error code: " + response.StatusCode);
            Assert.True(responseObject.Location == "At home", responseObject.Location.ToString());
            Assert.True(responseObject.StudentId == null, responseObject.StudentId.ToString());

            //Remove new timeslot
            response = await client.DeleteAsync("api/timeslots/" + responseObject.Id);
            Assert.True(response.StatusCode.Equals(HttpStatusCode.NoContent), "Delete studentsession error code: " + response.StatusCode);
           
            response = await client.GetAsync("/api/timeslots/company/-1");
            var responseList = JsonConvert.DeserializeObject<List<StudentSessionTimeslot>>((await response.Content.ReadAsStringAsync()));
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), response.StatusCode.ToString());
            Assert.True(responseList.Count == 3, "Wrong number of timeslots, got: " + responseList.Count.ToString());

        }

        [Fact]
        public async Task DeleteNonExistingTimeslot()
        {
            var application = new WebApplicationFactory<Nexpo.Program>();
            var client = application.CreateClient();
            await Login("admin", client);

            var response = await client.DeleteAsync("api/timeslots/-123");
            Assert.True(response.StatusCode.Equals(HttpStatusCode.NotFound), "Delete studentsession error code: " + response.StatusCode);
        }

    }
}
