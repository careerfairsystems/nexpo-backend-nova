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
    public class EventApplicationControllerTest
    {
        [Fact]
        public async Task DeleteEventApplicationSuccess()
        {
            // Login as student1
            var client = await TestUtils.Login("student1");

            // Delete event application with id -1
            var response = await client.DeleteAsync("/api/eventapplications/-1");
            Assert.True(response.StatusCode.Equals(HttpStatusCode.NoContent), "Wrong Status Code. Expected: NoContent. Received: " + response.StatusCode.ToString());


            // Check that the event application was deleted
            response = await client.GetAsync("/api/eventapplications/my/student");
            var eventApplications = JsonConvert.DeserializeObject<List<EventApplicationDTO>>(await response.Content.ReadAsStringAsync());
            Assert.True(eventApplications.Count == 0, "Wrong number of event applications. Expected: 1. Received: " + eventApplications.Count);

            // Restore the deleted event application: var EventApplication1 = new EventApplication { Motivation = "I want to learn more about the company", StudentId = student1.Id.Value, EventId = event7.Id.Value, CompanyId = company5.Id.Value };
            var DTO = new UpdateEventApplicationStudentDTO{
                Motivation = "I want to learn more about the company",
                EventId = -7,
                CompanyId = -5,
                PhotoOk = true


            };
            var json = JsonConvert.SerializeObject(DTO);
            var data = new StringContent(json, Encoding.UTF8, "application/json");
            response = await client.PostAsync("/api/eventapplications/apply", data);

            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), "Wrong Status Code. Expected: OK. Received: " + response.StatusCode.ToString());


        }

        [Fact]
        public async Task DeleteEventApplicationNotFound()
        {
            var client = await TestUtils.Login("student1");

            var response = await client.DeleteAsync("/api/eventapplications/-100");
            
            Assert.True(response.StatusCode.Equals(HttpStatusCode.NotFound), "Wrong Status Code. Expected: NotFound. Received: " + response.StatusCode.ToString());
        }

        [Fact]
        public async Task DeleteEventApplicationUnauthorized()
        {
            var client = await TestUtils.Login("volunteer1");

            var response = await client.DeleteAsync("/api/eventapplications/-1");

            Assert.True(response.StatusCode.Equals(HttpStatusCode.Unauthorized), "Wrong Status Code. Expected: Unauthorized. Received: " + response.StatusCode.ToString());
        }

        [Fact]
        public async Task DeleteEventApplicationForbiddenVolunteer()
        {
            var client = await TestUtils.Login("student2");

            var response = await client.DeleteAsync("/api/eventapplications/-1");

            Assert.True(response.StatusCode.Equals(HttpStatusCode.Forbidden), "Wrong Status Code. Expected: Forbidden. Received: " + response.StatusCode.ToString());

        }

        [Fact]
        public async Task DeleteEventApplicationForbiddenCompanyRep()
        {
            var client = await TestUtils.Login("company2");

            var response = await client.DeleteAsync("/api/eventapplications/-1");

            Assert.True(response.StatusCode.Equals(HttpStatusCode.Forbidden), "Wrong Status Code. Expected: Forbidden. Received: " + response.StatusCode.ToString());

        }

    }



}