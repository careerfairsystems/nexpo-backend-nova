using System;
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
    public class StudentSessionTimeslotTest
    {
        [Fact]
        public async Task GetAllByCompanyId()
        {
            var client = await TestUtils.Login("student1");
            var response = await client.GetAsync("/api/timeslots/company/-1");

            var responseList = JsonConvert.DeserializeObject<List<StudentSessionTimeslot>>(await response.Content.ReadAsStringAsync());

            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), "Wrong status code. Expected: OK. Received: " + response.StatusCode.ToString());
            Assert.True(responseList.Count == 3, "Wrong number of timeslots. Expected: 3. Received: " + responseList.Count.ToString());
        }

        [Fact]
        public async Task GetAllByCompanyIdNotExist()
        {
            var client = await TestUtils.Login("student1");
            var response = await client.GetAsync("/api/timeslots/company/-4");

            var responseList = JsonConvert.DeserializeObject<List<StudentSessionTimeslot>>(await response.Content.ReadAsStringAsync());

            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), "Wrong status code. Expected: OK. Received: " + response.StatusCode.ToString());
            Assert.True(responseList.Count == 0, "Wrong number of timeslots. Expected: 0. Received: " + responseList.Count.ToString());
        }

        [Fact]
        public async Task Get()
        {
            var client = await TestUtils.Login("student1");
            var response = await client.GetAsync("/api/timeslots/-1");

            var app = JsonConvert.DeserializeObject<StudentSessionTimeslot>(await response.Content.ReadAsStringAsync());

            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), "Wrong status code. Expected: OK. Received: " + response.StatusCode.ToString());
            Assert.True(app.Start.Equals(DateTime.Parse("2021-11-21 10:00")), "Wrong time. Expected: 2021-11-21 10:00. Received: " + app.Start);
        }

        [Fact]
        public async Task GetNotFound()
        {
            var client = await TestUtils.Login("student1");
            var response = await client.GetAsync("/api/timeslots/-123");

            // Verify response - Not found because the timeslot of id -123 does not exist in the database
            Assert.True(response.StatusCode.Equals(HttpStatusCode.NotFound), "Wrong status code. Expected: NotFound. Received: " + response.StatusCode.ToString());
        }

        [Fact]
        public async Task GetAllCompaniesWithTimeslots()
        {
            var client = await TestUtils.Login("student1");
            var response = await client.GetAsync("/api/timeslots/companies");

            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), "Wrong status code. Expected: OK. Received: " + response.StatusCode.ToString());

            var responseList = JsonConvert.DeserializeObject<List<PublicCompanyDTO>>(await response.Content.ReadAsStringAsync());
            var app3 = responseList.Find(timeslot => timeslot.Id == -1);

            Assert.True(responseList.Count == 3, "Wrong number of companies. Expected: 3. Received: " + responseList.Count);
            Assert.True(app3.Name.Equals("Apple"), "Wrong company name. Expected: Apple. Received: " + app3.Name);
        }

        [Fact]
        public async Task BookNotFoundCompany()
        {
            var client = await TestUtils.Login("student1");

            var payload = new StringContent("", Encoding.UTF8, "application/json");
            var response = await client.PutAsync("/api/timeslots/book/-123", payload);

            // Verify response - Not found because the timeslot of id -123 does not exist in the database
            Assert.True(response.StatusCode.Equals(HttpStatusCode.NotFound), "Wrong status code. Expected: NotFound. Received: " + response.StatusCode.ToString());
        }

        [Fact]
        public async Task BookNotFoundApplication()
        {
            var client = await TestUtils.Login("student1");
            var payload = new StringContent("", Encoding.UTF8, "application/json");

            var response = await client.PutAsync("/api/timeslots/book/-3", payload);

            // Verify response - Not found because the timeslot of id -3 does not have an application from the student
            Assert.True(response.StatusCode.Equals(HttpStatusCode.BadRequest), "Wrong status code. Expected: BadRequest. Received: " + response.StatusCode.ToString());
        }

        [Fact]
        public async Task BookNotAccepted()
        {
            var client = await TestUtils.Login("student1");
            var payload = new StringContent("", Encoding.UTF8, "application/json");

            var response = await client.PutAsync("/api/timeslots/book/-1", payload);

            // Verify response - Not found because the timeslot of id -1 has an application from the student but it is not accepted by the company
            Assert.True(response.StatusCode.Equals(HttpStatusCode.BadRequest), "Wrong status code. Expected: BadRequest. Received: " + response.StatusCode.ToString());
        }

        [Fact]
        public async Task UnbookNotFound()
        {
            var client = await TestUtils.Login("student1");
            var payload = new StringContent("", Encoding.UTF8, "application/json");
            
            var response = await client.PutAsync("/api/timeslots/unbook/-123", payload);

            Assert.True(response.StatusCode.Equals(HttpStatusCode.NotFound), "Wrong status code. Expected: NotFound. Received: " + response.StatusCode.ToString());
        }

        [Fact]
        public async Task UnbookNoApplication()
        {
            var client = await TestUtils.Login("student1");
            var payload = new StringContent("", Encoding.UTF8, "application/json");

            var response = await client.PutAsync("/api/timeslots/book/-7", payload);

            // Verify response - Not found because the timeslot of id -7 does not have an application from the student
            Assert.True(response.StatusCode.Equals(HttpStatusCode.BadRequest), "Wrong status code. Expected: BadReqest. Received: " + response.StatusCode.ToString());
        }

        [Fact]
        public async Task UnbookNotAccepted()
        {
            var client = await TestUtils.Login("student1");
            var payload = new StringContent("", Encoding.UTF8, "application/json");

            var response = await client.PutAsync("/api/timeslots/book/-1", payload);

            // Verify response - Not found because the timeslot of id -1 has an application from the student but it is not accepted by the company
            Assert.True(response.StatusCode.Equals(HttpStatusCode.BadRequest), "Wrong status code. Expected: BadReqest. Received: " + response.StatusCode.ToString());
        }

        [Fact]
        public async Task BookAndUnbook()
        {
            var studentClient = await TestUtils.Login("student2");
            var companyClient = await TestUtils.Login("company1");

            //Accept student application
            var json = new JsonObject
            {
                { "status", 1 }
            };

            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await companyClient.PutAsync("api/applications/-2", payload);

            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), "Wrong status code. Expected: OK. Received: " + response.StatusCode);

            //Book studentsession as student
            payload = new StringContent("", Encoding.UTF8, "application/json");
            var response2 = await studentClient.PutAsync("/api/timeslots/book/-2", payload);

            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), "Wrong status code. Expected: OK. Received: " + response.StatusCode.ToString());
            
            //Unbook studentsession as student
            payload = new StringContent("", Encoding.UTF8, "application/json");
            var response3 = await studentClient.PutAsync("/api/timeslots/unbook/-2", payload);

            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), "Wrong status code. Expected: OK. Received: " + response.StatusCode.ToString());

            //Restore student application
            json = new JsonObject
            {
                { "status", 0 }
            };

            payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            response = await companyClient.PutAsync("api/applications/-2", payload);

            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), "Wrong status code. Expected: OK. Received: " + response.StatusCode);

            //Verify behaviour
            var responseObject1 = JsonConvert.DeserializeObject<StudentSessionTimeslot>((await response2.Content.ReadAsStringAsync()));

            Assert.True(responseObject1.Location.Equals("Zoom"), "Wrong location. Expected: Zoom. Received: " + responseObject1.Location.ToString());
            Assert.True(responseObject1.StudentId == -2, "Wrong student id. Expected: -2. Recieved: " + responseObject1.StudentId.ToString());

            var responseObject2 = JsonConvert.DeserializeObject<StudentSessionTimeslot>(await response3.Content.ReadAsStringAsync());

            Assert.True(responseObject2.Location.Equals("Zoom"), "Wrong location. Expected: Zoom. Received: " + responseObject2.Location.ToString());
            Assert.True(responseObject2.StudentId == null, "Wrong student id. Expected: null. Recieved: " + responseObject2.StudentId.ToString());
        }

        [Fact]
        public async Task BookMultipleTimeslots()
        {
            var studentClient = await TestUtils.Login("student3");
            var companyClient = await TestUtils.Login("company1");

            //Accept student application
            var json = new JsonObject
            {
                { "status", 1 }
            };

            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await companyClient.PutAsync("api/applications/-3", payload);

            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), "Wrong status code. Expected: OK. Received: " + response.StatusCode);

            //Book studentsession as student
            payload = new StringContent("", Encoding.UTF8, "application/json");
            var response2 = await studentClient.PutAsync("/api/timeslots/book/-3", payload);

            Assert.True(response2.StatusCode.Equals(HttpStatusCode.OK), "Wrong status code. Expected: OK. Received: " + response.StatusCode.ToString());
            
            //Book another studentsession as student
            payload = new StringContent("", Encoding.UTF8, "application/json");
            response = await studentClient.PutAsync("/api/timeslots/book/-1", payload);

            // Verify response - Bad request because the student already has a booked timeslot
            Assert.True(response.StatusCode.Equals(HttpStatusCode.BadRequest), "Wrong status code. Expected: BadRequest. Received: " + response.StatusCode.ToString());

            //Unbook studentsession as student
            payload = new StringContent("", Encoding.UTF8, "application/json");
            var response3 = await studentClient.PutAsync("/api/timeslots/unbook/-3", payload);

            Assert.True(response3.StatusCode.Equals(HttpStatusCode.OK), "Wrong status code. Expected: OK. Received: " + response.StatusCode.ToString());

            //Restore student application
            json = new JsonObject
            {
                { "status", 0 }
            };

            payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            response = await companyClient.PutAsync("api/applications/-2", payload);

            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), "Wrong status code. Expected: OK. Received: " + response.StatusCode);

            //Verify
            var responseObject1 = JsonConvert.DeserializeObject<StudentSessionTimeslot>(await response2.Content.ReadAsStringAsync());

            Assert.True(responseObject1.Location.Equals("Zoom"), "Wrong location. Expected: Zoom. Received: " + responseObject1.Location.ToString());
            Assert.True(responseObject1.StudentId == -3, "Wrong student id. Expected: -3. Recieved: " + responseObject1.StudentId.ToString());

            var responseObject2 = JsonConvert.DeserializeObject<StudentSessionTimeslot>(await response3.Content.ReadAsStringAsync());

            Assert.True(responseObject2.Location.Equals("Zoom"), "Wrong location. Expected: Zoom. Received: " + responseObject2.Location.ToString());
            Assert.True(responseObject2.StudentId == null, "Wrong student id. Expected: null. Recieved: " + responseObject2.StudentId.ToString());
        }

        [Fact]
        public async Task UnbookNoBook()
        {
            var studentClient = await TestUtils.Login("student1");
            var companyClient = await TestUtils.Login("company1");

            //Accept student application
            var json = new JsonObject
            {
                { "status", 1 }
            };

            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await companyClient.PutAsync("api/applications/-1", payload);

            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), "Wrong status code. Expected: OK. Received: " + response.StatusCode);

            //Unbook studentsession as student
            payload = new StringContent("", Encoding.UTF8, "application/json");
            response = await studentClient.PutAsync("/api/timeslots/unbook/-1", payload);

            // Verify response - Bad request because the student has no booked timeslot
            Assert.True(response.StatusCode.Equals(HttpStatusCode.BadRequest), "Wrong status code. Expected: BadRequest. Received: " + response.StatusCode.ToString());

            //Restore student application
            json = new JsonObject
            {
                { "status", 0 }
            };

            payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            response = await companyClient.PutAsync("api/applications/-1", payload);

            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), "Wrong status code. Expected: OK. Received: " + response.StatusCode);
        }

        [Fact]
        public async Task UnbookNotBooked()
        {
            var studentClient1 = await TestUtils.Login("student1");
            var studentClient2 = await TestUtils.Login("student2");
            var companyClient = await TestUtils.Login("company2");

            //Accept student applications
            var json = new JsonObject
            {
                { "status", 1 }
            };

            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response1 = await companyClient.PutAsync("api/applications/-4", payload);
            var response2 = await companyClient.PutAsync("api/applications/-5", payload);

            Assert.True(response1.StatusCode.Equals(HttpStatusCode.OK), "Wrong status code. Expected: OK. Received: " + response1.StatusCode);
            Assert.True(response2.StatusCode.Equals(HttpStatusCode.OK), "Wrong status code. Expected: OK. Received: " + response2.StatusCode);

            //Book studentsession as student1
            payload = new StringContent("", Encoding.UTF8, "application/json");
            var response3 = await studentClient1.PutAsync("/api/timeslots/book/-4", payload);

            Assert.True(response3.StatusCode.Equals(HttpStatusCode.OK), "Wrong status code. Expected: OK. Received: " + response3.StatusCode.ToString());
            
            //Unbook same studentsession as student2
            payload = new StringContent("", Encoding.UTF8, "application/json");
            var response4 = await studentClient2.PutAsync("/api/timeslots/unbook/-4", payload);

            // Verify response - Bad request because the student is not the one who booked the timeslot
            Assert.True(response4.StatusCode.Equals(HttpStatusCode.BadRequest), "Wrong status code. Expected: BadRequest. Received: " + response4.StatusCode.ToString());

            //Unbook studentsession as student1
            payload = new StringContent("", Encoding.UTF8, "application/json");
            var response5 = await studentClient1.PutAsync("/api/timeslots/unbook/-4", payload);
            Assert.True(response5.StatusCode.Equals(HttpStatusCode.OK), "Wrong status code. Expected: OK. Received: " + response5.StatusCode.ToString());
            
            //Restore student application
            json = new JsonObject
            {
                { "status", 0 }
            };

            payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response6 = await companyClient.PutAsync("api/applications/-4", payload);
            var response7 = await companyClient.PutAsync("api/applications/-5", payload);

            Assert.True(response6.StatusCode.Equals(HttpStatusCode.OK), "Wrong status code. Expected: OK. Received: " + response6.StatusCode);
            Assert.True(response7.StatusCode.Equals(HttpStatusCode.OK), "Wrong status code. Expected: OK. Received: " + response7.StatusCode);

            //Verify
            var responseObject1 = JsonConvert.DeserializeObject<StudentSessionTimeslot>(await response3.Content.ReadAsStringAsync());

            Assert.True(responseObject1.Location.Equals("Zoom"), "Wrong location. Expected: Zoom. Receievd: " + responseObject1.Location.ToString());
            Assert.True(responseObject1.StudentId == -1, "Wrong student id. Expected: -1. Receievd: " + responseObject1.StudentId.ToString());

            var responseObject2 = JsonConvert.DeserializeObject<StudentSessionTimeslot>(await response5.Content.ReadAsStringAsync());

            Assert.True(responseObject2.Location.Equals("Zoom"), "Wrong location. Expected: Zoom. Receievd: " + responseObject2.Location.ToString());
            Assert.True(responseObject2.StudentId == null, "Wrong student id. Expected: null. Receievd: " + responseObject2.StudentId.ToString());
        }

        [Fact]
        public async Task BookOnAlreadyBooked()
        {
            var studentClient1 = await TestUtils.Login("student1");
            var studentClient2 = await TestUtils.Login("student2");
            var companyClient = await TestUtils.Login("company2");

            //Accept student applications
            var json = new JsonObject
            {
                { "status", 1 }
            };

            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response1 = await companyClient.PutAsync("api/applications/-4", payload);
            var response2 = await companyClient.PutAsync("api/applications/-5", payload);

            Assert.True(response1.StatusCode.Equals(HttpStatusCode.OK), "Wrong status code. Expected: OK. Received: " + response1.StatusCode);
            Assert.True(response2.StatusCode.Equals(HttpStatusCode.OK), "Wrong status code. Expected: OK. Received: " + response2.StatusCode);

            //Book studentsession as student1
            payload = new StringContent("", Encoding.UTF8, "application/json");
            var response3 = await studentClient1.PutAsync("/api/timeslots/book/-5", payload);

            Assert.True(response3.StatusCode.Equals(HttpStatusCode.OK), "Wrong status code. Expected: OK. Received: " + response3.StatusCode.ToString());

            //Book same studentsession as student2
            payload = new StringContent("", Encoding.UTF8, "application/json");
            var response4 = await studentClient2.PutAsync("/api/timeslots/book/-5", payload);

            // Verify response - Bad request because the timeslot is already booked
            Assert.True(response4.StatusCode.Equals(HttpStatusCode.BadRequest), "Wrong status code. Expected: BadRequest. Received: " + response4.StatusCode.ToString());

            //Unbook studentsession as student
            payload = new StringContent("", Encoding.UTF8, "application/json");
            var response5 = await studentClient1.PutAsync("/api/timeslots/unbook/-5", payload);
            Assert.True(response5.StatusCode.Equals(HttpStatusCode.OK), "Wrong status code. Expected: OK. Received: " + response5.StatusCode.ToString());

            //Restore student application
            json = new JsonObject
            {
                { "status", 0 }
            };

            payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response6 = await companyClient.PutAsync("api/applications/-4", payload);
            var response7 = await companyClient.PutAsync("api/applications/-5", payload);

            Assert.True(response6.StatusCode.Equals(HttpStatusCode.OK), "Wrong status code. Expected: OK. Received: " + response6.StatusCode);
            Assert.True(response7.StatusCode.Equals(HttpStatusCode.OK), "Wrong status code. Expected: OK. Received: " + response7.StatusCode);

            //Verify
            var responseObject1 = JsonConvert.DeserializeObject<StudentSessionTimeslot>(await response3.Content.ReadAsStringAsync());

            Assert.True(responseObject1.Location.Equals("Zoom"), "Wrong location. Expected: Zoom. Receievd: " + responseObject1.Location.ToString());
            Assert.True(responseObject1.StudentId == -1, "Wrong student id. Expected: -1. Receievd: " + responseObject1.StudentId.ToString());

            var responseObject2 = JsonConvert.DeserializeObject<StudentSessionTimeslot>(await response5.Content.ReadAsStringAsync());

            Assert.True(responseObject2.Location.Equals("Zoom"), "Wrong location. Expected: Zoom. Receievd: " + responseObject2.Location.ToString());
            Assert.True(responseObject2.StudentId == null, "Wrong student id. Expected: null. Receievd: " + responseObject2.StudentId.ToString());
        }

                [Fact]
        public async Task DeleteNonExisting()
        {
            var client = await TestUtils.Login("admin");
            var response = await client.DeleteAsync("api/timeslots/-123");

            //Verify response - Not found because timeslot of id -123 does not exist in the database
            Assert.True(response.StatusCode.Equals(HttpStatusCode.NotFound), "Wrong status code. Expected: NotFound. Received: " + response.StatusCode);
        }

        [Fact]
        public async Task UpdateLocation()
        {
            var client = await TestUtils.Login("admin");
            var json = new JsonObject
            {
                { "location", "E:A" }
            };  

            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response1 = await client.PutAsync("api/timeslots/-6", payload);

            Assert.True(response1.StatusCode.Equals(HttpStatusCode.OK), "Wrong status code. Expected: OK. Received: " + response1.StatusCode);

            //Restore 
            json = new JsonObject
            {
                { "location", "Zoom" }
            };

            payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response2 = await client.PutAsync("api/timeslots/-6", payload);

            Assert.True(response2.StatusCode.Equals(HttpStatusCode.OK), "Wrong status code. Expected: OK. Received: " + response2.StatusCode);
            
            //Verify
            var responseObject1 = JsonConvert.DeserializeObject<StudentSessionTimeslot>(await response1.Content.ReadAsStringAsync());

            Assert.True(responseObject1.Location.Equals("E:A"), "Wrong location. Expected: E:A. Received: " + responseObject1.Location.ToString());
            Assert.True(responseObject1.CompanyId == -3, "Wrong CompandyId. Expected: -3. Received: " + responseObject1.CompanyId.ToString());
            Assert.True(responseObject1.Start.Equals(DateTime.Parse("2021-11-23 12:00")), "Wrong start time. Expected: 2021-11-23 12:00. Received: " + responseObject1.Start);

            var responseObject2 = JsonConvert.DeserializeObject<StudentSessionTimeslot>(await response2.Content.ReadAsStringAsync());

            Assert.True(responseObject2.Location.Equals("Zoom"), "Wrong location. Expected: Zoom. Received: " + responseObject2.Location.ToString());
            Assert.True(responseObject2.CompanyId == -3, "Wrong CompandyId. Expected: -3. Received: " + responseObject2.CompanyId.ToString());
            Assert.True(responseObject2.Start.Equals(DateTime.Parse("2021-11-23 12:00")), "Wrong start time. Expected: 2021-11-23 12:00. Received: " + responseObject2.Start);
        }

        [Fact]
        public async Task UpdateLocationNonExisting()
        {
            var client = await TestUtils.Login("admin");
            var json = new JsonObject
            {
                { "location", "E:A" }
            };

            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await client.PutAsync("api/timeslots/-123", payload);

            //Verify response - Not found because timeslot of id -123 does not exist in the database
            Assert.True(response.StatusCode.Equals(HttpStatusCode.NotFound), "Wrong status code. Expected: NotFound. Received: " + response.StatusCode);
        }

        [Fact]
        public async Task CreateAndDelete()
        {
            var client =  await TestUtils.Login("admin");

            //Add new timeslot
            var json = new JsonObject
            {
                { "start", DateTime.Parse("2021-11-15 12:45") },
                { "end", DateTime.Parse("2021-11-15 13:15") },
                { "companyid", "-3" },
                { "location", "At home" }
            };

            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response1 = await client.PostAsync("api/timeslots/add", payload);
            var responseObject = JsonConvert.DeserializeObject<StudentSessionTimeslot>(await response1.Content.ReadAsStringAsync());

            Assert.True(response1.StatusCode.Equals(HttpStatusCode.OK), "Wrong status code. Expected: OK. Received: " + response1.StatusCode);

            //Verify new total number of timeslots
            var response2 = await client.GetAsync("/api/timeslots/company/-3");
            Assert.True(response2.StatusCode.Equals(HttpStatusCode.OK), "Wrong status code. Expected: OK. Received: " + response2.StatusCode.ToString());

            //Remove new timeslot
            var response3 = await client.DeleteAsync("api/timeslots/" + responseObject.Id);
            Assert.True(response3.StatusCode.Equals(HttpStatusCode.NoContent), "Wrong status code. Expected: NoContent. Received: " + response3.StatusCode);
            
            //Verify total number of timeslots 
            var response4 = await client.GetAsync("/api/timeslots/company/-3");
            Assert.True(response4.StatusCode.Equals(HttpStatusCode.OK), response4.StatusCode.ToString());
            
            //Verify
            Assert.True(responseObject.Location.Equals("At home"), "Wrong Location. Expected: At home. Received: " +  responseObject.Location.ToString());
            Assert.True(responseObject.StudentId == null, "Wrong student id. Expected: null. Received: " + responseObject.StudentId.ToString());

            var responseList1 = JsonConvert.DeserializeObject<List<StudentSessionTimeslot>>(await response2.Content.ReadAsStringAsync());
            Assert.True(responseList1.Count == 3, "Wrong number of timeslots. Expected: 2. Received: " + responseList1.Count.ToString());

            var responseList2 = JsonConvert.DeserializeObject<List<StudentSessionTimeslot>>(await response4.Content.ReadAsStringAsync());
            Assert.True(responseList2.Count == 2, "Wrong number of timeslots. Expected: 1. Received: " + responseList2.Count.ToString());
        }
    }
}
