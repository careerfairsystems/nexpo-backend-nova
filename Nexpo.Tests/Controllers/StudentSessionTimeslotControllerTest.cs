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
    public class StudentSessionTimeslotControllerTest
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

        public static async Task<HttpClient> CompanyClient()
        {
            //Create client and login
            var application = new WebApplicationFactory<Nexpo.Program>();
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
        public async Task GetAllTimeslotsByCompanyId()
        {
            var client = await StudentClient();
            var response = await client.GetAsync("/api/timeslots/company/1");

            string responseText = await response.Content.ReadAsStringAsync();
            var responseList = JsonConvert.DeserializeObject<List<StudentSessionTimeslot>>(responseText);
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK));
            Assert.True(responseList.Count == 3, responseText.ToString());

            var timeslot1 = responseList.Find(r => r.Id == 1);
            var timeslot2 = responseList.Find(r => r.Id == 2);
            var timeslot3 = responseList.Find(r => r.Id == 3);


            Assert.True(timeslot1.Start == DateTime.Parse("2021-11-21 10:00"), timeslot1.Start.ToString());
            Assert.True(timeslot2.End == DateTime.Parse("2021-11-21 10:30"), timeslot2.End.ToString());
            Assert.True(timeslot3.Location == "Zoom", timeslot3.Location);
        }

        [Fact]
        public async Task GetSingleTimeslotsById()
        {
            var client = await StudentClient();
            var response = await client.GetAsync("/api/timeslots/4");

            string responseText = await response.Content.ReadAsStringAsync();
            var timeslot = JsonConvert.DeserializeObject<StudentSessionTimeslot>(responseText);
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK));

            Assert.True(timeslot.Start == DateTime.Parse("2021-11-22 11:00"), timeslot.Start.ToString());
            Assert.True(timeslot.End == DateTime.Parse("2021-11-22 11:15"), timeslot.End.ToString());
            Assert.True(timeslot.Location == "Zoom", timeslot.Location);
        }
    }
}
