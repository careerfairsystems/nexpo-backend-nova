﻿using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Nexpo.Models;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Xunit;

namespace Nexpo.Tests.Controllers
{ 
    /// <summary>
    /// This class contains tests for the UserController
    /// To see workflow of tests, see the comments in the test UpdateRoleOfVolenteer()
    /// </summary>
    public class UserControllerTest
    {
        [Fact]
        public async Task UpdateRoleOfVolenteer()
        {
            //Setup
            //Login as volenteer
            var client = await TestUtils.Login("volenteer"); 

            //Create json payload with new role
            var json = new JsonObject
            {
                { "role", "CompanyRepresentative" }
            }; 
            
            //Create payload from json
            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json"); 

            //Send a PUT request to update the user with id -10 with the payload. 
            //Note that in the ApplicationDBContext the volenteer has id -10
            var response = await client.PutAsync("api/users/-10", payload); 
            
            //Assertions of response, meaning
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), "Wrong status code. Expected: OK. Received: " + response.StatusCode.ToString());

            //Restore
            var json2 = new JsonObject
            {
                { "role", "Volunteer" }
            };

            //change role back to Volenteer
            var payload2 = new StringContent(json2.ToString(), Encoding.UTF8, "application/json");
            var response2 = await client.PutAsync("api/users/-10", payload2);
            Assert.True(response2.StatusCode.Equals(HttpStatusCode.OK), "Wrong status code. Expected: OK. Received: " + response2.StatusCode.ToString());

            //Assertions of response
            //Verify that the role has been changed
            //await response.Content.ReadAsStringAsync() returns the response body as a string
            //JsonConvert.DeserializeObject<User> deserializes the string to a User object (meaning that the response body is a json object)
            var responseObject = JsonConvert.DeserializeObject<User>(await response.Content.ReadAsStringAsync());

            //Verify that the role has been changed
            //Takes the information in the response body and compares it to the expected value
            Assert.True(responseObject.Role.Equals(Role.CompanyRepresentative), "Wrong role. Expected: 2. Received: " + responseObject.Role.ToString());

            //Verify that the role has been changed back
            var responseObject2 = JsonConvert.DeserializeObject<User>(await response2.Content.ReadAsStringAsync());
            Assert.True(responseObject2.Role.Equals(Role.Volunteer), "Wrong role. Expected: 3. Received: " + responseObject2.Role.ToString());
        }

        [Fact]
        public async Task AdminGetAllUsers()
        {
            var client = await TestUtils.Login("admin");
            var response = await client.GetAsync("/api/users/");
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), "Wrong Status Code. Expected: OK. Received: " + response.StatusCode.ToString());

            var responseList = JsonConvert.DeserializeObject<List<User>>(await response.Content.ReadAsStringAsync());
            var userAdmin = responseList.Find(r => r.Id == -1);
            var userStudent = responseList.Find(r => r.Id == -2);
            var userRep = responseList.Find(r => r.Id == -5);
            
            Assert.True(responseList.Count == 9, "Wrong number of users. Expected: 9. Received: " + responseList.Count.ToString());
            Assert.True(userAdmin.Role.Equals(Role.Administrator), "Wrong user role. Expected: admin. Received: " + userAdmin.Role.ToString());
            Assert.True(userStudent.FirstName.Equals("Alpha"), "Wrong user first name. Expected: Alpha. Received: " +  userStudent.FirstName);
            Assert.True(userRep.CompanyId == -1, "Wrong company id. Expected: -1. Received: " + userRep.CompanyId.ToString());
        }

        [Fact]
        public async Task StudentGetAllUsers()
        {
            var client =  await TestUtils.Login("student1");
            var response = await client.GetAsync("/api/users/");

            Assert.True(response.StatusCode.Equals(HttpStatusCode.Forbidden), "Wrong Status Code. Expected: Forbidden. Received: " + response.StatusCode.ToString());
        }

        [Fact]
        public async Task AdminGetUser()
        {
            var client = await TestUtils.Login("admin");
            var response = await client.GetAsync("/api/users/-5");
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), "Wrong Status Code. Expected: OK. Received: " + response.StatusCode.ToString());

            var responseUser = JsonConvert.DeserializeObject<User>(await response.Content.ReadAsStringAsync());
            Assert.True(responseUser.CompanyId == -1, "Wrong company id. Expected: -1. Received: " + responseUser.CompanyId.ToString());
            Assert.True(responseUser.Email.Equals("rep1@company1.example.com"), "Wrong email. Expected: rep1@company1.example.com. Received: " + responseUser.Email);
            Assert.True(responseUser.Role.Equals(Role.CompanyRepresentative), "Wrong user role. Expected: CompanyRepresentative. Received: " + responseUser.Role.ToString());
        }

        [Fact]
        public async Task CompanyGetUserLegit()
        {
            var client = await TestUtils.Login("company1");
            var response = await client.GetAsync("/api/users/-2");
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), "Wrong Status Code. Expected: OK. Received: " + response.StatusCode.ToString());

            var responseUser = JsonConvert.DeserializeObject<User>(await response.Content.ReadAsStringAsync());
            Assert.True(responseUser.FirstName.Equals("Alpha"), "Wrong user first name. Expected: Alpha. Received: " + responseUser.FirstName);
            Assert.True(responseUser.Email.Equals("student1@example.com"), "Wrong email. Expected: student1@example.com. Received: " + responseUser.Email);
            Assert.True(responseUser.Role.Equals(Role.Student), "Wrong user role. Expected: Student. Received: " + responseUser.Role.ToString());
        }

        [Fact]
        public async Task CompanyGetUserNotStudent()
        {
            var client = await TestUtils.Login("company1");
            var response = await client.GetAsync("/api/users/-6");

            Assert.True(response.StatusCode.Equals(HttpStatusCode.Forbidden), "Wrong Status Code. Expected: Forbidden. Received: " + response.StatusCode.ToString());
        }

        [Fact]
        public async Task CompanyGetUserNoStudentApplication()
        {
            var client =  await TestUtils.Login("company3");
            var response = await client.GetAsync("/api/users/-2");

            Assert.True(response.StatusCode.Equals(HttpStatusCode.Forbidden), "Wrong Status Code. Expected: Forbidden. Received: " + response.StatusCode.ToString());
        }

        [Fact]
        public async Task StudentGetUser()
        {
            var client =  await TestUtils.Login("student1");
            var response = await client.GetAsync("/api/users/-2");

            Assert.True(response.StatusCode.Equals(HttpStatusCode.Forbidden), "Wrong Status Code. Expected: Forbidden. Received: " + response.StatusCode.ToString());
        }

        [Fact]
        public async Task AdminUpdateUser()
        {
            var application = new WebApplicationFactory<Program>();
            var client =  await TestUtils.Login("admin");
            var json = new JsonObject
            {
                { "firstName", "Rakel" },
                { "password", "superdupersecret" }
            };
            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await client.PutAsync("api/users/-6", payload);
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), "Wrong Status Code. Expected: OK. Received: " + response.ToString());

            //Sign-in with old password
            var testClient = application.CreateClient();
            var testJson = new JsonObject
            {
                { "email", "rep2@company1.example.com" },
                { "password", "password" }
            };
            var testPayload = new StringContent(testJson.ToString(), Encoding.UTF8, "application/json");
            var testResponse = await testClient.PostAsync("/api/session/signin", testPayload);
            Assert.True(testResponse.StatusCode.Equals(HttpStatusCode.BadRequest), "Wrong Status Code. Expected: BadRequest. Received: " + testResponse.StatusCode.ToString());

            //Sign-in with new password
            var test2Client = application.CreateClient();
            var test2Json = new JsonObject
            {
                { "email", "rep2@company1.example.com" },
                { "password", "superdupersecret" }
            };
            var test2Payload = new StringContent(test2Json.ToString(), Encoding.UTF8, "application/json");
            var test2Response = await test2Client.PostAsync("/api/session/signin", test2Payload);
            Assert.True(test2Response.StatusCode.Equals(HttpStatusCode.OK), "Wrong Status Code. Expected: OK. Received: " + test2Response.StatusCode.ToString());

            //Restore
            var json2 = new JsonObject
            {
                { "firstName", "Alpha" },
                { "lastName", null },
                { "password", "password" }
            };
            var payload2 = new StringContent(json2.ToString(), Encoding.UTF8, "application/json");
            var response2 = await client.PutAsync("api/users/-6", payload2);
            Assert.True(response2.StatusCode.Equals(HttpStatusCode.OK), "Wrong Status Code. Expected: OK. Received: " + response.ToString());
            
            //Sign-in with new password
            var verifyClient = application.CreateClient();
            var verifyJson = new JsonObject
            {
                { "email", "rep2@company1.example.com" },
                { "password", "password" }
            };
            var verifyPayload = new StringContent(verifyJson.ToString(), Encoding.UTF8, "application/json");
            var verifyResponse = await verifyClient.PostAsync("/api/session/signin", verifyPayload);
            Assert.True(verifyResponse.StatusCode.Equals(HttpStatusCode.OK), "Wrong Status Code. Expected: OK. Received: " + verifyResponse.StatusCode.ToString());

            //Verify
            var responseObject = JsonConvert.DeserializeObject<User>(await response.Content.ReadAsStringAsync());
            Assert.True(responseObject.Id == -6, "Wrong user id. Expected: -6. Received: " + responseObject.Id.ToString());
            Assert.True(responseObject.FirstName.Equals("Rakel"), "Wrong first name. Expected: Rakel. Received: " +  responseObject.FirstName);
            Assert.True(responseObject.LastName.Equals("Rep"), "Wrong last name. Expected: Rep. Received: " + responseObject.LastName);
            Assert.True(responseObject.Role.Equals(Role.CompanyRepresentative), "Wrong user role. Expected: CompanyRepresentative. Received: " + responseObject.Role.ToString());
            Assert.True(responseObject.PhoneNr == null, "Wrong phone number. Expected: null. Received: " + responseObject.PhoneNr);
            Assert.True(responseObject.FoodPreferences == null, "Wrong food preferences. Expected: null. Received: " + responseObject.FoodPreferences);

            var responseObject2 = JsonConvert.DeserializeObject<User>(await response2.Content.ReadAsStringAsync());
            Assert.True(responseObject2.Id == -6, "Wrong user id. Expected: -6. Received: " + responseObject2.Id.ToString());
            Assert.True(responseObject2.FirstName.Equals("Alpha"), "Wrong first name. Expected: Alpha. Received: " + responseObject2.FirstName);
            Assert.True(responseObject2.LastName.Equals("Rep"), "Wrong last name. Expected: Rep. Received: " + responseObject2.LastName);
            Assert.True(responseObject2.Role.Equals(Role.CompanyRepresentative), "Wrong user role. Expected: CompanyRepresentative. Received: " + responseObject2.Role.ToString());
            Assert.True(responseObject2.PhoneNr == null, "Wrong phone number. Expected: null. Received: " + responseObject2.PhoneNr);
            Assert.True(responseObject2.FoodPreferences == null, "Wrong food preferences. Expected: null. Received: " + responseObject2.FoodPreferences);
        }

        [Fact]
        public async Task AdminUpdateUserBadPassword()
        {
            var client = await TestUtils.Login("admin");
            var json = new JsonObject
            {
                { "password", "test" }
            };
            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await client.PutAsync("api/users/-2", payload);

            Assert.True(response.StatusCode.Equals(HttpStatusCode.BadRequest), "Wrong Status Code. Expected: BadRequest. Received: " + response.ToString());
        }

        [Fact]
        public async Task AdminUpdateUserNotExist()
        {
            var client = await TestUtils.Login("admin");
            var json = new JsonObject
            {
                { "firstName", "Rakel" },
                { "lastName", "Spektakel" }
            };
            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await client.PutAsync("api/users/-123", payload);

            Assert.True(response.StatusCode.Equals(HttpStatusCode.NotFound), "Wrong Status Code. Expected: NotFound. Received: " + response.ToString());
        }

        [Fact]
        public async Task UnautherizedUpdateUser()
        {
            var client =  await TestUtils.Login("company1");
            var json = new JsonObject
            {
                { "firstName", "Rakel" },
                { "lastName", "Spektakel" }
            };
            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await client.PutAsync("api/users/-2", payload);

            Assert.True(response.StatusCode.Equals(HttpStatusCode.Forbidden), "Wrong Status Code. Expected: Forbidden. Received: " + response.ToString());
        }

        [Fact]
        public async Task CompanyGetMe()
        {
            var client = await TestUtils.Login("company1");
            var response = await client.GetAsync("/api/users/me");
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), "Wrong Status Code. Expected: OK. Received: " + response.StatusCode.ToString());

            var responseUser = JsonConvert.DeserializeObject<User>(await response.Content.ReadAsStringAsync());
            Assert.True(responseUser.FirstName.Equals("Alpha"), "Wrong first name. Expected: Alpha. Received: " + responseUser.FirstName);
            Assert.True(responseUser.Email.Equals("rep1@company1.example.com"), "Wrong email. Expected: rep1@company1.example.com. Received: " + responseUser.Email);
            Assert.True(responseUser.Role.Equals(Role.CompanyRepresentative), "Wrong user role. Expected: CompanyRepresentative. Received: " + responseUser.Role.ToString());
        }

        [Fact]
        public async Task StudentGetMe()
        {
            var client =  await TestUtils.Login("student1");
            var response = await client.GetAsync("/api/users/me");
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), "Wrong Status Code. Expected: OK. Received: " + response.StatusCode.ToString());

            var responseUser = JsonConvert.DeserializeObject<User>(await response.Content.ReadAsStringAsync());
            Assert.True(responseUser.FirstName.Equals("Alpha"), "Wrong first name. Expected: Alpha. Received: " + responseUser.FirstName);
            Assert.True(responseUser.Email.Equals("student1@example.com"), "Wrong email. Expected: student1@example.com. Received: " + responseUser.Email);
            Assert.True(responseUser.Role.Equals(Role.Student), "Wrong user role. Expected: Student. Received: " + responseUser.Role.ToString());
        }

        [Fact]
        public async Task AdminGetMe()
        {
            var client =  await TestUtils.Login("admin");
            var response = await client.GetAsync("/api/users/me");
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), "Wrong Status Code. Expected: OK. Received: " + response.StatusCode.ToString());

            var responseUser = JsonConvert.DeserializeObject<User>(await response.Content.ReadAsStringAsync());
            Assert.True(responseUser.FirstName.Equals("Alpha"), "Wrong first name. Expected: Alpha. Received: " + responseUser.FirstName);
            Assert.True(responseUser.Email.Equals("admin@example.com"), "Wrong email. Expected: admin@example.com. Received: " + responseUser.Email);
            Assert.True(responseUser.Role.Equals(Role.Administrator), "Wrong user role. Expected: Administrator. Received: " + responseUser.Role.ToString());
        }

        [Fact]
        public async Task UnauthorizedGetMe()
        {
            var application = new WebApplicationFactory<Program>();
            var client = application.CreateClient();
            var response = await client.GetAsync("/api/users/me");

            Assert.True(response.StatusCode.Equals(HttpStatusCode.Unauthorized), "Wrong Status Code. Expected: Unauthorized. Received: " + response.StatusCode.ToString());
        }

        [Fact]
        public async Task UserUpdateMe()
        {
            var application = new WebApplicationFactory<Program>();
            var client = await TestUtils.Login("company4");
            var json = new JsonObject
            {
                { "firstName", "Rakel" },
                { "lastName", "Spektakel" },
                { "password", "superdupersecret" }
            };
            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await client.PutAsync("api/users/me", payload);

            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), "Wrong Status Code. Expected: OK. Received: " + response.ToString());

            //Sign-in with old password
            var testClient = application.CreateClient();
            var testJson = new JsonObject
            {
                { "email", "rep1@company4.example.com" },
                { "password", "password" }
            };
            var testPayload = new StringContent(testJson.ToString(), Encoding.UTF8, "application/json");
            var testResponse = await testClient.PostAsync("/api/session/signin", testPayload);

            Assert.True(testResponse.StatusCode.Equals(HttpStatusCode.BadRequest), "Wrong Status Code. Expected: BadRequest. Received: " + testResponse.StatusCode.ToString());

            //Sign-in with new password
            var test2Client = application.CreateClient();
            var test2Json = new JsonObject
            {
                { "email", "rep1@company4.example.com" },
                { "password", "superdupersecret" }
            };
            var test2Payload = new StringContent(test2Json.ToString(), Encoding.UTF8, "application/json");
            var test2Response = await test2Client.PostAsync("/api/session/signin", test2Payload);

            Assert.True(test2Response.StatusCode.Equals(HttpStatusCode.OK), "Wrong Status Code. Expected: OK. Received: " + test2Response.StatusCode.ToString());

            //Restore
            var json2 = new JsonObject
            {
                { "firstName", "Epsilon" },
                { "lastName", "Rep" },
                { "password", "password" }
            };
            var payload2 = new StringContent(json2.ToString(), Encoding.UTF8, "application/json");
            var response2 = await client.PutAsync("api/users/me", payload2);
            Assert.True(response2.StatusCode.Equals(HttpStatusCode.OK), "Wrong Status Code. Expected: OK. Received: " + response.ToString());

            //Sign-in with new password
            var verifyClient = application.CreateClient();
            var verifyJson = new JsonObject
            {
                { "email", "rep1@company4.example.com" },
                { "password", "password" }
            };
            var verifyPayload = new StringContent(verifyJson.ToString(), Encoding.UTF8, "application/json");
            var verifyResponse = await verifyClient.PostAsync("/api/session/signin", verifyPayload);
            Assert.True(verifyResponse.StatusCode.Equals(HttpStatusCode.OK), "Wrong Status Code. Expected: OK. Received: " + verifyResponse.StatusCode.ToString());

            //Verify
            var responseObject = JsonConvert.DeserializeObject<User>(await response.Content.ReadAsStringAsync());
            Assert.True(responseObject.Id == -9, "Wrong user id. Expected: -9. Received: " + responseObject.Id.ToString());
            Assert.True(responseObject.FirstName.Equals("Rakel"), "Wrong first name. Expected: Rakel. Received: " + responseObject.FirstName);
            Assert.True(responseObject.LastName.Equals("Spektakel"), "Wrong last name. Expected: Spektakel. Received: " + responseObject.LastName);
            Assert.True(responseObject.Role.Equals(Role.CompanyRepresentative), "Wrong user role. Expected: CompanyRepresentative. Received: " + responseObject.Role.ToString());
            Assert.True(responseObject.PhoneNr == null, "Wrong phone number. Expected: null. Received: " + responseObject.PhoneNr);
            Assert.True(responseObject.FoodPreferences == null, "Wrong food preferences. Expected: null. Received: " + responseObject.FoodPreferences);

            var responseObject2 = JsonConvert.DeserializeObject<User>((await response2.Content.ReadAsStringAsync()));
            Assert.True(responseObject2.Id == -9, "Wrong user id. Expected: -9. Received: " + responseObject2.Id.ToString());
            Assert.True(responseObject2.FirstName.Equals("Epsilon"), "Wrong first name. Expected: Epsikon. Received: " + responseObject2.FirstName);
            Assert.True(responseObject2.LastName.Equals("Rep"), "Wrong last name. Expected: Rep. Received: " + responseObject2.LastName);
            Assert.True(responseObject2.Role.Equals(Role.CompanyRepresentative), "Wrong user role. Expected: CompanyRepresentative. Received: " + responseObject2.Role.ToString());
            Assert.True(responseObject2.PhoneNr == null, "Wrong phone number. Expected: null. Received: " + responseObject2.PhoneNr);
            Assert.True(responseObject2.FoodPreferences == null, "Wrong food preferences. Expected: null. Received: " + responseObject2.FoodPreferences);
        }

        [Fact]
        public async Task StudentUpdateMeBadPassword()
        {
            var client = await TestUtils.Login("student1");
            var json = new JsonObject
            {
                { "password", "test" }
            };
            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await client.PutAsync("api/users/me", payload);

            Assert.True(response.StatusCode.Equals(HttpStatusCode.BadRequest), "Wrong Status Code. Expected: BadRequest. Received: " + response.ToString());
        }

        [Fact]
        public async Task UpdateMeUnautherized()
        {
            var application = new WebApplicationFactory<Program>();
            var client = application.CreateClient();
            var json = new JsonObject
            {
                { "password", "newSuperSecretPassword" }
            };
            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await client.PutAsync("api/users/me", payload);

            Assert.True(response.StatusCode.Equals(HttpStatusCode.Unauthorized), "Wrong Status Code. Expected: Unauthorized. Received: " + response.ToString());
        }
    }
}
