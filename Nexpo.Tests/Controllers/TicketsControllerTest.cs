﻿using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json.Linq;
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
    public class TicketsControllerTest
    {
        [Fact]
        public async Task GetAllForStudent()
        {
            var client = await TestUtils.Login("student1");
            var response = await client.GetAsync("/api/tickets");

            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), "Wrong status code. Expected: OK. Received: " + response.StatusCode.ToString());

            var responseList = JsonConvert.DeserializeObject<List<Ticket>>(await response.Content.ReadAsStringAsync());

            var ticket1 = responseList.Find(ticket => ticket.Id == -1);
            var ticket2 = responseList.Find(ticket => ticket.Id == -4);

            Assert.True(responseList.Count == 3, "Wrong number of tickets. Expected: 3. Received: " + responseList.Count.ToString());
            Assert.True(ticket1.PhotoOk, "Wrong PhotoOk value. Expected: true. Received: " + ticket1.PhotoOk.ToString());
            Assert.True(!ticket2.PhotoOk, "Wrong PhotoOk value. Expected: false. Received: " + ticket2.PhotoOk.ToString());
        }


        /// <summary>
        
        [Fact]
        public async Task GetAllForCompanyWith0()
        {
            var client = await TestUtils.Login("company1");
            var response = await client.GetAsync("/api/tickets");

            var responseList = JsonConvert.DeserializeObject<List<Ticket>>(await response.Content.ReadAsStringAsync());

            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), "Wrong status code. Expected: OK. Received: " + response.StatusCode.ToString());
            Assert.True(responseList.Count == 0, "Wrong number of tickets. Expected: 0. Receivd: " + responseList.Count.ToString());
        }

        [Fact]
        public async Task GetAllTicketsNotLoggedIn()
        {
            var application = new WebApplicationFactory<Program>();
            var client = application.CreateClient();

            var response = await client.GetAsync("/api/tickets");

            // Verify response - Unauthorized because non logged in user does not have tickets
            Assert.True(response.StatusCode.Equals(HttpStatusCode.Unauthorized), "Wrong status code. Expected: Unauthorized. Received: " + response.StatusCode.ToString());
        }

        [Fact]
        public async Task getEventTypeOfTicketAsStudent(){
            var client = await TestUtils.Login("student1");
            var response = await client.GetAsync("/api/tickets/-11/type");
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), "Wrong status code. Expected: OK. Received: " + response.StatusCode.ToString());

            var responseObject = JsonConvert.DeserializeObject<EventType>(await response.Content.ReadAsStringAsync());
            Assert.True(responseObject == EventType.Lunch, "Wrong event type. Expected: Lunch. Received: " + responseObject.ToString());
        }

        [Fact]
        public async Task getEventTypeOfTicketAsCompanyRepresentative()
        {
            var client = await TestUtils.Login("company4");
            var response = await client.GetAsync("/api/tickets/-12/type");
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), "Wrong status code. Expected: OK. Received: " + response.StatusCode.ToString());

            var responseObject = JsonConvert.DeserializeObject<EventType>(await response.Content.ReadAsStringAsync());
            Assert.True(responseObject == EventType.Banquet, "Wrong event type. Expected: Banquet. Received: " + responseObject.ToString());
        }
        [Fact]
        public async Task getEventTypeOfTicketNotLoggedIn()
        {
            var application = new WebApplicationFactory<Program>();
            var client = application.CreateClient();
            var response = await client.GetAsync("/api/tickets/-1/type");

            Assert.True(response.StatusCode.Equals(HttpStatusCode.Unauthorized), "Wrong status code. Expected: Unauthorized. Received: " + response.StatusCode.ToString());
        }

        [Fact]
        public async Task getEventTypeOfTicketNotExisting()
        {
            var client = await TestUtils.Login("student1");
            var response = await client.GetAsync("/api/tickets/-123/type");

            Assert.True(response.StatusCode.Equals(HttpStatusCode.NotFound), "Wrong status code. Expected: NotFound. Received: " + response.StatusCode.ToString());
        }

        [Fact]
        public async Task getEventTypeOfTicketNotOwn()
        {
            var client = await TestUtils.Login("student2");
            var response = await client.GetAsync("/api/tickets/-11/type");

            Assert.True(response.StatusCode.Equals(HttpStatusCode.Forbidden), "Wrong status code. Expected: NotFound. Received: " + response.StatusCode.ToString());
        }


        [Fact]
        public async Task GetSpecificTicketLegitimate()
        {
            var client = await TestUtils.Login("student1");
            var response = await client.GetAsync("/api/tickets/id/-1");
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), "Wrong status code. Expected: OK. Received: " + response.StatusCode.ToString());

            var responseObject = JsonConvert.DeserializeObject<Ticket>(await response.Content.ReadAsStringAsync());
            Assert.True(responseObject.Id == -1, "Wrong ticket id. Expected: -1. Received: " + responseObject.Id.ToString());
            Assert.True(responseObject.PhotoOk, "Wrong PhotoOk value. Expected: true. Received: " + responseObject.PhotoOk.ToString());
            Assert.True(responseObject.EventId == -1, "Wrong event id. Expected: -1. Received: " + responseObject.EventId.ToString());
            Assert.True(responseObject.UserId == -2, "Wrong user id. Expected: -2. Received: " + responseObject.UserId.ToString());
        }

        [Fact]
        public async Task GetFromAnotherUser()
        {
            var client = await TestUtils.Login("company1");
            var response = await client.GetAsync("/api/tickets/id/-1");

            // Verify response - Not found because the company does not have access to the ticket
            Assert.True(response.StatusCode.Equals(HttpStatusCode.NotFound), "Wrong status code. Expected: NotFound. Received: " + response.StatusCode.ToString());
        }

        [Fact]
        public async Task GetAsAdmin()
        {
            var client = await TestUtils.Login("admin");
            var response = await client.GetAsync("/api/tickets/id/-1");

            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), "Wrong status code. Expected: OK. Received: " + response.StatusCode.ToString());

            var responseObject = JsonConvert.DeserializeObject<Ticket>(await response.Content.ReadAsStringAsync());

            Assert.True(responseObject.Id == -1, "Wrong ticket id. Expected: -1. Received: " + responseObject.Id.ToString());
            Assert.True(responseObject.PhotoOk, "Wrong PhotoOk value. Expected: true. Received: " + responseObject.PhotoOk.ToString());
            Assert.True(responseObject.EventId == -1, "Wrong event id. Expected: -1. Received: " + responseObject.EventId.ToString());
            Assert.True(responseObject.UserId == -2, "Wrong user id. Expected: -2. Received: " + responseObject.UserId.ToString());
        }

        [Fact]
        public async Task GetNotFound()
        {
            var client = await TestUtils.Login("admin");
            var response = await client.GetAsync("/api/tickets/id/-123");

            // Verify response - Not found because the ticket of id -123 does not exist in the database
            Assert.True(response.StatusCode.Equals(HttpStatusCode.NotFound), "Wrong status code. Expected: NotFound. Received: " + response.StatusCode.ToString());
        }


        [Fact]
        public async Task GetIdAndGuid()
        {
            var client = await TestUtils.Login("admin");
            var response = await client.GetAsync("/api/tickets/id/-1");

            var responseObject = JsonConvert.DeserializeObject<Ticket>(await response.Content.ReadAsStringAsync());
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), "Wrong status code. Expected: OK. Received: " + response.StatusCode.ToString());

            var response2 = await client.GetAsync("/api/tickets/" + responseObject.Code);
            var responseObject2 = JsonConvert.DeserializeObject<Ticket>(await response2.Content.ReadAsStringAsync());

            Assert.True(response2.StatusCode.Equals(HttpStatusCode.OK), "Wrong status code. Expected: OK. Received: " + response2.StatusCode.ToString());

            Assert.True(responseObject.Id == responseObject2.Id, "Mismatch in ticket id. Recieved: " + responseObject.Id.ToString() + " & " + responseObject2.ToString());
            Assert.True(responseObject.PhotoOk && responseObject2.PhotoOk, "Mismatch in PhotoOk value. Recieved: " + responseObject.PhotoOk.ToString() + " & " + responseObject2.PhotoOk.ToString());
            Assert.True(responseObject.EventId == responseObject2.EventId, "Mismatch in event id. Recieved: " + responseObject.EventId.ToString() + " & " + responseObject2.EventId.ToString());
            Assert.True(responseObject.UserId == responseObject2.UserId, "Mismatch in user id. Recieved: " + responseObject.UserId.ToString() + " & " + responseObject2.UserId.ToString());
        }

        [Fact]
        public async Task PostMultipleTicketsToSameEvent()
        {
            var client =  await TestUtils.Login("student1");
            var json = new JsonObject
            {
                { "eventid", -2 },
                { "photook", true }
            };

            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("api/tickets", payload);

            // Verify response - Conflict because the student already has a ticket for the event
            Assert.True(response.StatusCode.Equals(HttpStatusCode.Conflict), "Wrong status code. Expected: Conflict. Received: " + response.ToString());
        }

        [Fact]
        public async Task PostNotFoundEvent()
        {
            var client = await TestUtils.Login("student1");
            var json = new JsonObject
            {
                { "eventid", -123 },
                { "photook", true }
            };
            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("api/tickets", payload);

            // Verify response - Not found because the event of id -123 does not exist in the database
            Assert.True(response.StatusCode.Equals(HttpStatusCode.NotFound), "Wrong status code. Expected: NotFound. Received: " + response.ToString());
        }

        [Fact]
        public async Task PostToFullEvent()
        {
            var client = await TestUtils.Login("student1");
            var json = new JsonObject
            {
                { "eventid", -4 },
                { "photook", true }
            };

            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("api/tickets", payload);

            // Verify response - Conflict because the event is full
            Assert.True(response.StatusCode.Equals(HttpStatusCode.Conflict), "Wrong status code. Expected: Conflict. Received: " + response.ToString());
        }

        [Fact]
        public async Task PostTicketUnauthorized()
        {
            var application = new WebApplicationFactory<Program>();
            var client = application.CreateClient();

            var json = new JsonObject
            {
                { "eventid", -1 },
                { "photook", true }
            };

            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("api/tickets", payload);

            // Verify response - Unauthorized because the user is not logged in
            Assert.True(response.StatusCode.Equals(HttpStatusCode.Unauthorized), "Wrong status code. Expected: Unauthorized. Received: " + response.ToString());
        }

        [Fact]
        public async Task PostLessThanTwoDaysBeforeEvent()
        {
            var client = await TestUtils.Login("student1");
            var json = new JsonObject
            {
                { "eventid", -5 },
                { "photook", true }
            };
            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("api/tickets", payload);

            // Verify response - Conflict because the event is too close in time
            Assert.True(response.StatusCode.Equals(HttpStatusCode.BadRequest), "Wrong status code. Expected: BadRequest. Received: " + response.ToString());
        }

        [Fact]
        public async Task PostandDelete()
        {
            var client =  await TestUtils.Login("student1");

            //Add new ticket
            var json = new JsonObject
            {
                { "eventid", -3 },
                { "photook", true }
            };

            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("api/tickets", payload);
            Assert.True(response.StatusCode.Equals(HttpStatusCode.Created), "Wrong status code. Expected: Created. Received: " + response.ToString());
            
            //Check response and db updated
            string responseText = await response.Content.ReadAsStringAsync();
            var parsedContent = JObject.Parse(responseText);
            var newTicketId = parsedContent.Value<string>("id");

            var response2 = await client.GetAsync("/api/tickets/id/" + newTicketId);
            Assert.True(response2.StatusCode.Equals(HttpStatusCode.OK), "Wrong status code. Expected: OK. Received: " + response2.StatusCode.ToString());
            
            var response3 = await client.GetAsync("/api/tickets");
            Assert.True(response3.StatusCode.Equals(HttpStatusCode.OK), "Wrong status code. Expected: OK. Received: " + response3.StatusCode.ToString());

            //Delete created ticket
            var response4 = await client.DeleteAsync("api/tickets/" + newTicketId);
            Assert.True(response4.StatusCode.Equals(HttpStatusCode.NoContent), "Wrong status code. Expected: NoContent. Received: " + response4.StatusCode.ToString());

            //Check ticket does not exist and users total nbr. of tickets
            var response5 = await client.GetAsync("/api/tickets");
            Assert.True(response5.StatusCode.Equals(HttpStatusCode.OK), "Wrong status code. Expected: OK. Received: " + response5.StatusCode.ToString());

            var response6 = await client.GetAsync("/api/tickets/id/" + newTicketId);
            Assert.True(response6.StatusCode.Equals(HttpStatusCode.NotFound), "Wrong status code. Expected: NotFound. Received: " + response6.StatusCode.ToString());

            //Verify
            var responseObject = JsonConvert.DeserializeObject<Ticket>(await response2.Content.ReadAsStringAsync());
            Assert.True(responseObject.PhotoOk, "Wrong PhotoOk value. Expected: true. Received: " + responseObject.PhotoOk.ToString());
            Assert.True(responseObject.EventId == -3, "Wrong event id. Expected: -3. Received: " + responseObject.EventId.ToString());
            Assert.True(responseObject.UserId == -2, "Wrong user id. Expected: -2. Received: " + responseObject.UserId.ToString());

            var responseList = JsonConvert.DeserializeObject<List<Ticket>>((await response3.Content.ReadAsStringAsync()));
            Assert.True(responseList.Count == 4, "Wrong number of tickets. Expected: 3. Received: " + responseList.Count.ToString());

            var responseList2 = JsonConvert.DeserializeObject<List<Ticket>>((await response5.Content.ReadAsStringAsync()));
            Assert.True(responseList2.Count == 3, "Wrong number of tickets. Expected: 3. Received: " + responseList2.Count.ToString());
        }

        [Fact]
        public async Task PostandDeleteTakeaway()
        {
            var client =  await TestUtils.Login("student1");
            var time = DateTime.Now.AddDays(1);
            //Add new ticket
            var json = new JsonObject
            {
                { "eventid", -7 },
                { "photook", true },
                { "takeaway", true },
                { "takeawaytime", time }
            };
            
            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("api/tickets", payload);
            Assert.True(response.StatusCode.Equals(HttpStatusCode.Created), "Wrong status code. Expected: Created. Received: " + response.ToString());
            
            //Check response and db updated
            string responseText = await response.Content.ReadAsStringAsync();
            var parsedContent = JObject.Parse(responseText);
            var newTicketId = parsedContent.Value<string>("id");

            var response2 = await client.GetAsync("/api/tickets/id/" + newTicketId);
            Assert.True(response2.StatusCode.Equals(HttpStatusCode.OK), "Wrong status code. Expected: OK. Received: " + response2.StatusCode.ToString());
            
            var response3 = await client.GetAsync("/api/tickets");
            Assert.True(response3.StatusCode.Equals(HttpStatusCode.OK), "Wrong status code. Expected: OK. Received: " + response3.StatusCode.ToString());

            //Delete created ticket
            var response4 = await client.DeleteAsync("api/tickets/" + newTicketId);
            Assert.True(response4.StatusCode.Equals(HttpStatusCode.NoContent), "Wrong status code. Expected: NoContent. Received: " + response4.StatusCode.ToString());

            //Check ticket does not exist and users total nbr. of tickets
            var response5 = await client.GetAsync("/api/tickets");
            Assert.True(response5.StatusCode.Equals(HttpStatusCode.OK), "Wrong status code. Expected: OK. Received: " + response5.StatusCode.ToString());

            var response6 = await client.GetAsync("/api/tickets/id/" + newTicketId);
            Assert.True(response6.StatusCode.Equals(HttpStatusCode.NotFound), "Wrong status code. Expected: NotFound. Received: " + response6.StatusCode.ToString());

            //Verify
            var responseObject = JsonConvert.DeserializeObject<Ticket>(await response2.Content.ReadAsStringAsync());
            Assert.True(responseObject.PhotoOk, "Wrong PhotoOk value. Expected: true. Received: " + responseObject.PhotoOk.ToString());
            Assert.True(responseObject.EventId == -7, "Wrong event id. Expected: -7. Received: " + responseObject.EventId.ToString());
            Assert.True(responseObject.UserId == -2, "Wrong user id. Expected: -2. Received: " + responseObject.UserId.ToString());
            Assert.True(responseObject.TakeAway, "Wrong TakeAway value. Expected: true. Received: " + responseObject.TakeAway.ToString());
            Assert.True(responseObject.TakeAwayTime.Day == time.Day, "Wrong TakeAwayTime value. Expected: " + time.ToString() + ". Received: " + responseObject.TakeAwayTime.ToString());


            var responseList = JsonConvert.DeserializeObject<List<Ticket>>((await response3.Content.ReadAsStringAsync()));
            Assert.True(responseList.Count == 4, "Wrong number of tickets. Expected: 4. Received: " + responseList.Count.ToString());

            var responseList2 = JsonConvert.DeserializeObject<List<Ticket>>((await response5.Content.ReadAsStringAsync()));
            Assert.True(responseList2.Count == 3, "Wrong number of tickets. Expected: 3. Received: " + responseList2.Count.ToString());
        }

        [Fact]
        public async Task PostandDeleteTakeawayToNonLunch()
        {
            var client =  await TestUtils.Login("student1");

            //Add new ticket
            var json = new JsonObject
            {
                { "eventid", -3 },
                { "photook", true },
                { "takeaway", true },
                { "takeawaytime", DateTime.Now.AddDays(1) }
            };
            
            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("api/tickets", payload);
            Assert.True(response.StatusCode.Equals(HttpStatusCode.BadRequest), "Wrong status code. Expected: BadRequest. Received: " + response.ToString());

        }

        [Fact]
        public async Task PostandDeleteTakeawayWithoutTime()
        {
            var client =  await TestUtils.Login("student1");

            //Add new ticket
            var json = new JsonObject
            {
                { "eventid", -3 },
                { "photook", true },
                { "takeaway", true },
            };
            
            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("api/tickets", payload);
            Assert.True(response.StatusCode.Equals(HttpStatusCode.BadRequest), "Wrong status code. Expected: BadRequest. Received: " + response.ToString());
        }

        [Fact]
        public async Task PostandDeleteAsAdminCloseToEvent()
        {
            var client = await TestUtils.Login("admin");

            //Add new ticket
            var json = new JsonObject
            {
                { "eventid", -5 },
                { "photook", true },
                { "userid", -4 }
            };

            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("api/tickets/add", payload);

            Assert.True(response.StatusCode.Equals(HttpStatusCode.Created), "Wrong status code. Expected: Created. Received: " + response.ToString());

            //Check response and db updated
            string responseText = await response.Content.ReadAsStringAsync();
            var parsedContent = JObject.Parse(responseText);
            var newTicketId = parsedContent.Value<string>("id");

            var response2 = await client.GetAsync("/api/tickets/id/" + newTicketId);
            Assert.True(response2.StatusCode.Equals(HttpStatusCode.OK), "Wrong status code. Expected: OK. Received: " + response2.StatusCode.ToString());
            
            //Delete created ticket
            var response4 = await client.DeleteAsync("api/tickets/" + newTicketId);
            Assert.True(response4.StatusCode.Equals(HttpStatusCode.NoContent), "Wrong status code. Expected: NoContent. Received: " + response4.StatusCode.ToString());

            //Check ticket does not exist
            var response6 = await client.GetAsync("/api/tickets/id/" + newTicketId);
            Assert.True(response6.StatusCode.Equals(HttpStatusCode.NotFound), "Wrong status code. Expected: NotFound. Received: " + response6.StatusCode.ToString());

            //Verify
            var responseObject = JsonConvert.DeserializeObject<Ticket>(await response2.Content.ReadAsStringAsync());
            Assert.True(responseObject.PhotoOk, "Wrong PhotoOk value. Expected: true. Received: " + responseObject.PhotoOk.ToString());
            Assert.True(responseObject.EventId == -5, "Wrong event id. Expected: -5. Received: " + responseObject.EventId.ToString());
            Assert.True(responseObject.UserId == -4, "Wrong user id. Expected: -4. Received: " + responseObject.UserId.ToString());
        }

        [Fact]
        public async Task PostandDeleteTakeawayAsAdminCloseToEvent()
        {
            var client = await TestUtils.Login("admin");
            var time = DateTime.Now.AddDays(1);
            //Add new ticket
            var json = new JsonObject
            {
                { "eventid", -7 },
                { "photook", true },
                { "userid", -4 },
                { "takeaway", true },
                { "takeawaytime", time }
            };

            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("api/tickets/add", payload);

            Assert.True(response.StatusCode.Equals(HttpStatusCode.Created), "Wrong status code. Expected: Created. Received: " + response.ToString());

            //Check response and db updated
            string responseText = await response.Content.ReadAsStringAsync();
            var parsedContent = JObject.Parse(responseText);
            var newTicketId = parsedContent.Value<string>("id");

            var response2 = await client.GetAsync("/api/tickets/id/" + newTicketId);
            Assert.True(response2.StatusCode.Equals(HttpStatusCode.OK), "Wrong status code. Expected: OK. Received: " + response2.StatusCode.ToString());
            
            //Delete created ticket
            var response4 = await client.DeleteAsync("api/tickets/" + newTicketId);
            Assert.True(response4.StatusCode.Equals(HttpStatusCode.NoContent), "Wrong status code. Expected: NoContent. Received: " + response4.StatusCode.ToString());

            //Check ticket does not exist
            var response6 = await client.GetAsync("/api/tickets/id/" + newTicketId);
            Assert.True(response6.StatusCode.Equals(HttpStatusCode.NotFound), "Wrong status code. Expected: NotFound. Received: " + response6.StatusCode.ToString());

            //Verify
            var responseObject = JsonConvert.DeserializeObject<Ticket>(await response2.Content.ReadAsStringAsync());
            Assert.True(responseObject.PhotoOk, "Wrong PhotoOk value. Expected: true. Received: " + responseObject.PhotoOk.ToString());
            Assert.True(responseObject.EventId == -7, "Wrong event id. Expected: -7. Received: " + responseObject.EventId.ToString());
            Assert.True(responseObject.UserId == -4, "Wrong user id. Expected: -4. Received: " + responseObject.UserId.ToString());
            Assert.True(responseObject.TakeAway, "Wrong TakeAway value. Expected: true. Received: " + responseObject.TakeAway.ToString());
            Assert.True(responseObject.TakeAwayTime.Day == time.Day, "Wrong TakeAwayTime value. Expected: " + DateTime.Now.AddDays(1).ToString() + ". Received: " + responseObject.TakeAwayTime.ToString());
        }


        [Fact]
        public async Task DeleteConsumedTicketAsAdmin()
        {
            var client = await TestUtils.Login("admin");

            //Add new ticket
            var json = new JsonObject
            {
                { "eventid", -2 },
                { "photook", true },
                { "userid", -3 }
            };

            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("api/tickets/add", payload);

            Assert.True(response.StatusCode.Equals(HttpStatusCode.Created), "Wrong status code. Expected: Created. Received: " + response.ToString());

            //Set consumed & check response
            string responseText = await response.Content.ReadAsStringAsync();
            var parsedContent = JObject.Parse(responseText);
            var newTicketId = parsedContent.Value<string>("id");

            json = new JsonObject
            {
                { "isConsumed", true }
            };

            payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response2 = await client.PutAsync("api/tickets/" + newTicketId, payload);

            Assert.True(response2.StatusCode.Equals(HttpStatusCode.OK), "Wrong status code. Expected: OK. Received: " + response2.StatusCode.ToString());

            //Delete created ticket
            var response3 = await client.DeleteAsync("api/tickets/" + newTicketId);
            Assert.True(response3.StatusCode.Equals(HttpStatusCode.NoContent), "Wrong status code. Expected: NoContent. Received: " + response3.StatusCode.ToString());

            //Check ticket does not exist
            var response4 = await client.GetAsync("/api/tickets/id/" + newTicketId);
            Assert.True(response4.StatusCode.Equals(HttpStatusCode.NotFound), "Wrong status code. Expected: NotFound. Received: " + response4.StatusCode.ToString());

            //Verify
            var responseObject = JsonConvert.DeserializeObject<Ticket>(await response.Content.ReadAsStringAsync());
            Assert.True(responseObject.PhotoOk, "Wrong PhotoOk value. Expected: true. Received: " + responseObject.PhotoOk.ToString());
            Assert.True(responseObject.EventId == -2, "Wrong event id. Expected: -2. Received: " + responseObject.EventId.ToString());
            Assert.True(responseObject.UserId == -3, "Wrong user id. Expected: -3. Received: " + responseObject.UserId.ToString());
            Assert.True(!responseObject.isConsumed, "Wrong isConsumed value. Expected: false. Received: " + responseObject.isConsumed.ToString());

        }

        [Fact]
        public async Task DeleteCloseToEvent()
        {
            var client = await TestUtils.Login("student2");
            var response = await client.DeleteAsync("api/tickets/-8");

            // Verify response - Bad request because the event is too close in time
            Assert.True(response.StatusCode.Equals(HttpStatusCode.BadRequest), "Wrong status code. Expected: BadRequest. Received: " + response.ToString());
        }

        [Fact]
        public async Task DeleteAnotherUsersTicket()
        {
            var client = await TestUtils.Login("student1");
            var response = await client.DeleteAsync("api/tickets/-2");

            // Verify response - Bad request because the user is not allowed to delete another users ticket
            Assert.True(response.StatusCode.Equals(HttpStatusCode.BadRequest), "Wrong status code. Expected: BadRequest. Received: " + response.ToString());
        }

        [Fact]
        public async Task DeleteNonExisting()
        {
            var client = await TestUtils.Login("student1");
            var response = await client.DeleteAsync("api/tickets/-123");

            // Verify response - Not found because the ticket of id -123 does not exist in the database
            Assert.True(response.StatusCode.Equals(HttpStatusCode.NotFound), "Wrong status code. Expected: NotFound. Received: " + response.ToString());
        }

        [Fact]
        public async Task PostTicketWrongEventIdAsAdmin()
        {
            var client = await TestUtils.Login("admin");

            //Add new ticket
            var json = new JsonObject
            {
                { "eventid", -123 },
                { "photook", true },
                { "userid", -4 }
            };

            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("api/tickets/add", payload);

            // Verify response - Not Found because event of id -123 does not exist in database
            Assert.True(response.StatusCode.Equals(HttpStatusCode.NotFound), "Wrong status code. Expected: NotFound. Received: " + response.ToString());
        }

        [Fact]
        public async Task PostDuplicateTicketToUserAsAdmin()
        {
            var client = await TestUtils.Login("admin");

            //Add new ticket
            var json = new JsonObject
            {
                { "eventid", -1 },
                { "photook", true },
                { "userid", -4 }
            };

            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("api/tickets/add", payload);

            // Verify response - Conflict because user -4 already has a ticket for event -1
            Assert.True(response.StatusCode.Equals(HttpStatusCode.Conflict), "Wrong status code. Expected: Conflict. Received: " + response.ToString());
        }

        [Fact]
        public async Task PostWrongPathAsStudent()
        {
            var client = await TestUtils.Login("student2");

            //Add new ticket
            var json = new JsonObject
            {
                { "eventid", -2 },
                { "photook", true },
                { "userid", -3 }
            };
            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("api/tickets/add", payload);

            Assert.True(response.StatusCode.Equals(HttpStatusCode.Forbidden), "Wrong status code. Expected: Forbidden. Received: " + response.ToString());
        }

        [Fact]
        public async Task DeleteConsumedExistingTicket()
        {
            var client =  await TestUtils.Login("student1");
            var response = await client.DeleteAsync("api/tickets/-1");

            // Verify response - Forbidden because ticket is consumed and user can not delete consumed tickets
            Assert.True(response.StatusCode.Equals(HttpStatusCode.BadRequest), "Wrong status code. Expected: BadRequest. Received: " + response.ToString());
        }

        [Fact]
        public async Task UpdateIsConsumedAsAdmin()
        {
            var client =  await TestUtils.Login("admin");
            var json = new JsonObject
            {
                { "isConsumed", true }
            };

            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await client.PutAsync("api/tickets/-2", payload);
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), "Wrong status code. Expected: OK. Received: " + response.StatusCode.ToString());

            //Restore
            var json2 = new JsonObject
            {
                { "isConsumed", false }
            };

            var payload2 = new StringContent(json2.ToString(), Encoding.UTF8, "application/json");
            var response2 = await client.PutAsync("api/tickets/-2", payload2);
            Assert.True(response2.StatusCode.Equals(HttpStatusCode.OK), "Wrong status code. Expected: OK. Received: " + response2.StatusCode.ToString());

            //Verify
            var responseObject = JsonConvert.DeserializeObject<Ticket>(await response.Content.ReadAsStringAsync());
            Assert.True(responseObject.isConsumed, "Wrong isConsumed value. Expected: true. Received: " + responseObject.isConsumed.ToString());
            
            var responseObject2 = JsonConvert.DeserializeObject<Ticket>(await response2.Content.ReadAsStringAsync());
            Assert.True(!responseObject2.isConsumed, "Wrong isConsumed value. Expected: false. Received: " + responseObject2.isConsumed.ToString());
        }  


        [Fact]
        public async Task UpdateTakeAwayAsStudent()
        {
            var client =  await TestUtils.Login("student1");

            var responseOld = await client.GetAsync("/api/tickets/id/-1");
            Assert.True(responseOld.StatusCode.Equals(HttpStatusCode.OK), "Wrong status code. Expected: OK. Received: " + responseOld.StatusCode.ToString());
            var responseObject = JsonConvert.DeserializeObject<Ticket>(await responseOld.Content.ReadAsStringAsync());

            // Save current values
            var oldIsConsumed = responseObject.isConsumed;      // True
            var oldTakeAway = responseObject.TakeAway;          // False
            var oldTakeAwayTime = responseObject.TakeAwayTime;  // 1/1/0001 12:00:00 AM 
            var oldId = responseObject.Id;                      // -1
            var oldPhotoOk = responseObject.PhotoOk;            // True
            var oldEventId = responseObject.EventId;            // -1
            var oldUserId = responseObject.UserId;              // -2 


            // Try to update TakeAway to true without TakeAwayTime
            var updateTicketDTO = new UpdateTicketDTO
            {
                isConsumed = true,
                TakeAway = true,
            };

            var json = JsonConvert.SerializeObject(updateTicketDTO);
            var payload = new StringContent(json, UnicodeEncoding.UTF8, "application/json");
            var response = await client.PutAsync("api/tickets/-1", payload);

            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK),"Wrong status code. Expected: OK. Received: " + response.StatusCode.ToString());

            var serializedUser = await response.Content.ReadAsStringAsync();
            var ticket = JsonConvert.DeserializeObject<Ticket>(serializedUser);
        
            // Verify that TakeAway hasn't been changed
            Assert.True(ticket.TakeAway.Equals(false), "Wrong takeAway status. Expected: False. Received: "+ ticket.TakeAway.ToString());
            Assert.True(ticket.TakeAwayTime.Equals(default(DateTime)), "Wrong takeAwayTime. Expected:  "+ default(DateTime) + "Received: " + ticket.TakeAwayTime.ToString());

            // Verify that nothing else has been changed
            Assert.True(oldIsConsumed == ticket.isConsumed, "Wrong isConsumed. Expected: " + oldIsConsumed.ToString() + " Received: " + ticket.isConsumed.ToString());
            Assert.True(oldId == ticket.Id, "Wrong Id. Expected: " + oldId.ToString() + " Received: " + ticket.Id.ToString());
            Assert.True(oldPhotoOk == ticket.PhotoOk, "Wrong PhotoOk. Expected: " + oldPhotoOk.ToString() + " Received: " + ticket.PhotoOk.ToString());
            Assert.True(oldEventId == ticket.EventId, "Wrong EventId. Expected: " + oldEventId.ToString() + " Received: " + ticket.EventId.ToString());
            Assert.True(oldUserId == ticket.UserId, "Wrong UserId. Expected: " + oldUserId.ToString() + " Received: " + ticket.UserId.ToString());


            // Try to update TakeAway and TakeAwayTime
            updateTicketDTO = new UpdateTicketDTO
            {
                isConsumed = true,
                TakeAway = true,
                TakeAwayTime = DateTime.Parse("2023-12-12 12:00")
            };

            json = JsonConvert.SerializeObject(updateTicketDTO);
            payload = new StringContent(json, UnicodeEncoding.UTF8, "application/json");
            response = await client.PutAsync("api/tickets/-1", payload);

            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK),"Wrong status code. Expected: OK. Received: " + response.StatusCode.ToString());

            serializedUser = await response.Content.ReadAsStringAsync();
            ticket = JsonConvert.DeserializeObject<Ticket>(serializedUser);
        
            // Verify change of TakeAway and TakeAwayTime
            Assert.True(ticket.TakeAway.Equals(true), "Wrong takeAway status. Expected: True. Received: "+ ticket.TakeAway.ToString());
            Assert.True(ticket.TakeAwayTime.Equals(DateTime.Parse("2023-12-12 12:00")), "Wrong takeAwayTime. Expected: 2023-12-12 12:00 . Received: " + ticket.TakeAwayTime.ToString());

            // Verify that nothing else has been changed
            Assert.True(oldIsConsumed == ticket.isConsumed, "Wrong isConsumed. Expected: " + oldIsConsumed.ToString() + " Received: " + ticket.isConsumed.ToString());
            Assert.True(oldId == ticket.Id, "Wrong Id. Expected: " + oldId.ToString() + " Received: " + ticket.Id.ToString());
            Assert.True(oldPhotoOk == ticket.PhotoOk, "Wrong PhotoOk. Expected: " + oldPhotoOk.ToString() + " Received: " + ticket.PhotoOk.ToString());
            Assert.True(oldEventId == ticket.EventId, "Wrong EventId. Expected: " + oldEventId.ToString() + " Received: " + ticket.EventId.ToString());
            Assert.True(oldUserId == ticket.UserId, "Wrong UserId. Expected: " + oldUserId.ToString() + " Received: " + ticket.UserId.ToString());



            // Try to update TakeAway to false
            updateTicketDTO = new UpdateTicketDTO
            {
                isConsumed = false,
                TakeAway = false,
                TakeAwayTime = DateTime.Parse("2022-10-10 10:00")
            };

            json = JsonConvert.SerializeObject(updateTicketDTO);
            payload = new StringContent(json, UnicodeEncoding.UTF8, "application/json");
            response = await client.PutAsync("api/tickets/-1", payload);

            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK),"Wrong status code. Expected: OK. Received: " + response.StatusCode.ToString());

            serializedUser = await response.Content.ReadAsStringAsync();
            ticket = JsonConvert.DeserializeObject<Ticket>(serializedUser);

            // Verify change of TakeAway and that TakeAwayTime has been set to default
            Assert.True(ticket.TakeAway.Equals(false), "Wrong takeAway status. Expected: False. Received: "+ ticket.TakeAway.ToString());
            Assert.True(ticket.TakeAwayTime.Equals(default(DateTime)), "Wrong takeAwayTime. Expected:  "+ default(DateTime) + "Received: " + ticket.TakeAwayTime.ToString());

            // Verify that nothing else has been changed
            Assert.True(oldIsConsumed == ticket.isConsumed, "Wrong isConsumed. Expected: " + oldIsConsumed.ToString() + " Received: " + ticket.isConsumed.ToString());
            Assert.True(oldId == ticket.Id, "Wrong Id. Expected: " + oldId.ToString() + " Received: " + ticket.Id.ToString());
            Assert.True(oldPhotoOk == ticket.PhotoOk, "Wrong PhotoOk. Expected: " + oldPhotoOk.ToString() + " Received: " + ticket.PhotoOk.ToString());
            Assert.True(oldEventId == ticket.EventId, "Wrong EventId. Expected: " + oldEventId.ToString() + " Received: " + ticket.EventId.ToString());
            Assert.True(oldUserId == ticket.UserId, "Wrong UserId. Expected: " + oldUserId.ToString() + " Received: " + ticket.UserId.ToString());


            // Reset to original values
            updateTicketDTO = new UpdateTicketDTO
            {
                isConsumed = oldIsConsumed,
                TakeAway = oldTakeAway,
                TakeAwayTime = oldTakeAwayTime
            };

            json = JsonConvert.SerializeObject(updateTicketDTO);
            payload = new StringContent(json, UnicodeEncoding.UTF8, "application/json");
            response = await client.PutAsync("api/tickets/-1", payload);

            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK),"Wrong status code. Expected: OK. Received: " + response.StatusCode.ToString());

            serializedUser = await response.Content.ReadAsStringAsync();
            ticket = JsonConvert.DeserializeObject<Ticket>(serializedUser);

            // Verify
            Assert.True(oldIsConsumed == ticket.isConsumed, "Wrong isConsumed. Expected: " + oldIsConsumed.ToString() + " Received: " + ticket.isConsumed.ToString());
            Assert.True(oldTakeAway == ticket.TakeAway, "Wrong TakeAway. Expected: " + oldTakeAway.ToString() + " Received: " + ticket.TakeAway.ToString());
            Assert.True(oldTakeAwayTime == ticket.TakeAwayTime, "Wrong TakeAwayTime. Expected: " + oldTakeAwayTime.ToString() + " Received: " + ticket.TakeAwayTime.ToString());
            Assert.True(oldId == ticket.Id, "Wrong Id. Expected: " + oldId.ToString() + " Received: " + ticket.Id.ToString());
            Assert.True(oldPhotoOk == ticket.PhotoOk, "Wrong PhotoOk. Expected: " + oldPhotoOk.ToString() + " Received: " + ticket.PhotoOk.ToString());
            Assert.True(oldEventId == ticket.EventId, "Wrong EventId. Expected: " + oldEventId.ToString() + " Received: " + ticket.EventId.ToString());
            Assert.True(oldUserId == ticket.UserId, "Wrong UserId. Expected: " + oldUserId.ToString() + " Received: " + ticket.UserId.ToString());
        } 

        [Fact]
        public async Task UpdateTakeAwayAsAdmin()
        {
            var client =  await TestUtils.Login("admin");

            var responseOld = await client.GetAsync("/api/tickets/id/-1");
            Assert.True(responseOld.StatusCode.Equals(HttpStatusCode.OK), "Wrong status code. Expected: OK. Received: " + responseOld.StatusCode.ToString());
            var responseObject = JsonConvert.DeserializeObject<Ticket>(await responseOld.Content.ReadAsStringAsync());

            // Save current values
            var oldIsConsumed = responseObject.isConsumed;      // False
            var oldTakeAway = responseObject.TakeAway;          // False
            var oldTakeAwayTime = responseObject.TakeAwayTime;  // 1/1/0001 12:00:00 AM 
            var oldId = responseObject.Id;                      // -1
            var oldPhotoOk = responseObject.PhotoOk;            // True
            var oldEventId = responseObject.EventId;            // -1
            var oldUserId = responseObject.UserId;              // -2 

            // Try to update TakeAway to true without TakeAwayTime
            var updateTicketDTO = new UpdateTicketDTO
            {
                isConsumed = oldIsConsumed,
                TakeAway = true,
            };

            var json = JsonConvert.SerializeObject(updateTicketDTO);
            var payload = new StringContent(json, UnicodeEncoding.UTF8, "application/json");
            var response = await client.PutAsync("api/tickets/-1", payload);

            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK),"Wrong status code. Expected: OK. Received: " + response.StatusCode.ToString());

            var serializedUser = await response.Content.ReadAsStringAsync();
            var ticket = JsonConvert.DeserializeObject<Ticket>(serializedUser);
        
            // Verify that TakeAway hasn't been changed
            Assert.True(ticket.TakeAway.Equals(false), "Wrong takeAway status. Expected: False. Received: "+ ticket.TakeAway.ToString());
            Assert.True(ticket.TakeAwayTime.Equals(default(DateTime)), "Wrong takeAwayTime. Expected:  "+ default(DateTime) + "Received: " + ticket.TakeAwayTime.ToString());

            // Verify that nothing else has been changed
            Assert.True(oldIsConsumed == ticket.isConsumed, "Wrong isConsumed. Expected: " + oldIsConsumed.ToString() + " Received: " + ticket.isConsumed.ToString());
            Assert.True(oldId == ticket.Id, "Wrong Id. Expected: " + oldId.ToString() + " Received: " + ticket.Id.ToString());
            Assert.True(oldPhotoOk == ticket.PhotoOk, "Wrong PhotoOk. Expected: " + oldPhotoOk.ToString() + " Received: " + ticket.PhotoOk.ToString());
            Assert.True(oldEventId == ticket.EventId, "Wrong EventId. Expected: " + oldEventId.ToString() + " Received: " + ticket.EventId.ToString());
            Assert.True(oldUserId == ticket.UserId, "Wrong UserId. Expected: " + oldUserId.ToString() + " Received: " + ticket.UserId.ToString());



            // Try to update TakeAway and TakeAwayTime
            updateTicketDTO = new UpdateTicketDTO
            {
                isConsumed = oldIsConsumed,
                TakeAway = true,
                TakeAwayTime = DateTime.Parse("2023-12-12 12:00")
            };

            json = JsonConvert.SerializeObject(updateTicketDTO);
            payload = new StringContent(json, UnicodeEncoding.UTF8, "application/json");
            response = await client.PutAsync("api/tickets/-1", payload);

            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK),"Wrong status code. Expected: OK. Received: " + response.StatusCode.ToString());

            serializedUser = await response.Content.ReadAsStringAsync();
            ticket = JsonConvert.DeserializeObject<Ticket>(serializedUser);
        
            // Verify change of TakeAway and TakeAwayTime
            Assert.True(ticket.TakeAway.Equals(true), "Wrong takeAway status. Expected: True. Received: "+ ticket.TakeAway.ToString());
            Assert.True(ticket.TakeAwayTime.Equals(DateTime.Parse("2023-12-12 12:00")), "Wrong takeAwayTime. Expected: 2023-12-12 12:00 . Received: " + ticket.TakeAwayTime.ToString());

            // Verify that nothing else has been changed
            Assert.True(oldIsConsumed == ticket.isConsumed, "Wrong isConsumed. Expected: " + oldIsConsumed.ToString() + " Received: " + ticket.isConsumed.ToString());
            Assert.True(oldId == ticket.Id, "Wrong Id. Expected: " + oldId.ToString() + " Received: " + ticket.Id.ToString());
            Assert.True(oldPhotoOk == ticket.PhotoOk, "Wrong PhotoOk. Expected: " + oldPhotoOk.ToString() + " Received: " + ticket.PhotoOk.ToString());
            Assert.True(oldEventId == ticket.EventId, "Wrong EventId. Expected: " + oldEventId.ToString() + " Received: " + ticket.EventId.ToString());
            Assert.True(oldUserId == ticket.UserId, "Wrong UserId. Expected: " + oldUserId.ToString() + " Received: " + ticket.UserId.ToString());


            // Try to update TakeAway to false
            updateTicketDTO = new UpdateTicketDTO
            {
                isConsumed = oldIsConsumed,
                TakeAway = false,
                TakeAwayTime = DateTime.Parse("2022-10-10 10:00")
            };

            json = JsonConvert.SerializeObject(updateTicketDTO);
            payload = new StringContent(json, UnicodeEncoding.UTF8, "application/json");
            response = await client.PutAsync("api/tickets/-1", payload);

            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK),"Wrong status code. Expected: OK. Received: " + response.StatusCode.ToString());

            serializedUser = await response.Content.ReadAsStringAsync();
            ticket = JsonConvert.DeserializeObject<Ticket>(serializedUser);

            // Verify change of TakeAway and that TakeAwayTime has been set to default
            Assert.True(ticket.TakeAway.Equals(false), "Wrong takeAway status. Expected: False. Received: "+ ticket.TakeAway.ToString());
            Assert.True(ticket.TakeAwayTime.Equals(default(DateTime)), "Wrong takeAwayTime. Expected:  "+ default(DateTime) + "Received: " + ticket.TakeAwayTime.ToString());

            // Verify that nothing else has been changed
            Assert.True(oldIsConsumed == ticket.isConsumed, "Wrong isConsumed. Expected: " + oldIsConsumed.ToString() + " Received: " + ticket.isConsumed.ToString());
            Assert.True(oldId == ticket.Id, "Wrong Id. Expected: " + oldId.ToString() + " Received: " + ticket.Id.ToString());
            Assert.True(oldPhotoOk == ticket.PhotoOk, "Wrong PhotoOk. Expected: " + oldPhotoOk.ToString() + " Received: " + ticket.PhotoOk.ToString());
            Assert.True(oldEventId == ticket.EventId, "Wrong EventId. Expected: " + oldEventId.ToString() + " Received: " + ticket.EventId.ToString());
            Assert.True(oldUserId == ticket.UserId, "Wrong UserId. Expected: " + oldUserId.ToString() + " Received: " + ticket.UserId.ToString());


            // Reset to original values
            updateTicketDTO = new UpdateTicketDTO
            {
                isConsumed = oldIsConsumed,
                TakeAway = oldTakeAway,
                TakeAwayTime = oldTakeAwayTime
            };

            json = JsonConvert.SerializeObject(updateTicketDTO);
            payload = new StringContent(json, UnicodeEncoding.UTF8, "application/json");
            response = await client.PutAsync("api/tickets/-1", payload);

            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK),"Wrong status code. Expected: OK. Received: " + response.StatusCode.ToString());

            serializedUser = await response.Content.ReadAsStringAsync();
            ticket = JsonConvert.DeserializeObject<Ticket>(serializedUser);

            // Verify
            Assert.True(oldIsConsumed == ticket.isConsumed, "Wrong isConsumed. Expected: " + oldIsConsumed.ToString() + " Received: " + ticket.isConsumed.ToString());
            Assert.True(oldTakeAway == ticket.TakeAway, "Wrong TakeAway. Expected: " + oldTakeAway.ToString() + " Received: " + ticket.TakeAway.ToString());
            Assert.True(oldTakeAwayTime == ticket.TakeAwayTime, "Wrong TakeAwayTime. Expected: " + oldTakeAwayTime.ToString() + " Received: " + ticket.TakeAwayTime.ToString());
            Assert.True(oldId == ticket.Id, "Wrong Id. Expected: " + oldId.ToString() + " Received: " + ticket.Id.ToString());
            Assert.True(oldPhotoOk == ticket.PhotoOk, "Wrong PhotoOk. Expected: " + oldPhotoOk.ToString() + " Received: " + ticket.PhotoOk.ToString());
            Assert.True(oldEventId == ticket.EventId, "Wrong EventId. Expected: " + oldEventId.ToString() + " Received: " + ticket.EventId.ToString());
            Assert.True(oldUserId == ticket.UserId, "Wrong UserId. Expected: " + oldUserId.ToString() + " Received: " + ticket.UserId.ToString());
        } 

        [Fact]
        public async Task SendTicketViaEmailAdmin()
        {
            var client =  await TestUtils.Login("admin");

            var sendTickerViaMailDTO = new SendTicketViaMailDTO
            {
                mail = "test.testsson@gmail.com",
                eventId = -1,
                numberOfTickets = 1
            };

            var json = JsonConvert.SerializeObject(sendTickerViaMailDTO);
            var payload = new StringContent(json, UnicodeEncoding.UTF8, "application/json");
            var response = await client.PostAsync("api/tickets/send", payload);

            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK),"Wrong status code. Expected: OK. Received: " + response.StatusCode.ToString());

            
        }

        [Fact]
        public async Task SendTicketViaEmailNotLoggedIn()
        {
            var application = new WebApplicationFactory<Program>();
            var client = application.CreateClient();

            var sendTickerViaMailDTO = new SendTicketViaMailDTO
            {
                mail = "test.testsson@gmail.com",
                eventId = -1,
                numberOfTickets = 1

            };

            var json = JsonConvert.SerializeObject(sendTickerViaMailDTO);
            var payload = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

            var response = await client.PostAsync("api/tickets/send", payload);

            Assert.True(response.StatusCode.Equals(HttpStatusCode.Unauthorized),"Wrong status code. Expected: Unauthorized. Received: " + response.StatusCode.ToString());
        }

        [Fact]
        public async Task SendTicketViaEmailNotAdmin()
        {
            var client =  await TestUtils.Login("student1");

            var sendTickerViaMailDTO = new SendTicketViaMailDTO
            {
                mail = "test.testsson@gmail.com",
                eventId = -1,
                numberOfTickets = 1
                
            };

            var json = JsonConvert.SerializeObject(sendTickerViaMailDTO);
            var payload = new StringContent(json, UnicodeEncoding.UTF8, "application/json");
            var response = await client.PostAsync("api/tickets/send", payload);

            Assert.True(response.StatusCode.Equals(HttpStatusCode.Forbidden),"Wrong status code. Expected: Forbidden. Received: " + response.StatusCode.ToString());
            
        }

        [Fact]
        public async Task sendTicketViaEmailEventNotFound(){
            var client =  await TestUtils.Login("admin");

            var sendTickerViaMailDTO = new SendTicketViaMailDTO
            {
                mail = "test.testsson@gmail.com",
                eventId = -100,
                numberOfTickets = 1
            };

            var json = JsonConvert.SerializeObject(sendTickerViaMailDTO);
            var payload = new StringContent(json, UnicodeEncoding.UTF8, "application/json");
            var response = await client.PostAsync("api/tickets/send", payload);

            Assert.True(response.StatusCode.Equals(HttpStatusCode.NotFound),"Wrong status code. Expected: NotFound. Received: " + response.StatusCode.ToString());

        }

        [Fact]
        public async Task sendTicketViaEmailLessThanOneTicket(){
            var client =  await TestUtils.Login("admin");

            var sendTickerViaMailDTO = new SendTicketViaMailDTO
            {
                mail = "test.testsson@gmail.com",
                eventId = -1,
                numberOfTickets = 0
            };

            var json = JsonConvert.SerializeObject(sendTickerViaMailDTO);
            var payload = new StringContent(json, UnicodeEncoding.UTF8, "application/json");
            var response = await client.PostAsync("api/tickets/send", payload);

            Assert.True(response.StatusCode.Equals(HttpStatusCode.BadRequest),"Wrong status code. Expected: BadRequest. Received: " + response.StatusCode.ToString());
        }

        [Fact]
        public async Task sendTicketViaEmailSeveralTickets(){
            var client =  await TestUtils.Login("admin");

            var sendTickerViaMailDTO = new SendTicketViaMailDTO
            {
                mail = "test.testsson@gmail.com",
                eventId = -1,
                numberOfTickets = 2
            };

            var json = JsonConvert.SerializeObject(sendTickerViaMailDTO);
            var payload = new StringContent(json, UnicodeEncoding.UTF8, "application/json");
            var response = await client.PostAsync("api/tickets/send", payload);

            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK),"Wrong status code. Expected: OK. Received: " + response.StatusCode.ToString());
        }

        [Fact]
        public async Task sendTicketViaEmailSeveralTicketsNonAdmin(){
            var client =  await TestUtils.Login("student1");

            var sendTickerViaMailDTO = new SendTicketViaMailDTO
            {
                mail = "test.testsson@gmail.com",
                eventId = -1,
                numberOfTickets = 2
            };

            var json = JsonConvert.SerializeObject(sendTickerViaMailDTO);
            var payload = new StringContent(json, UnicodeEncoding.UTF8, "application/json");
            var response = await client.PostAsync("api/tickets/send", payload);

            Assert.True(response.StatusCode.Equals(HttpStatusCode.Forbidden),"Wrong status code. Expected: Forbidden. Received: " + response.StatusCode.ToString());

        }

        [Fact]
        public async Task sendTicketViaEmailSeveralTicketsNonLoggedIn(){
            var application = new WebApplicationFactory<Program>();
            var client = application.CreateClient();

            var sendTickerViaMailDTO = new SendTicketViaMailDTO
            {
                mail = "test.testsson@gmail.com",   
                eventId = -1,
                numberOfTickets = 2
            };

            var json = JsonConvert.SerializeObject(sendTickerViaMailDTO);
            var payload = new StringContent(json, UnicodeEncoding.UTF8, "application/json");
            var response = await client.PostAsync("api/tickets/send", payload);

            Assert.True(response.StatusCode.Equals(HttpStatusCode.Unauthorized),"Wrong status code. Expected: Unauthorized. Received: " + response.StatusCode.ToString());
        }

        [Fact]
        public async Task TooManyTickets()
        {
            // Student2 has 5 tickets already but not for the event with the id -7,
            // Therefore booking event -7 should return "too many requests" because the user has too many tickets
            var client = await TestUtils.Login("student2");
            var json = new JsonObject
            {
                { "eventid", -7 },
                { "photook", true }
            };
            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("api/tickets", payload);
            // Verify response - Conflict because the student already has a ticket for the event
            Assert.True(response.StatusCode.Equals(HttpStatusCode.TooManyRequests),
                "Wrong status code. Expected: TooManyRequests. Received: " + response);
        }
    } 
}
