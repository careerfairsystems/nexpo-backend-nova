﻿using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Xunit;
using Newtonsoft.Json;
using Nexpo.Models;

namespace Nexpo.Tests.Controllers
{
    public class StudentsControllerTest
    {

        [Fact]
        public async Task GetSpecificStudentAsAdmin()
        {
            var client = await TestUtils.Login("admin");

            var response = await client.GetAsync("/api/students/-3");
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), "Wrong status code. Expected: OK. Recieved: " + response.StatusCode.ToString());

            var responseObject = JsonConvert.DeserializeObject<Student>((await response.Content.ReadAsStringAsync()));
            Assert.True(responseObject.Id == -3, "Wrong id. Expected: -3. Recieved: " + responseObject.Id.ToString());
            Assert.True(responseObject.Programme == Programme.Väg_och_vatttenbyggnad, "Wrong programme. Recieved: " + responseObject.Programme.ToString());
            Assert.True(responseObject.Year == 3, "Wrong year. Expected: 3. Recieved: " + responseObject.Year.ToString());
            Assert.True(responseObject.UserId == -4, "Wrong userID. Expected: -4. Recieved: " + responseObject.UserId.ToString());
        }

        [Fact]
        public async Task GetSpecificStudentAsCompanyRep()
        {
            var client = await TestUtils.Login("company1");

            var response = await client.GetAsync("/api/students/-3");
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), "Wrong status code. Expected: OK. Recieved: " + response.StatusCode.ToString());

            var responseObject = JsonConvert.DeserializeObject<Student>((await response.Content.ReadAsStringAsync()));
            Assert.True(responseObject.Id == -3, "Wrong id. Expected: -3. Recieved: " + responseObject.Id.ToString());
            Assert.True(responseObject.Programme == Programme.Väg_och_vatttenbyggnad, "Wrong programme. Recieved: " + responseObject.Programme.ToString());
            Assert.True(responseObject.Year == 3, "Wrong year. Expected: 3. Recieved: " + responseObject.Year.ToString());
            Assert.True(responseObject.UserId == -4, "Wrong userID. Expected: -4. Recieved: " + responseObject.UserId.ToString());
        }

        [Fact]
        public async Task GetSpecificStudentInvalidId()
        {
            var client = await TestUtils.Login("admin");

            var response = await client.GetAsync("/api/students/-123");
            Assert.True(response.StatusCode.Equals(HttpStatusCode.NotFound), "Wrong status code. Expected: NotFound. Recieved: " + response.StatusCode.ToString());
        }


        [Fact]
        public async Task GetSpecificStudentAsStudent()
        {
            var client = await TestUtils.Login("student1");

            var response = await client.GetAsync("/api/students/-1");
            Assert.True(response.StatusCode.Equals(HttpStatusCode.Forbidden), "Wrong status code. Expected: Forbidden. Recieved: " + response.StatusCode.ToString());
        }

        [Fact]
        public async Task GetSpecificStudentNotLoggedIn()
        {
            var application = new WebApplicationFactory<Nexpo.Program>();
            var client = application.CreateClient();

            var response = await client.GetAsync("/api/students/-1");
            Assert.True(response.StatusCode.Equals(HttpStatusCode.Unauthorized), "Wrong status code. Expected: Unauthorized. Recieved: " + response.StatusCode.ToString());
        }

        [Fact]
        public async Task GetCurrentSignedInStudent()
        {
            var client = await TestUtils.Login("student3");

            var response = await client.GetAsync("api/students/me");
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), "Wrong status code. Expected: OK. Recieved: " + response.StatusCode.ToString());

            var responseObject = JsonConvert.DeserializeObject<Student>((await response.Content.ReadAsStringAsync()));
            Assert.True(responseObject.Id == -3, "Wrong id. Expected: -3. Recieved: " + responseObject.Id.ToString());
            Assert.True(responseObject.Programme == Programme.Väg_och_vatttenbyggnad, "Wrong programme. Recieved: " + responseObject.Programme.ToString());
            Assert.True(responseObject.Year == 3, "Wrong year. Expected: 3. Recieved: " + responseObject.Year.ToString());
            Assert.True(responseObject.UserId == -4, "Wrong userID. Expected: -4. Recieved: " + responseObject.UserId.ToString());
        }

        [Fact]
        public async Task GetCurrentSignedInStudentAsAdmin()
        {
            var client = await TestUtils.Login("admin");

            var response = await client.GetAsync("api/students/me");
            Assert.True(response.StatusCode.Equals(HttpStatusCode.Forbidden), "Wrong status code. Expected: Forbidden. Recieved: " + response.StatusCode.ToString());
        }

        [Fact]
        public async Task GetCurrentSignedInStudentAsNotSignedIn()
        {
            var application = new WebApplicationFactory<Nexpo.Program>();
            var client = application.CreateClient();

            var response = await client.GetAsync("api/students/me");
            Assert.True(response.StatusCode.Equals(HttpStatusCode.Unauthorized), "Wrong status code. Expected: Unauthorized. Recieved: " + response.StatusCode.ToString());
        }

        [Fact]
        public async Task UpdateStudentInfoAsAdminPartialCorrect()
        {
            //Setup
            var client = await TestUtils.Login("admin");

            var json = new JsonObject();
            json.Add("programme", 19);
            json.Add("linkedIn", "linkedin.com");
            json.Add("masterTitle", "Math");
            json.Add("year", 10);

            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await client.PutAsync("api/students/-1", payload);

            //Restore
            var json2 = new JsonObject();
            json2.Add("Programme", 18);
            json2.Add("linkedin", "");
            json2.Add("mastertitle", "Project management in software systems");
            json2.Add("year", 4);

            var payload2 = new StringContent(json2.ToString(), Encoding.UTF8, "application/json");
            var response2 = await client.PutAsync("api/students/-1", payload2);

            var responseObject = JsonConvert.DeserializeObject<Student>((await response.Content.ReadAsStringAsync()));
            var responseObject2 = JsonConvert.DeserializeObject<Student>((await response2.Content.ReadAsStringAsync()));

            //Assertions of response
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), "Wrong status code. Expected: OK. Recieved: " + response.StatusCode.ToString());
            Assert.True(responseObject.Id == -1, responseObject.Id.ToString());
            Assert.True(responseObject.Programme == Programme.Teknisk_Fysik, responseObject.Programme.ToString());
            Assert.True(responseObject.Year == 4, responseObject.Year.ToString());
            Assert.True(responseObject.UserId == -2, responseObject.UserId.ToString());
            Assert.True(responseObject.MasterTitle == "Math", responseObject.MasterTitle.ToString());

            //Verify Restore
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), "Wrong status code. Expected: OK. Recieved: " + response2.StatusCode.ToString());
            Assert.True(responseObject2.Id == -1, responseObject2.Id.ToString());
            Assert.True(responseObject2.Programme == Programme.Datateknik, responseObject2.Programme.ToString());
            Assert.True(responseObject2.Year == 4, responseObject2.Year.ToString());
            Assert.True(responseObject2.LinkedIn == "", responseObject2.LinkedIn.ToString());
            Assert.True(responseObject2.MasterTitle == "Project management in software systems", responseObject2.MasterTitle.ToString());
        }

        [Fact]
        public async Task UpdateStudentInfoAsAdminCorrect()
        {
            //Setup
            var application = new WebApplicationFactory<Nexpo.Program>();
            var client = application.CreateClient();
            var token = await Login("admin", client);

            var json = new JsonObject();
            json.Add("linkedIn", "https://www.linkedin.com/in/test");
            json.Add("year", 1);

            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await client.PutAsync("api/students/-1", payload);
            var responseObject = JsonConvert.DeserializeObject<Student>((await response.Content.ReadAsStringAsync()));

            //Assertions of response
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), response.ToString());
            Assert.True(responseObject.Id == -1, responseObject.Id.ToString());
            Assert.True(responseObject.Programme == Programme.Datateknik, responseObject.Programme.ToString());
            Assert.True(responseObject.Year == 1, responseObject.Year.ToString());
            Assert.True(responseObject.UserId == -2, responseObject.UserId.ToString());
            Assert.True(responseObject.LinkedIn == "https://www.linkedin.com/in/test", responseObject.LinkedIn.ToString());
            Assert.True(responseObject.MasterTitle == "Project management in software systems", responseObject.MasterTitle.ToString());

            //Restore
            var json2 = new JsonObject();
            json2.Add("linkedin", "");
            json2.Add("year", 4);

            var payload2 = new StringContent(json2.ToString(), Encoding.UTF8, "application/json");
            var response2 = await client.PutAsync("api/students/-1", payload2);
            var responseObject2 = JsonConvert.DeserializeObject<Student>((await response2.Content.ReadAsStringAsync()));

            //Verify Restore
            Assert.True(response2.StatusCode.Equals(HttpStatusCode.OK), response2.StatusCode.ToString());
            Assert.True(responseObject2.Id == -1, responseObject2.Id.ToString());
            Assert.True(responseObject2.Programme == Programme.Datateknik, responseObject2.Programme.ToString());
            Assert.True(responseObject2.Year == 4, responseObject2.Year.ToString());
            Assert.True(responseObject2.LinkedIn == "", responseObject2.LinkedIn.ToString());
            Assert.True(responseObject2.MasterTitle == "Project management in software systems", responseObject2.MasterTitle.ToString());
        }

        [Fact]
        public async Task UpdateStudentInfoAsAdminWrongId()
        {
            var application = new WebApplicationFactory<Nexpo.Program>();
            var client = application.CreateClient();
            var token = await Login("admin", client);

            var json = new JsonObject();
            json.Add("linkedIn", "linkedin.com");
            json.Add("year", 10);

            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await client.PutAsync("api/students/-112", payload);

            Assert.True(response.StatusCode.Equals(HttpStatusCode.NotFound), response.ToString());
        }

        [Fact]
        public async Task UpdateStudentInfoAsCompanyRepUsingId()
        {
            var application = new WebApplicationFactory<Nexpo.Program>();
            var client = application.CreateClient();
            var token = await Login("company", client);

            var json = new JsonObject();
            json.Add("linkedIn", "linkedin.com");
            json.Add("year", 10);

            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await client.PutAsync("api/students/-1", payload);

            Assert.True(response.StatusCode.Equals(HttpStatusCode.Forbidden), response.ToString());
        }

        [Fact]
        public async Task UpdateStudentInfoAsCompanyRepUsingMe()
        {
            var application = new WebApplicationFactory<Nexpo.Program>();
            var client = application.CreateClient();
            var token = await Login("company", client);

            var json = new JsonObject();
            json.Add("linkedIn", "linkedin.com");
            json.Add("year", 10);

            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await client.PutAsync("api/students/me", payload);

            Assert.True(response.StatusCode.Equals(HttpStatusCode.Forbidden), response.ToString());
        }

        [Fact]
        public async Task UpdateStudentInfoAsStudentCorrect()
        {
            //Setup
            var application = new WebApplicationFactory<Nexpo.Program>();
            var client = application.CreateClient();
            var token = await Login("", client);

            var json = new JsonObject();
            json.Add("programme", 20);
            json.Add("linkedIn", "linkedin.com");
            json.Add("masterTitle", "Math");
            json.Add("year", 1);

            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await client.PutAsync("api/students/me", payload);
            var responseObject = JsonConvert.DeserializeObject<Student>((await response.Content.ReadAsStringAsync()));

            //Assertions of response
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), response.ToString());
            Assert.True(responseObject.Id == -1, responseObject.Id.ToString());
            Assert.True(responseObject.Programme == Programme.Byggteknik_med_väg_och_trafikteknik, responseObject.Programme.ToString());
            Assert.True(responseObject.Year == 1, responseObject.Year.ToString());
            Assert.True(responseObject.UserId == -2, responseObject.UserId.ToString());
            Assert.True(responseObject.MasterTitle == "Math", responseObject.MasterTitle.ToString());

            //Restore
            var json2 = new JsonObject();
            json2.Add("Programme", 18);
            json2.Add("linkedin", "");
            json2.Add("mastertitle", "Project management in software systems");
            json2.Add("year", 4);

            var payload2 = new StringContent(json2.ToString(), Encoding.UTF8, "application/json");
            var response2 = await client.PutAsync("api/students/me", payload2);
            var responseObject2 = JsonConvert.DeserializeObject<Student>((await response2.Content.ReadAsStringAsync()));

            //Verify Restore
            Assert.True(response2.StatusCode.Equals(HttpStatusCode.OK), response2.StatusCode.ToString());
            Assert.True(responseObject2.Id == -1, responseObject2.Id.ToString());
            Assert.True(responseObject2.Programme == Programme.Datateknik, responseObject2.Programme.ToString());
            Assert.True(responseObject2.Year == 4, responseObject2.Year.ToString());
            Assert.True(responseObject2.LinkedIn == "", responseObject2.LinkedIn.ToString());
            Assert.True(responseObject2.MasterTitle == "Project management in software systems", responseObject2.MasterTitle.ToString());
        }

        [Fact]
        public async Task UpdateStudentInfoAsStudentCorrectPartial()
        {
            //Setup
            var application = new WebApplicationFactory<Nexpo.Program>();
            var client = application.CreateClient();
            var token = await Login("", client);

            var json = new JsonObject();
            json.Add("linkedIn", "https://www.linkedin.com/in/test");
            json.Add("year", 5);

            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await client.PutAsync("api/students/me", payload);
            var responseObject = JsonConvert.DeserializeObject<Student>((await response.Content.ReadAsStringAsync()));

            //Assertions of response
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), response.ToString());
            Assert.True(responseObject.Id == -1, responseObject.Id.ToString());
            Assert.True(responseObject.Programme == Programme.Datateknik, responseObject.Programme.ToString());
            Assert.True(responseObject.Year == 5, responseObject.Year.ToString());
            Assert.True(responseObject.UserId == -2, responseObject.UserId.ToString());
            Assert.True(responseObject.LinkedIn == "https://www.linkedin.com/in/test", responseObject.LinkedIn.ToString());
            Assert.True(responseObject.MasterTitle == "Project management in software systems", responseObject.MasterTitle.ToString());

            //Restore
            var json2 = new JsonObject();
            json2.Add("linkedin", "");
            json2.Add("year", 4);

            var payload2 = new StringContent(json2.ToString(), Encoding.UTF8, "application/json");
            var response2 = await client.PutAsync("api/students/me", payload2);
            var responseObject2 = JsonConvert.DeserializeObject<Student>((await response2.Content.ReadAsStringAsync()));

            //Verify Restore
            Assert.True(response2.StatusCode.Equals(HttpStatusCode.OK), response2.StatusCode.ToString());
            Assert.True(responseObject2.Id == -1, responseObject2.Id.ToString());
            Assert.True(responseObject2.Programme == Programme.Datateknik, responseObject2.Programme.ToString());
            Assert.True(responseObject2.Year == 4, responseObject2.Year.ToString());
            Assert.True(responseObject2.LinkedIn == "", responseObject2.LinkedIn.ToString());
            Assert.True(responseObject2.MasterTitle == "Project management in software systems", responseObject2.MasterTitle.ToString());
        }

        [Fact]
        public async Task UpdateStudentInfoAsStudentWithId()
        {
            var application = new WebApplicationFactory<Nexpo.Program>();
            var client = application.CreateClient();
            var token = await Login("", client);

            var json = new JsonObject();
            json.Add("programme", 18);
            json.Add("linkedIn", "linkedin.com");
            json.Add("masterTitle", "Math");
            json.Add("year", 10);

            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await client.PutAsync("api/students/-1", payload);
            var responseObject = JsonConvert.DeserializeObject<Student>((await response.Content.ReadAsStringAsync()));

            Assert.True(response.StatusCode.Equals(HttpStatusCode.Forbidden), response.ToString());
        }

    }
}
