using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Xunit;
using Newtonsoft.Json;
using Nexpo.Models;
using System.Collections.Generic;
using Nexpo.DTO;
using System;

namespace Nexpo.Tests.Controllers
{
    public class EventsControllerTests
    {

        [Fact]
        public async Task GetAllAdmin()
        {
            var client = await TestUtils.Login("admin");
            var response = await client.GetAsync("/api/events/-1/tickets");
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), "Wrong status code. Expected: OK. Received: " + response.StatusCode.ToString());

            var responseList = JsonConvert.DeserializeObject<List<TicketInfoDTO>>(await response.Content.ReadAsStringAsync());
            Assert.True(responseList.Count == 3, "Wrong number of tickets. Expected: 3. Received: " + responseList.Count);

            var firstTicket = responseList.Find(r => r.ticket.Id == -1);
            var thirdTicket = responseList.Find(r => r.ticket.Id == -3);

            Assert.True(firstTicket.userFirstName.Equals("Alpha"), "Wrong ticket first name. Expected: Alpha. Received: " + firstTicket.userFirstName.ToString());
            Assert.True(firstTicket.userLastName.Equals("Student"), "Wrong ticket last name. Expected: Student. Received: " + firstTicket.userLastName.ToString());
            Assert.True(firstTicket.ticket.EventId == -1, "Wrong event id. Expected: -1. Received: " + firstTicket.ticket.EventId.ToString());
            Assert.True(firstTicket.ticket.UserId == -2, "Wrong userID. Expected: -2. Received: " + firstTicket.ticket.UserId.ToString());
            Assert.True(thirdTicket.ticket.UserId == -4, "Wrong userID, Expected: -4. Reciéved: " + thirdTicket.ticket.UserId.ToString());
        }

        [Fact]
        public async Task GetAllNotLoggedInSuccess()
        {
            var application = new WebApplicationFactory<Program>();
            var client = application.CreateClient();

            var response = await client.GetAsync("/api/events");
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), "Wrong Status Code. Expected: OK. Received: " + response.StatusCode.ToString());

            var responseList = JsonConvert.DeserializeObject<List<Event>>(await response.Content.ReadAsStringAsync());
            Assert.True(responseList.Count == 5, "Wrong number of events. Expected: 5. Received: " + responseList.Count);

            var firstEvent = responseList.Find(r => r.Id == -1);
            var secondEvent = responseList.Find(r => r.Id == -2);
            var thirdEvent = responseList.Find(r => r.Id == -3);
            var fourthEvent = responseList.Find(r => r.Id == -4);

            Assert.True(firstEvent.Description.Equals("Breakfast with SEB"), "Wrong Description. Expected: Breakfast with SEB. Received: " + firstEvent.Description);
            Assert.True(secondEvent.Date.Equals(DateTime.Now.AddDays(11).Date.ToString()), "Wrong Date. Expexted: " + DateTime.Now.AddDays(11).Date.ToString() + ". Received: " + secondEvent.Date);
            Assert.True(thirdEvent.Host.Equals("Randstad"), "Wrong Host. Expected: Ranstad. Received: " + thirdEvent.Host);
            Assert.True(fourthEvent.Capacity == 2, "Wrong capacity. Expected: 2. Received: " + fourthEvent.Capacity.ToString());
        }

        [Fact]
        public async Task GetAllNotLoggedInUnauthorized()
        {
            var application = new WebApplicationFactory<Program>();
            var client = application.CreateClient();

            var response = await client.GetAsync("/api/events/2/tickets");
            Assert.True(response.StatusCode.Equals(HttpStatusCode.Unauthorized), "Wrong Status Code. Expected: Unautherized. Received: " + response.StatusCode.ToString());
        }

        [Fact]
        public async Task GetAllAsCompRepForbidden()
        {
            var client = await TestUtils.Login("company1");

            var response = await client.GetAsync("/api/events/-1/tickets");
            Assert.True(response.StatusCode.Equals(HttpStatusCode.Forbidden), "Wrong Status Code. Expected: Forbidden. Received: " + response.StatusCode.ToString());
        }

        [Fact]
        public async Task GetNotLoggedInSuccess()
        {
            var application = new WebApplicationFactory<Program>();
            var client = application.CreateClient();

            var response = await client.GetAsync("/api/events/-3");
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), "Wrong Status Code. Expected: OK. Received: " + response.StatusCode.ToString());

            var responseObject = JsonConvert.DeserializeObject<Event>(await response.Content.ReadAsStringAsync());
            Assert.True(responseObject.Name.Equals("CV Workshop with Randstad"), "Wrong event name. Expected: CV Workshop with Randstad. Received: " + responseObject.Name);
            Assert.True(responseObject.Date.Equals(DateTime.Now.AddDays(12).Date.ToString()), "Wrong Date. Expected: " + DateTime.Now.AddDays(12).Date.ToString() + ". Received: " + responseObject.Date);
            Assert.True(responseObject.End.Equals("15:00"), "Wrong end time. Expected: 15:00. Received: " + responseObject.End);
            Assert.True(responseObject.Language.Equals("Swedish"), "Wrong Language. Expected: Swedish. Received: " + responseObject.Language);
        }

                [Fact]
        public async Task GetAdminWrongId()
        {
            var client = await TestUtils.Login("admin");
            var response = await client.GetAsync("/api/events/-123/tickets");

            Assert.True(response.StatusCode.Equals(HttpStatusCode.NotFound), "Wrong status code. Expected: NotFound. Received: " + response.StatusCode.ToString());
        }

        [Fact]
        public async Task GetNotLoggedInWithIncorrectId()
        {
            var application = new WebApplicationFactory<Program>();
            var client = application.CreateClient();

            var response = await client.GetAsync("/api/events/-123");
            Assert.True(response.StatusCode.Equals(HttpStatusCode.NotFound), "Wrong Status Code. Expected: NotFound. Received: " + response.StatusCode.ToString());
        }

        [Fact]
        public async Task PutEventSuccess()
        {
            var client = await TestUtils.Login("admin");
            var json = new JsonObject
            {
                { "description", "New description" },
                { "date", "2011-03-07" },
                { "end", "17:00" },
                { "language", "English" },
                { "capacity", 25 }
            };

            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await client.PutAsync("/api/events/-1", payload);
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), "Wrong status code. Expected: OK. Received: " + response.StatusCode.ToString());

            //Restore
            json = new JsonObject
            {
                { "description", "Breakfast with SEB" },
                { "date", "2022-11-12" },
                { "end", "10:00" },
                { "language", "Swedish" },
                { "capacity", 30 }
            };

            var response2 = await client.PutAsync("/api/events/-1", new StringContent(json.ToString(), Encoding.UTF8, "application/json"));
            Assert.True(response2.StatusCode.Equals(HttpStatusCode.OK), "Wrong status code. Expected: OK. Received: " + response2.StatusCode.ToString());

            //Verify
            var responseObject = JsonConvert.DeserializeObject<AddEventDTO>(await response.Content.ReadAsStringAsync());
            Assert.True(responseObject.Description.Equals("New description"), "Wrong description. Expected: New Description. Received: " + responseObject.Description);
            Assert.True(responseObject.Date.Equals("2011-03-07"), "Wrong date. Expected: 2011-03-07. Received: " + responseObject.Date);
            Assert.True(responseObject.End.Equals("17:00"), "Wrong end time. Expected: 17:00. Received: " + responseObject.End);
            Assert.True(responseObject.Language.Equals("English"), "Wrong language. Expected: English. Received: " + responseObject.Language);
            Assert.True(responseObject.Capacity == 25, "Wrong Capacity. Expected: 25. Received: " + responseObject.Capacity);
            Assert.True(responseObject.Name.Equals("Breakfast Mingle"), "Wrong name. Expected: Breakfast Mingle. Received: " + responseObject.Name);
            Assert.True(responseObject.Start.Equals("08:15"), "Wrong starttime. Expected: 08:15. Received: " + responseObject.Start);
        }

        [Fact]
        public async Task PutUnauthorized()
        {
            var client = await TestUtils.Login("company1");
            var json = new JsonObject
            {
                { "description", "None" }
            };

            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await client.PutAsync("/api/events/-1", payload);

            Assert.True(response.StatusCode.Equals(HttpStatusCode.Forbidden), "Wrong status code. Expected: Forbidden. Received: " + response.StatusCode.ToString());

            string responseText = await response.Content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject<AddEventDTO>(responseText);
            Assert.True(responseObject == null, "Returned Object was not null. Received: " + responseText);
        }

        [Fact]
        public async Task PutEmptySuccess()
        {
            var client = await TestUtils.Login("admin");
            var json = new JsonObject();

            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await client.PutAsync("/api/events/-4", payload);

            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), response.StatusCode.ToString());

            var responseObject = JsonConvert.DeserializeObject<AddEventDTO>(await response.Content.ReadAsStringAsync());
            Assert.True(responseObject.Description.Equals("Get inspired and expand your horizons"), $"Wrong description. Received: {responseObject.Description}");
            Assert.True(responseObject.Date.Equals(DateTime.Now.AddDays(14).Date.ToString()), $"Wrong date. Received: {responseObject.Date}");
            Assert.True(responseObject.End.Equals("13:00"), $"Wrong end time. Expected: 13:00. Received: {responseObject.End}");
            Assert.True(responseObject.Language.Equals("Swedish"), $"Wrong Language. Expected: Swedish. Received: {responseObject.Language}");
            Assert.True(responseObject.Capacity == 2, $"Wrong capacity. Expected: 2. Received: {responseObject.Capacity}");
            Assert.True(responseObject.Name.Equals("Inspirational lunch lecture"), $"Wrong name. Received: {responseObject.Name}");
            Assert.True(responseObject.Start.Equals("12:15"), $"Wrong start time. Expected: 12:15. Received: {responseObject.Start}");
            Assert.True(responseObject.Location.Equals("MA:3"), $"Wrong location. Expected: MA:3. Received: {responseObject.Location}");
        }
    }
}