using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Nexpo.Controllers;
using Nexpo.Models;
using Nexpo.Repositories;
using Nexpo.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Xunit;

namespace Nexpo.Tests.Controllers
{ 
    public class UserControllerTest
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
                    json.Add("password", "password123");
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
        public async Task AdminGetAllUsers()
        {
            var application = new WebApplicationFactory<Nexpo.Program>();
            var client = application.CreateClient();
            var token = await Login("admin", client);
            var response = await client.GetAsync("/api/users/");

            string responseText = await response.Content.ReadAsStringAsync();
            var responseList = JsonConvert.DeserializeObject<List<User>>(responseText);
            Assert.True(responseList.Count == 9, responseText.ToString());

            var userAdmin = responseList.Find(r => r.Id == -1);
            var userStudent = responseList.Find(r => r.Id == -2);
            var userRep = responseList.Find(r => r.Id == -5);

            Assert.True(userAdmin.Role.Equals(Role.Administrator), userAdmin.Role.ToString());
            Assert.True(userStudent.FirstName == "Alpha", userStudent.FirstName);
            Assert.True(userRep.CompanyId == -1, userRep.CompanyId.ToString());
        }

        [Fact]
        public async Task StudentGetAllUsers()
        {
            var application = new WebApplicationFactory<Nexpo.Program>();
            var client = application.CreateClient();
            var token = await Login("", client);
            var response = await client.GetAsync("/api/users/");

            string responseText = await response.Content.ReadAsStringAsync();
            var responseList = JsonConvert.DeserializeObject<List<User>>(responseText);
            Assert.True(response.StatusCode.Equals(HttpStatusCode.Forbidden), response.StatusCode.ToString());
        }

        [Fact]
        public async Task AdminGetUser()
        {
            var application = new WebApplicationFactory<Nexpo.Program>();
            var client = application.CreateClient();
            var token = await Login("admin", client);
            var response = await client.GetAsync("/api/users/-5");

            string responseText = await response.Content.ReadAsStringAsync();
            var responseUser = JsonConvert.DeserializeObject<User>(responseText);
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), response.StatusCode.ToString());
            Assert.True(responseUser.CompanyId == -1, responseUser.CompanyId.ToString());
            Assert.True(responseUser.Email == "rep1@company1.example.com", responseUser.Email);
            Assert.True(responseUser.Role.Equals(Role.CompanyRepresentative), responseUser.Role.ToString());
        }

        [Fact]
        public async Task CompanyGetUserLegit()
        {
            var application = new WebApplicationFactory<Nexpo.Program>();
            var client = application.CreateClient();
            var token = await Login("company", client);
            var response = await client.GetAsync("/api/users/-2");

            string responseText = await response.Content.ReadAsStringAsync();
            var responseUser = JsonConvert.DeserializeObject<User>(responseText);
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), response.StatusCode.ToString());
            Assert.True(responseUser.FirstName == "Alpha", responseUser.FirstName);
            Assert.True(responseUser.Email == "student1@example.com", responseUser.Email);
            Assert.True(responseUser.Role.Equals(Role.Student), responseUser.Role.ToString());
        }

        [Fact]
        public async Task CompanyGetUserNotStudent()
        {
            var application = new WebApplicationFactory<Nexpo.Program>();
            var client = application.CreateClient();
            var token = await Login("company", client);
            var response = await client.GetAsync("/api/users/-6");

            string responseText = await response.Content.ReadAsStringAsync();
            var responseUser = JsonConvert.DeserializeObject<User>(responseText);
            Assert.True(response.StatusCode.Equals(HttpStatusCode.Forbidden), response.StatusCode.ToString());
        }

        [Fact]
        public async Task CompanyGetUserNoStudentApplication()
        {
            var application = new WebApplicationFactory<Nexpo.Program>();
            var client = application.CreateClient();
            var token = await Login("company3", client);
            var response = await client.GetAsync("/api/users/-2");

            string responseText = await response.Content.ReadAsStringAsync();
            var responseUser = JsonConvert.DeserializeObject<User>(responseText);
            Assert.True(response.StatusCode.Equals(HttpStatusCode.Forbidden), response.StatusCode.ToString());
        }

        [Fact]
        public async Task StudentGetUser()
        {
            var application = new WebApplicationFactory<Nexpo.Program>();
            var client = application.CreateClient();
            var token = await Login("", client);
            var response = await client.GetAsync("/api/users/-2");

            string responseText = await response.Content.ReadAsStringAsync();
            var responseUser = JsonConvert.DeserializeObject<User>(responseText);
            Assert.True(response.StatusCode.Equals(HttpStatusCode.Forbidden), response.StatusCode.ToString());
        }

        [Fact]
        public async Task AdminUpdateUser()
        {
            var application = new WebApplicationFactory<Nexpo.Program>();
            var client = application.CreateClient();
            var token = await Login("admin", client);

            var json = new JsonObject();
            json.Add("firstName", "Rakel");
            json.Add("lastName", "Spektakel");
            json.Add("password", "superdupersecret");

            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await client.PutAsync("api/users/-2", payload);
            var responseObject = JsonConvert.DeserializeObject<User>((await response.Content.ReadAsStringAsync()));

            //Assertions of response
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), response.ToString());
            Assert.True(responseObject.Id == -2, responseObject.Id.ToString());
            Assert.True(responseObject.FirstName == "Rakel", responseObject.FirstName);
            Assert.True(responseObject.LastName == "Spektakel", responseObject.LastName);
            Assert.True(responseObject.Role.Equals(Role.Student), responseObject.Role.ToString());
            Assert.True(responseObject.PhoneNr == "", responseObject.PhoneNr);
            Assert.True(responseObject.FoodPreferences == "", responseObject.FoodPreferences);

            //Sign-in with old password
            var testClient = application.CreateClient();
            var testJson = new JsonObject();
            testJson.Add("email", "student1@example.com");
            testJson.Add("password", "password");
            var testPayload = new StringContent(testJson.ToString(), Encoding.UTF8, "application/json");
            var testResponse = await testClient.PostAsync("/api/session/signin", testPayload);
            Assert.True(response.StatusCode.Equals(HttpStatusCode.BadRequest), "Unauthorized Login, returned: " + testResponse.StatusCode.ToString());

            //Sign-in with new password
            var test2Client = application.CreateClient();
            var test2Json = new JsonObject();
            test2Json.Add("email", "student1@example.com");
            test2Json.Add("password", "superdupersecret");
            var test2Payload = new StringContent(test2Json.ToString(), Encoding.UTF8, "application/json");
            var test2Response = await test2Client.PostAsync("/api/session/signin", test2Payload);
            Assert.True(test2Response.StatusCode.Equals(HttpStatusCode.OK), "Login failed, returned: " + test2Response.StatusCode.ToString());

            //Restore
            var json2 = new JsonObject();
            json2.Add("firstName", "Alpha");
            json2.Add("lastName", "Student");
            json2.Add("password", "password123");

            var payload2 = new StringContent(json2.ToString(), Encoding.UTF8, "application/json");
            var response2 = await client.PutAsync("api/users/-2", payload2);
            var responseObject2 = JsonConvert.DeserializeObject<User>((await response2.Content.ReadAsStringAsync()));

            //Verify Restore
            Assert.True(response2.StatusCode.Equals(HttpStatusCode.OK), response.ToString());
            Assert.True(responseObject2.Id == -2, responseObject.Id.ToString());
            Assert.True(responseObject2.FirstName == "Alpha", responseObject.FirstName);
            Assert.True(responseObject2.LastName == "Student", responseObject.LastName);
            Assert.True(responseObject2.Role.Equals(Role.Student), responseObject.Role.ToString());
            Assert.True(responseObject2.PhoneNr == "", responseObject.PhoneNr);
            Assert.True(responseObject2.FoodPreferences == "", responseObject.FoodPreferences);

            //Sign-in with new password
            var verifyClient = application.CreateClient();
            var verifyJson = new JsonObject();
            verifyJson.Add("email", "student1@example.com");
            verifyJson.Add("password", "password");
            var verifyPayload = new StringContent(verifyJson.ToString(), Encoding.UTF8, "application/json");
            var verifyResponse = await verifyClient.PostAsync("/api/session/signin", verifyPayload);
            Assert.True(verifyResponse.StatusCode.Equals(HttpStatusCode.OK), "Login failed, returned: " + verifyResponse.StatusCode.ToString());

        }



    }
}
