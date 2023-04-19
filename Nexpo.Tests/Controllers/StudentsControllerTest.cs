using Microsoft.AspNetCore.Mvc.Testing;
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
        public async Task GetStudentAsAdmin()
        {
            var client = await TestUtils.Login("admin");
            var response = await client.GetAsync("/api/students/-3");
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), "Wrong status code. Expected: OK. Received: " + response.StatusCode.ToString());

            var responseObject = JsonConvert.DeserializeObject<Student>(await response.Content.ReadAsStringAsync());
            Assert.True(responseObject.Id == -3, "Wrong id. Expected: -3. Received: " + responseObject.Id.ToString());
            Assert.True(responseObject.Programme == Programme.Väg_och_vatttenbyggnad, "Wrong programme. Received: " + responseObject.Programme.ToString());
            Assert.True(responseObject.Year == 3, "Wrong year. Expected: 3. Received: " + responseObject.Year.ToString());
            Assert.True(responseObject.UserId == -4, "Wrong userID. Expected: -4. Received: " + responseObject.UserId.ToString());
        }

        [Fact]
        public async Task GetStudentAsCompanyRep()
        {
            var client = await TestUtils.Login("company1");
            var response = await client.GetAsync("/api/students/-3");
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), "Wrong status code. Expected: OK. Received: " + response.StatusCode.ToString());

            var responseObject = JsonConvert.DeserializeObject<Student>(await response.Content.ReadAsStringAsync());
            Assert.True(responseObject.Id == -3, "Wrong id. Expected: -3. Received: " + responseObject.Id.ToString());
            Assert.True(responseObject.Programme == Programme.Väg_och_vatttenbyggnad, "Wrong programme. Received: " + responseObject.Programme.ToString());
            Assert.True(responseObject.Year == 3, "Wrong year. Expected: 3. Received: " + responseObject.Year.ToString());
            Assert.True(responseObject.UserId == -4, "Wrong userID. Expected: -4. Received: " + responseObject.UserId.ToString());
        }

        [Fact]
        public async Task GetStudentInvalidId()
        {
            var client = await TestUtils.Login("admin");
            var response = await client.GetAsync("/api/students/-123");

            Assert.True(response.StatusCode.Equals(HttpStatusCode.NotFound), "Wrong status code. Expected: NotFound. Received: " + response.StatusCode.ToString());
        }


        [Fact]
        public async Task GetStudentAsStudent()
        {
            var client = await TestUtils.Login("student1");
            var response = await client.GetAsync("/api/students/-1");

            Assert.True(response.StatusCode.Equals(HttpStatusCode.Forbidden), "Wrong status code. Expected: Forbidden. Received: " + response.StatusCode.ToString());
        }

        [Fact]
        public async Task GetStudentNotLoggedIn()
        {
            var application = new WebApplicationFactory<Program>();
            var client = application.CreateClient();
            var response = await client.GetAsync("/api/students/-1");

            Assert.True(response.StatusCode.Equals(HttpStatusCode.Unauthorized), "Wrong status code. Expected: Unauthorized. Received: " + response.StatusCode.ToString());
        }

        [Fact]
        public async Task GetMe()
        {
            var client = await TestUtils.Login("student3");
            var response = await client.GetAsync("api/students/me");
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), "Wrong status code. Expected: OK. Received: " + response.StatusCode.ToString());

            var responseObject = JsonConvert.DeserializeObject<Student>(await response.Content.ReadAsStringAsync());
            Assert.True(responseObject.Id == -3, "Wrong id. Expected: -3. Received: " + responseObject.Id.ToString());
            Assert.True(responseObject.Programme == Programme.Väg_och_vatttenbyggnad, "Wrong programme. Received: " + responseObject.Programme.ToString());
            Assert.True(responseObject.Year == 3, "Wrong year. Expected: 3. Received: " + responseObject.Year.ToString());
            Assert.True(responseObject.UserId == -4, "Wrong userID. Expected: -4. Received: " + responseObject.UserId.ToString());
        }

        [Fact]
        public async Task GetMeAsAdmin()
        {
            var client = await TestUtils.Login("admin");
            var response = await client.GetAsync("api/students/me");

            Assert.True(response.StatusCode.Equals(HttpStatusCode.Forbidden), "Wrong status code. Expected: Forbidden. Received: " + response.StatusCode.ToString());
        }

        [Fact]
        public async Task GetMeAsNotSignedIn()
        {
            var application = new WebApplicationFactory<Program>();
            var client = application.CreateClient();
            var response = await client.GetAsync("api/students/me");

            Assert.True(response.StatusCode.Equals(HttpStatusCode.Unauthorized), "Wrong status code. Expected: Unauthorized. Received: " + response.StatusCode.ToString());
        }

                [Fact]
        public async Task UpdateMeSuccess()
        {
            //Setup
            var client = await TestUtils.Login("student1");
            var json = new JsonObject
            {
                { "programme", 20 },
                { "linkedIn", "linkedin.com" },
                { "masterTitle", "Math" },
                { "year", 1 }
            };

            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await client.PutAsync("api/students/me", payload);

            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), "Wrong status code. Expected: OK. Received: " + response.StatusCode.ToString());

            //Restore
            var json2 = new JsonObject
            {
                { "Programme", 18 },
                { "linkedin", "" },
                { "mastertitle", "Project management in software systems" },
                { "year", 4 }
            };

            var payload2 = new StringContent(json2.ToString(), Encoding.UTF8, "application/json");
            var response2 = await client.PutAsync("api/students/me", payload2);

            Assert.True(response2.StatusCode.Equals(HttpStatusCode.OK), "Wrong status code. Expected: OK. Received: " + response2.StatusCode.ToString());

            //Assertions of response
            var responseObject = JsonConvert.DeserializeObject<Student>(await response.Content.ReadAsStringAsync());
            Assert.True(responseObject.Id == -1, "Wrong Student Id. Expected: -1. Received: " + responseObject.Id.ToString());
            Assert.True(responseObject.Programme == Programme.Byggteknik_med_väg_och_trafikteknik, "Wrong programme. Expected: 20. Received: " + responseObject.Programme.ToString());
            Assert.True(responseObject.Year == 1, "Wrong year. Expected: 1. Received: " + responseObject.Year.ToString());
            Assert.True(responseObject.UserId == -2, "Wrong User Id. Expected: -2. Received: " + responseObject.UserId.ToString());
            Assert.True(responseObject.MasterTitle.Equals("Math"), "Wrong master title. Expected: Math. Received: " + responseObject.MasterTitle.ToString());

            //Verify Restore
            var responseObject2 = JsonConvert.DeserializeObject<Student>(await response2.Content.ReadAsStringAsync());
            Assert.True(responseObject2.Id == -1, "Wrong Student Id. Expected: -1. Received: " + responseObject2.Id.ToString());
            Assert.True(responseObject2.Programme == Programme.Datateknik, "Wrong programme. Expected: 18. Received: " + responseObject2.Programme.ToString());
            Assert.True(responseObject2.Year == 4, "Wrong year. Expected: 4. Received: " + responseObject2.Year.ToString());
            Assert.True(responseObject2.LinkedIn.Equals(""), "Wrong likedin. Expected: \"\". Received:" + responseObject2.LinkedIn.ToString());
            Assert.True(responseObject2.MasterTitle.Equals("Project management in software systems"), "Wrong master title. Expected: Project management in software systems. Received: " + responseObject2.MasterTitle.ToString());
        }

        [Fact]
        public async Task UpdatePartialMeSuccess()
        {
            //Setup
            var client =  await TestUtils.Login("student2");
            var json = new JsonObject
            {
                { "linkedIn", "https://www.linkedin.com/in/test" },
                { "year", 5 }
            };

            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await client.PutAsync("api/students/me", payload);

            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), response.ToString());

            //Restore
            var json2 = new JsonObject
            {
                { "linkedin", "" },
                { "year", 2 }
            };

            var payload2 = new StringContent(json2.ToString(), Encoding.UTF8, "application/json");
            var response2 = await client.PutAsync("api/students/me", payload2);

            Assert.True(response2.StatusCode.Equals(HttpStatusCode.OK), response2.StatusCode.ToString());

            //Assertions of response
            var responseObject = JsonConvert.DeserializeObject<Student>(await response.Content.ReadAsStringAsync());
            Assert.True(responseObject.Id == -2, "Wrong Student Id. Expected: -2. Received: " + responseObject.Id.ToString());
            Assert.True(responseObject.Programme == Programme.Industriell_ekonomi, "Wrong programme. Expected: 14. Received: " + responseObject.Programme.ToString());
            Assert.True(responseObject.Year == 5, "Wrong year. Expected: 5. Received: " + responseObject.Year.ToString());
            Assert.True(responseObject.UserId == -3, "Wrong User Id. Expected: -3. Received: " + responseObject.UserId.ToString());
            Assert.True(responseObject.LinkedIn.Equals("https://www.linkedin.com/in/test"), "Wrong likedin. Expected: https://www.linkedin.com/in/test. Received:" + responseObject.LinkedIn.ToString());

            //Verify Restore
            var responseObject2 = JsonConvert.DeserializeObject<Student>(await response2.Content.ReadAsStringAsync());
            Assert.True(responseObject2.Id == -2, "Wrong Student Id. Expected: -2. Received: " + responseObject2.Id.ToString());
            Assert.True(responseObject2.Programme == Programme.Industriell_ekonomi, "Wrong programme. Expected: 14. Received: " + responseObject2.Programme.ToString());
            Assert.True(responseObject2.Year == 2, "Wrong year. Expected: 2. Received: " + responseObject2.Year.ToString());
            Assert.True(responseObject2.LinkedIn.Equals(""), "Wrong likedin. Expected: \"\". Received:" + responseObject2.LinkedIn.ToString());
        }

        [Fact]
        public async Task UpdateMeUsingIdForbidden()
        {
            var client = await TestUtils.Login("student1");
            var json = new JsonObject
            {
                { "programme", 18 },
                { "linkedIn", "linkedin.com" },
                { "masterTitle", "Math" },
                { "year", 10 }
            };

            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await client.PutAsync("api/students/-1", payload);

            Assert.True(response.StatusCode.Equals(HttpStatusCode.Forbidden), "Wrong status code. Expected: Forbidden. Received: " + response.StatusCode.ToString());
        }

                [Fact]
        public async Task UpdateAsAdminSuccess()
        {
            //Setup
            var client = await TestUtils.Login("admin");
            var json = new JsonObject
            {
                { "linkedIn", "https://www.linkedin.com/in/test" },
                { "year", 1 }
            };

            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await client.PutAsync("api/students/-2", payload);

            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), "Wrong status code. Expected: OK. Received: " + response.StatusCode.ToString());

            //Restore
            var json2 = new JsonObject
            {
                { "linkedin", "" },
                { "year", 2 }
            };

            var payload2 = new StringContent(json2.ToString(), Encoding.UTF8, "application/json");
            var response2 = await client.PutAsync("api/students/-2", payload2);

            Assert.True(response2.StatusCode.Equals(HttpStatusCode.OK), "Wrong status code. Expected: OK. Received: " + response2.StatusCode.ToString());

            //Assertions of response
            var responseObject = JsonConvert.DeserializeObject<Student>(await response.Content.ReadAsStringAsync());
            Assert.True(responseObject.Id == -2, "Wrong Student Id. Expected: -2. Received: " + responseObject.Id.ToString());
            Assert.True(responseObject.Programme == Programme.Industriell_ekonomi, "Wrong programme. Expected: 14. Received: " + responseObject.Programme.ToString());
            Assert.True(responseObject.Year == 1, "Wrong year. Expected: 1. Received: " + responseObject.Year.ToString());
            Assert.True(responseObject.UserId == -3, "Wrong User Id. Expected: -3. Received: " + responseObject.UserId.ToString());
            Assert.True(responseObject.LinkedIn.Equals("https://www.linkedin.com/in/test"), "Wrong likedin. Expected: https://www.linkedin.com/in/test. Received:" + responseObject.LinkedIn.ToString());

            //Verify Restore
            var responseObject2 = JsonConvert.DeserializeObject<Student>(await response2.Content.ReadAsStringAsync());
            Assert.True(responseObject2.Id == -2, "Wrong Student Id. Expected: -2. Received: " + responseObject2.Id.ToString());
            Assert.True(responseObject2.Programme == Programme.Industriell_ekonomi, "Wrong programme. Expected: 14. Received: " + responseObject2.Programme.ToString());
            Assert.True(responseObject2.Year == 2, "Wrong year. Expected: 2. Received: " + responseObject2.Year.ToString());
            Assert.True(responseObject2.LinkedIn.Equals(""), "Wrong likedin. Expected: \"\". Received:" + responseObject2.LinkedIn.ToString());
        }

        [Fact]
        public async Task UpdatePartialAsAdminCorrect()
        {
            //Setup
            var client = await TestUtils.Login("admin");
            var json = new JsonObject
            {
                { "programme", 19 },
                { "linkedIn", "linkedin.com" },
                { "masterTitle", "Math" },
                { "year", 10 }
            };

            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await client.PutAsync("api/students/-1", payload);

            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), "Wrong status code. Expected: OK. Received: " + response.StatusCode.ToString());

            //Restore
            var json2 = new JsonObject
            {
                { "Programme", 18 },
                { "linkedin", "" },
                { "mastertitle", "Project management in software systems" },
                { "year", 4 }
            };

            var payload2 = new StringContent(json2.ToString(), Encoding.UTF8, "application/json");
            var response2 = await client.PutAsync("api/students/-1", payload2);

            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), "Wrong status code. Expected: OK. Received: " + response2.StatusCode.ToString());

            var responseObject = JsonConvert.DeserializeObject<Student>(await response.Content.ReadAsStringAsync());
            var responseObject2 = JsonConvert.DeserializeObject<Student>(await response2.Content.ReadAsStringAsync());

            //Assertions of response
            Assert.True(responseObject.Id == -1, "Wrong Student Id. Expected: -1. Received: " + responseObject.Id.ToString());
            Assert.True(responseObject.Programme == Programme.Teknisk_Fysik, "Wrong programme. Expected: 19. Received: " + responseObject.Programme.ToString());
            Assert.True(responseObject.Year == 4, "Wrong year. Expected: 4. Received: " + responseObject.Year.ToString());
            Assert.True(responseObject.UserId == -2, "Wrong User Id. Expected: -2. Received: " + responseObject.UserId.ToString());
            Assert.True(responseObject.MasterTitle.Equals("Math"), "Wrong master title. Expected: Math. Received: " + responseObject.MasterTitle.ToString());

            //Verify Restore
            Assert.True(responseObject2.Id == -1, "Wrong Student Id. Expected: -1. Received: " + responseObject2.Id.ToString());
            Assert.True(responseObject2.Programme == Programme.Datateknik, "Wrong programme. Expected: 18. Received: " + responseObject2.Programme.ToString());
            Assert.True(responseObject2.Year == 4, "Wrong year. Expected: 4. Received: " + responseObject2.Year.ToString());
            Assert.True(responseObject2.LinkedIn.Equals(""), "Wrong likedin. Expected: \"\". Received:" + responseObject2.LinkedIn.ToString());
            Assert.True(responseObject2.MasterTitle.Equals("Project management in software systems"), "Wrong master title. Expected: Project management in software systems. Received: " + responseObject2.MasterTitle.ToString());
        }



        [Fact]
        public async Task UpdateStudentAsAdminNotFound()
        {
            var client =  await TestUtils.Login("admin");
            var json = new JsonObject
            {
                { "linkedIn", "linkedin.com" },
                { "year", 10 }
            };
            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await client.PutAsync("api/students/-123", payload);

            Assert.True(response.StatusCode.Equals(HttpStatusCode.NotFound), "Wrong status code. Expected: NotFound. Received: " + response.StatusCode.ToString());
        }

        [Fact]
        public async Task UpdateStudentAsCompanyRepUsingIdForbidden()
        {
            var client = await TestUtils.Login("company1");
            var json = new JsonObject
            {
                { "linkedIn", "linkedin.com" },
                { "year", 10 }
            };

            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await client.PutAsync("api/students/-1", payload);

            Assert.True(response.StatusCode.Equals(HttpStatusCode.Forbidden), "Wrong status code. Expected: Forbidden. Received: " + response.StatusCode.ToString());
        }

        [Fact]
        public async Task UpdateStudentAsCompanyRepUsingMeForbbiden()
        {
            var client = await TestUtils.Login("company1");
            var json = new JsonObject
            {
                { "linkedIn", "linkedin.com" },
                { "year", 10 }
            };

            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await client.PutAsync("api/students/me", payload);

            Assert.True(response.StatusCode.Equals(HttpStatusCode.Forbidden), "Wrong status code. Expected: Forbidden. Received: " + response.StatusCode.ToString());
        }


    }
}
