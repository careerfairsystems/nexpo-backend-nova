using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Nexpo.Models;
using Nexpo.DTO;
using System.Collections.Generic;
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
        [Fact]
        public async Task GetAllAsAdmin()
        {
            //Setup
            //Login as admin, in order to be able to update the role of a user
            //Otherwise we'll receive "Forbidden" as response from the client.PutAync
            var client = await TestUtils.Login("admin");

            //Create a data transfer object (DTO) with the new role
            //DTO's are classes that are used to transfer data 
            //IMO this is easier than storing the enum Role in a json file
            var updateRoleDto = new UpdateUserDTO
            {
                Role = Role.Volunteer
            };

            //Serialize the data transfer object to json
            //Meaning take what we want to transfer inside the DTO and convert it to a string
            //and then to a StringContent so the client can send it
            var json = JsonConvert.SerializeObject(updateRoleDto);
            var payload = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

            //Send a PUT request to update the user with id -5 with the payload. 
            //Note that in the ApplicationDBContext the CompanyRepresentative has id -5
            //api/users/-5 is the endpoint for updating a user with id -5
            // -> (see UsersController.cs api/users/{id})
            //So we are updating the role of the CompanyRepresentative to the role contained in the DTO

            var response = await client.PutAsync("api/users/-5", payload);

            //Assertions of response, meaning that check that the "put" request was successful
            Assert.True(
                response.StatusCode.Equals(HttpStatusCode.OK),
                "Wrong status code. Expected: OK. Received: " + response.StatusCode.ToString()
            );

            //Extract the content of the response and deserialize it to a User object
            var serializedUser = await response.Content.ReadAsStringAsync();
            var user = JsonConvert.DeserializeObject<User>(serializedUser);
            
            //Check that the role of the user is now Volunteer
            Assert.True(
                user.Role.Equals(Role.Volunteer), 
                "Wrong role. Expected: CompanyRepresentative. Received: " + user.Role.ToString()
            );

            //The process of restoring back to an CompanyRepresentative
            //Note that if this is not done, the comming tests will fail
            //(an alternative is seeding the database with a new user that you can corrupt)
            var updateRoleDto2 = new UpdateUserDTO
            {
                Role = Role.CompanyRepresentative
            };

            //Same process as above
            var json2 = JsonConvert.SerializeObject(updateRoleDto2);
            var payload2 = new StringContent(json2, UnicodeEncoding.UTF8, "application/json");

            var response2 = await client.PutAsync("api/users/-5", payload2);
            Assert.True(
                response2.StatusCode.Equals(HttpStatusCode.OK), 
                "Wrong status code. Expected: OK. Received: " + response2.StatusCode.ToString()
            );

            var user2 = JsonConvert.DeserializeObject<User>(await response2.Content.ReadAsStringAsync());
            Assert.True(
                user2.Role.Equals(Role.CompanyRepresentative), 
                "Wrong role. Expected: CompanyRepresentative. Received: " + user2.Role.ToString()
            );

        }

        [Fact]
        public async Task AdminChangeNonExistingUserRole(){
            var client = await TestUtils.Login("admin");
            var updateRoleDto = new UpdateUserDTO
            {
                Role = Role.Volunteer
            };

            var json = JsonConvert.SerializeObject(updateRoleDto);
            var payload = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

            var response = await client.PutAsync("api/users/-100", payload);

            Assert.True(
                response.StatusCode.Equals(HttpStatusCode.NotFound),
                "Wrong status code. Expected: NotFound. Received: " + response.StatusCode.ToString()
            );
        }

        [Fact]
        public async Task NonAdminChangeRole(){
            var client = await TestUtils.Login("student1");
            var updateRoleDto = new UpdateUserDTO
            {
                Role = Role.Volunteer
            };

            var json = JsonConvert.SerializeObject(updateRoleDto);
            var payload = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

            var response = await client.PutAsync("api/users/-5", payload);

            Assert.True(
                response.StatusCode.Equals(HttpStatusCode.Forbidden),
                "Wrong status code. Expected: Forbidden. Received: " + response.StatusCode.ToString()
            );
        }
        
        [Fact]
        public async Task AdminGetAllUsers()
        {
            var client = await TestUtils.Login("admin");
            var response = await client.GetAsync("/api/users/");

            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), "Wrong Status Code. Expected: OK. Received: " + response.StatusCode.ToString());

            var responseList = JsonConvert.DeserializeObject<List<User>>(await response.Content.ReadAsStringAsync());
            var userAdmin = responseList.Find(user => user.Id == -1);
            var userStudent = responseList.Find(user => user.Id == -2);
            var userRep = responseList.Find(user => user.Id == -5);
            
            Assert.True(responseList.Count == 10, "Wrong number of users. Expected: 10. Received: " + responseList.Count.ToString());
            Assert.True(userAdmin.Role.Equals(Role.Administrator), "Wrong user role. Expected: admin. Received: " + userAdmin.Role.ToString());
            Assert.True(userStudent.FirstName.Equals("Alpha"), "Wrong user first name. Expected: Alpha. Received: " + userStudent.FirstName);
            Assert.True(userRep.CompanyId == -1, "Wrong company id. Expected: -1. Received: " + userRep.CompanyId.ToString());
        }

        [Fact]
        public async Task GetAllUsersAStudent()
        {
            var client = await TestUtils.Login("student1");
            var response = await client.GetAsync("/api/users/");

            // Verify response - Students should not be able to get all users
            Assert.True(response.StatusCode.Equals(HttpStatusCode.Forbidden), "Wrong Status Code. Expected: Forbidden. Received: " + response.StatusCode.ToString());
        }

        [Fact]
        public async Task GetAsAdmin()
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
        public async Task GetUserWithStudentApplicationAsCompany()
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
        public async Task GetUserNoStudentApplicationAsCompany()
        {
            var client = await TestUtils.Login("company3");
            var response = await client.GetAsync("/api/users/-2");

            // Verify response - Companies should not be able to get users who are have not applied for a student session
            Assert.True(response.StatusCode.Equals(HttpStatusCode.Forbidden), "Wrong Status Code. Expected: Forbidden. Received: " + response.StatusCode.ToString());
        }

        [Fact]
        public async Task GetUserNonStudentAsCompany()
        {
            var client = await TestUtils.Login("company1");
            var response = await client.GetAsync("/api/users/-6");

            // Verify response - Companies should not be able to get users who are not students
            Assert.True(response.StatusCode.Equals(HttpStatusCode.Forbidden), "Wrong Status Code. Expected: Forbidden. Received: " + response.StatusCode.ToString());
        }

        [Fact]
        public async Task GetAsStudent()
        {
            var client = await TestUtils.Login("student1");
            var response = await client.GetAsync("/api/users/-2");

            // Verify response - Students should not be able to get users
            Assert.True(response.StatusCode.Equals(HttpStatusCode.Forbidden), "Wrong Status Code. Expected: Forbidden. Received: " + response.StatusCode.ToString());
        }

        [Fact]
        public async Task UpdateAsAdmin()
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
            Assert.True(responseObject.FirstName.Equals("Rakel"), "Wrong first name. Expected: Rakel. Received: " + responseObject.FirstName);
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
        public async Task UpdateWithBadPasswordAsAdmin()
        {
            var client = await TestUtils.Login("admin");
            var json = new JsonObject
            {
                { "password", "test" }
            };

            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await client.PutAsync("api/users/-2", payload);

            // Verify response - should be BadRequest because password is too short
            Assert.True(response.StatusCode.Equals(HttpStatusCode.BadRequest), "Wrong Status Code. Expected: BadRequest. Received: " + response.ToString());
        }

        [Fact]
        public async Task UpdateNonExistingAsAdmin()
        {
            var client = await TestUtils.Login("admin");
            var json = new JsonObject
            {
                { "firstName", "Rakel" },
                { "lastName", "Spektakel" }
            };

            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await client.PutAsync("api/users/-123", payload);

            // Verify response - should be NotFound because user of id -123 does not exist in the database
            Assert.True(response.StatusCode.Equals(HttpStatusCode.NotFound), "Wrong Status Code. Expected: NotFound. Received: " + response.ToString());
        }

        [Fact]
        public async Task UpdateAsCompany()
        {
            var client = await TestUtils.Login("company1");
            var json = new JsonObject
            {
                { "firstName", "Rakel" },
                { "lastName", "Spektakel" }
            };

            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await client.PutAsync("api/users/-2", payload);

            // Verify response - should be Forbidden because company1 is not allowed to update user of id -2
            Assert.True(response.StatusCode.Equals(HttpStatusCode.Forbidden), "Wrong Status Code. Expected: Forbidden. Received: " + response.ToString());
        }

        [Fact]
        public async Task GetMeAsCompany()
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
        public async Task GetMeAsStudent()
        {
            var client = await TestUtils.Login("student1");
            var response = await client.GetAsync("/api/users/me");
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), "Wrong Status Code. Expected: OK. Received: " + response.StatusCode.ToString());

            var responseUser = JsonConvert.DeserializeObject<User>(await response.Content.ReadAsStringAsync());
            Assert.True(responseUser.FirstName.Equals("Alpha"), "Wrong first name. Expected: Alpha. Received: " + responseUser.FirstName);
            Assert.True(responseUser.Email.Equals("student1@example.com"), "Wrong email. Expected: student1@example.com. Received: " + responseUser.Email);
            Assert.True(responseUser.Role.Equals(Role.Student), "Wrong user role. Expected: Student. Received: " + responseUser.Role.ToString());
        }

        [Fact]
        public async Task GetMeAsAdmin()
        {
            var client = await TestUtils.Login("admin");
            var response = await client.GetAsync("/api/users/me");
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), "Wrong Status Code. Expected: OK. Received: " + response.StatusCode.ToString());

            var responseUser = JsonConvert.DeserializeObject<User>(await response.Content.ReadAsStringAsync());
            Assert.True(responseUser.FirstName.Equals("Alpha"), "Wrong first name. Expected: Alpha. Received: " + responseUser.FirstName);
            Assert.True(responseUser.Email.Equals("admin@example.com"), "Wrong email. Expected: admin@example.com. Received: " + responseUser.Email);
            Assert.True(responseUser.Role.Equals(Role.Administrator), "Wrong user role. Expected: Administrator. Received: " + responseUser.Role.ToString());
        }

        [Fact]
        public async Task GetMeNotLoggedIn()
        {
            var application = new WebApplicationFactory<Program>();
            var client = application.CreateClient();
            var response = await client.GetAsync("/api/users/me");

            // Verify response - should be Unauthorized because user is not logged in
            Assert.True(response.StatusCode.Equals(HttpStatusCode.Unauthorized), "Wrong Status Code. Expected: Unauthorized. Received: " + response.StatusCode.ToString());
        }

        [Fact]
        public async Task UpdateMeBadPasswordAsStudent()
        {
            var client = await TestUtils.Login("student1");
            var json = new JsonObject
            {
                { "password", "test" }
            };
            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await client.PutAsync("api/users/me", payload);

            // Verify response - should be BadRequest because password is too short
            Assert.True(response.StatusCode.Equals(HttpStatusCode.BadRequest), "Wrong Status Code. Expected: BadRequest. Received: " + response.ToString());
        }

        [Fact]
        public async Task UpdateMeNotLoggedIn()
        {
            var application = new WebApplicationFactory<Program>();
            var client = application.CreateClient();

            var json = new JsonObject
            {
                { "password", "newSuperSecretPassword" }
            };

            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await client.PutAsync("api/users/me", payload);

            // Verify response - should be Unauthorized because user is not logged in
            Assert.True(response.StatusCode.Equals(HttpStatusCode.Unauthorized), "Wrong Status Code. Expected: Unauthorized. Received: " + response.ToString());
        }

        [Fact]
        public async Task UpdateMe()
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
