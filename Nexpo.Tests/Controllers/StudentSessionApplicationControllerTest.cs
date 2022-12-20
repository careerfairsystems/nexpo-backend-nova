using System.Collections.Generic;
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
        [Fact]
        public async Task GetAllApplicationsAsCompany()
        {
            var client = await TestUtils.Login("company1");
            var response = await client.GetAsync("/api/applications/my/company");
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), "Wrong status code. Expected: OK. Received: " + response.StatusCode.ToString());

            var responseList = JsonConvert.DeserializeObject<List<StudentSessionApplicationDto>>(await response.Content.ReadAsStringAsync());
            Assert.True(responseList.Count == 3, "Wrong number of applications. Expected: 3. Received: " + responseList.Count.ToString());

            var app3 = responseList.Find(r => r.Id == -3);

            Assert.True(app3.Motivation.Equals("User experience is very important for me"), "Wrong motivation. Expected: \"User experience is very important for me\". Received: " + app3.Motivation);
            Assert.True(app3.StudentYear == 3, "Wrong student year. Expected: 3. Received: " + app3.StudentYear.ToString());
            Assert.True(app3.StudentProgramme == Programme.Väg_och_vatttenbyggnad, "Wrong student programme. Expected: 12. Received: " + app3.StudentProgramme.ToString());
            Assert.True(app3.StudentFirstName.Equals("Gamma"), "Wrong student first name. Expected: Gamma. Received: " + app3.StudentFirstName);
            Assert.True(app3.StudentLastName.Equals("Student"), "Wrong student last name. Expected: Student. Received: " + app3.StudentLastName);
        }

        [Fact]
        public async Task GetAllApplicationsAsCompanyWrongPath()
        {
            var client = await TestUtils.Login("company1");
            var response = await client.GetAsync("/api/applications/my/student");

            Assert.True(response.StatusCode.Equals(HttpStatusCode.Forbidden), "Wrong status code. Expected: Forbidden. Received: " + response.StatusCode.ToString());
        }

        [Fact]
        public async Task GetAllApplicationsAsStudent()
        {
            var client = await TestUtils.Login("student1");
            var response = await client.GetAsync("/api/applications/my/student");
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), "Wrong status code. Expected: OK. Received: " + response.StatusCode.ToString());
            
            var responseList = JsonConvert.DeserializeObject<List<StudentSessionApplication>>(await response.Content.ReadAsStringAsync());
            Assert.True(responseList.Count == 2, "Wrong number of applications. Expected: 2. Received: " + responseList.Count.ToString());

            var app = responseList.Find(r => r.Id == -4);

            Assert.True(app.Motivation.Equals("I would like to learn more about searching"), "Wrong motivation. Received: " + app.Motivation);
            Assert.True(app.StudentId == -1, "Wrong student id. Expected: -1. Received: " + app.StudentId.ToString());
            Assert.True(app.CompanyId == -2, "Wrong company id. Expected: -2. Received: " + app.CompanyId.ToString());
        }

        [Fact]
        public async Task GetAllApplicationsAsStudentWrongPath()
        {
            var client = await TestUtils.Login("student1");
            var response = await client.GetAsync("/api/applications/my/company");

            Assert.True(response.StatusCode.Equals(HttpStatusCode.Forbidden), "Wrong status code. Expected: Forbidden. Received: " + response.StatusCode.ToString());
        }

        [Fact]
        public async Task GetApplicationAsStudent()
        {
            var client =  await TestUtils.Login("student1");

            //Get students own application
            var response = await client.GetAsync("/api/applications/-1");
            var app = JsonConvert.DeserializeObject<StudentSessionApplication>(await response.Content.ReadAsStringAsync());

            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), "Wrong status code. Expected: OK. Received: " + response.StatusCode.ToString());
            Assert.True(app.Motivation.Equals("Hej, jag är jättebra och tror att ni vill träffa mig!"), "Wrong motivation. Received: " + app.Motivation);
        }

        [Fact]
        public async Task GetAnotherApplicationAsStudent()
        {
            var client = await TestUtils.Login("student1");
            var response = await client.GetAsync("/api/applications/-3");

            Assert.True(response.StatusCode.Equals(HttpStatusCode.Forbidden), "Wrong status code. Expected: Forbidden. Received: " + response.StatusCode.ToString());
        }

        [Fact]
        public async Task GetApplicationAsCompany()
        {
            var client = await TestUtils.Login("company1");
            var response = await client.GetAsync("/api/applications/-1");
            var app = JsonConvert.DeserializeObject<StudentSessionApplication>(await response.Content.ReadAsStringAsync());

            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), "Wrong status code. Expected: OK. Received: " + response.StatusCode.ToString());
            Assert.True(app.Motivation.Equals("Hej, jag är jättebra och tror att ni vill träffa mig!"), "Wrong motivation. Received: " + app.Motivation);
        }

        [Fact]
        public async Task GetAnotherCompanyApplication()
        {
            var client = await TestUtils.Login("company1");
            var response = await client.GetAsync("/api/applications/-4");

            Assert.True(response.StatusCode.Equals(HttpStatusCode.Forbidden), "Wrong status code. Expected: Forbidden. Received: " + response.StatusCode.ToString());
        }

        [Fact]
        public async Task GetCompanyApplicationNotExist()
        {
            var client = await TestUtils.Login("company1");
            var response = await client.GetAsync("/api/applications/-123");

            Assert.True(response.StatusCode.Equals(HttpStatusCode.NotFound), "Wrong status code. Expected: NotFound. Received: " + response.StatusCode.ToString());
        }

        [Fact]
        public async Task RespondToApplicationAsCompany()
        {
            var client = await TestUtils.Login("company2");

            //Update status
            var json = new JsonObject
            {
                { "status", 1 }
            };
            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response1 = await client.PutAsync("api/applications/-5", payload);
            Assert.True(response1.StatusCode.Equals(HttpStatusCode.OK), "Wrong status code. Expected: OK. Received: " + response1.StatusCode.ToString());

            //Check update worked
            var response2 = await client.GetAsync("/api/applications/-5");
            Assert.True(response2.StatusCode.Equals(HttpStatusCode.OK), "Wrong status code. Expected: OK. Received: " + response2.StatusCode.ToString());

            //Roll back
            json = new JsonObject
            {
                { "status", 0 }
            };
            payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response3 = await client.PutAsync("api/applications/-5", payload);
            Assert.True(response3.StatusCode.Equals(HttpStatusCode.OK), "Wrong status code. Expected: OK. Received: " + response3.StatusCode.ToString());

            //Verify change
            var app = JsonConvert.DeserializeObject<StudentSessionApplication>(await response2.Content.ReadAsStringAsync());
            Assert.True(app.Status == StudentSessionApplicationStatus.Accepted, "Status didn't update, got: " + app.Status);
        }

        [Fact]
        public async Task RespondToApplicationAsCompanyNotExist()
        {
            var client = await TestUtils.Login("company1");

            //Update status
            var json = new JsonObject
            {
                { "status", 1 }
            };
            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await client.PutAsync("api/applications/-123", payload);

            Assert.True(response.StatusCode.Equals(HttpStatusCode.NotFound), "Wrong status code. Expected: NotFound. Received: " + response.StatusCode.ToString());
        }

        [Fact]
        public async Task RespondToAnotherCompanyApplication()
        {
            var client = await TestUtils.Login("company1");

            //Update status
            var json = new JsonObject
            {
                { "status", 1 }
            };
            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await client.PutAsync("api/applications/-4", payload);

            Assert.True(response.StatusCode.Equals(HttpStatusCode.Forbidden), "Wrong status code. Expected: Forbidden. Received: " + response.StatusCode.ToString());
        }

        [Fact]
        public async Task RespondToApplicationAsStudent()
        {
            var client =  await TestUtils.Login("student1");

            //Update status
            var json = new JsonObject
            {
                { "status", 1 }
            };
            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await client.PutAsync("api/applications/-1", payload);

            Assert.True(response.StatusCode.Equals(HttpStatusCode.Forbidden), "Wrong status code. Expected: Forbidden. Received: " + response.StatusCode.ToString());
        }

        [Fact]
        public async Task PostApplicationAsStudentThenDelete()
        {
            var studentClient = await TestUtils.Login("student1");
            var companyClient = await TestUtils.Login("company3");

            //Post application
            var json = new JsonObject
            {
                { "motivation", "Hej, jag är jättebra och tror att ni vill träffa mig!" }
            };
            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await studentClient.PostAsync("api/applications/company/-3", payload);
            var responseObject = JsonConvert.DeserializeObject<StudentSessionApplicationDto>(await response.Content.ReadAsStringAsync());
            Assert.True(response.StatusCode.Equals(HttpStatusCode.Created), "Wrong status code. Expected: OK. Received: " + response.StatusCode.ToString());

            //Check application as company
            response = await companyClient.GetAsync("/api/applications/my/company");
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), "Wrong status code. Expected: OK. Received: " + response.StatusCode.ToString());

            var appList1 = JsonConvert.DeserializeObject<List<StudentSessionApplicationDto>>(await response.Content.ReadAsStringAsync());
            int id = responseObject.Id.GetValueOrDefault();

            //Restore
            response = await studentClient.DeleteAsync("api/applications/" + id);
            Assert.True(response.StatusCode.Equals(HttpStatusCode.NoContent), "Wrong status code. Expected: NoContent. Received: " + response.StatusCode.ToString());

            response = await companyClient.GetAsync("/api/applications/my/company");
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), "Wrong status code. Expected: OK. Received: " + response.StatusCode.ToString());

            var appList2 = JsonConvert.DeserializeObject<List<StudentSessionApplicationDto>>(await response.Content.ReadAsStringAsync());
            var app1 = appList1.Find(r => r.Id == id);

            //Verify 
            Assert.True(appList1.Count == 2, "Application list length should be 2, count:" + appList1.Count.ToString());
            Assert.True(app1.Motivation.Equals("Hej, jag är jättebra och tror att ni vill träffa mig!"), "Wrong motivation, got: " + appList1[1].Motivation);
            Assert.True(appList2.Count == 1, "Application list length should be 1, count:" + appList2.Count.ToString());
        }

        [Fact]
        public async Task PostApplicationAsStudentNoTimeslots()
        {
            var studentClient = await TestUtils.Login("student1");

            //Post application
            var json = new JsonObject
            {
                { "motivation", "Hej, jag är jättebra och tror att ni vill träffa mig!" }
            };
            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await studentClient.PostAsync("api/applications/company/-4", payload);

            Assert.True(response.StatusCode.Equals(HttpStatusCode.Conflict), "Wrong status code. Expected: Conflict. Received: " + response.StatusCode.ToString());
        }

        [Fact]
        public async Task UpdateApplicationAsStudent()
        {
            var studentClient = await TestUtils.Login("student3");
            var companyClient = await TestUtils.Login("company2");

            //Post application
            var json = new JsonObject
            {
                { "motivation", "This is a test" }
            };
            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response1 = await studentClient.PostAsync("api/applications/company/-2", payload);
            Assert.True(response1.StatusCode.Equals(HttpStatusCode.Created), "Wrong status code. Expected: Created. Received: " + response1.StatusCode.ToString());

            //Check application as company
            var response2 = await companyClient.GetAsync("/api/applications/my/company");
            Assert.True(response2.StatusCode.Equals(HttpStatusCode.OK), "Wrong status code. Expected: OK. Received: " + response2.StatusCode.ToString());

            //Restore
            json = new JsonObject
            {
                { "motivation", "Search algrorithms are very cool" }
            };
            payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response3 = await studentClient.PostAsync("api/applications/company/-2", payload);
            Assert.True(response3.StatusCode.Equals(HttpStatusCode.Created), "Wrong status code. Expected: Created. Received: " + response3.StatusCode.ToString());

            var response4 = await companyClient.GetAsync("/api/applications/my/company");
            Assert.True(response4.StatusCode.Equals(HttpStatusCode.OK), "Wrong status code. Expected: OK. Received: " + response4.StatusCode.ToString());

            var appList1 = JsonConvert.DeserializeObject<List<StudentSessionApplicationDto>>(await response2.Content.ReadAsStringAsync());
            var appList2 = JsonConvert.DeserializeObject<List<StudentSessionApplicationDto>>(await response4.Content.ReadAsStringAsync());
            var responseObject = JsonConvert.DeserializeObject<StudentSessionApplicationDto>(await response1.Content.ReadAsStringAsync());
            int id = responseObject.Id.GetValueOrDefault();
            var app1 = appList1.Find(r => r.Id == id);
            var app2 = appList2.Find(r => r.Id == id);

            //Verify
            Assert.True(appList1.Count == 3, "Application list length should be 3, count:" + appList1.Count.ToString());
            Assert.True(app1.Motivation.Equals("This is a test"), "Wrong motivation, got: " + appList1[2].Motivation);
            Assert.True(appList2.Count == 3, "Application list length should be 3, count:" + appList2.Count.ToString());
            Assert.True(app2.Motivation.Equals("Search algrorithms are very cool"), "Wrong motivation, got: " + appList2[2].Motivation);
        }


        [Fact]
        public async Task GetApplicationAccepted()
        {
            var client = await TestUtils.Login("student1");
            var response = await client.GetAsync("/api/applications/accepted/-1");
            var status = JsonConvert.DeserializeObject<ApplicationStatusDto>(await response.Content.ReadAsStringAsync());

            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), "Wrong status code. Expected: OK. Received: " + response.StatusCode.ToString());
            Assert.True(!status.accepted, "Accepted should be false. Received: " + status.accepted.ToString());
        }

        [Fact]
        public async Task GetApplicationAcceptedForNonAppliedCompany()
        {
            var client = await TestUtils.Login("student1");
            var response = await client.GetAsync("/api/applications/accepted/-4");

            Assert.True(response.StatusCode.Equals(HttpStatusCode.BadRequest), "Wrong status code. Expected: BadRequest. Received: " + response.StatusCode.ToString());
        }

        [Fact]
        public async Task GetApplicationAcceptedWhenAccepted()
        {
            var studentClient = await TestUtils.Login("student2");
            var companyClient = await TestUtils.Login("company3");

            //Update status
            var json = new JsonObject
            {
                { "status", 1 }
            };
            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await companyClient.PutAsync("api/applications/-7", payload);
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), "Wrong status code. Expected: OK. Received: " + response.StatusCode.ToString());

            response = await studentClient.GetAsync("/api/applications/accepted/-3");
            var status = JsonConvert.DeserializeObject<ApplicationStatusDto>(await response.Content.ReadAsStringAsync());
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), "Wrong status code. Expected: OK. Received: " + response.StatusCode.ToString());
            Assert.True(status.accepted, "Accepted should be true. Received: " + status.accepted.ToString());

            //Roll back
            json = new JsonObject
            {
                { "status", 0 }
            };
            payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            response = await companyClient.PutAsync("api/applications/-7", payload);
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), "Wrong status code. Expected: OK. Received: " + response.StatusCode.ToString());
        }

        [Fact]
        public async Task GetApplicationAcceptedWhenDeclined()
        {
            var studentClient = await TestUtils.Login("student1");
            var companyClient = await TestUtils.Login("company2");

            //Update status
            var json = new JsonObject
            {
                { "status", 2 }
            };
            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await companyClient.PutAsync("api/applications/-4", payload);
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), "Wrong status code. Expected: OK. Received: " + response.StatusCode.ToString());

            response = await studentClient.GetAsync("/api/applications/accepted/-2");
            var status = JsonConvert.DeserializeObject<ApplicationStatusDto>((await response.Content.ReadAsStringAsync()));
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), "Wrong status code. Expected: OK. Received: " + response.StatusCode.ToString());
            Assert.True(!status.accepted, "Accepted should be false. Received: " + status.accepted.ToString());

            //Roll back
            json = new JsonObject
            {
                { "status", 0 }
            };
            payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            response = await companyClient.PutAsync("api/applications/-4", payload);
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), "Wrong status code. Expected: OK. Received: " + response.StatusCode.ToString());
        }
    }
}
