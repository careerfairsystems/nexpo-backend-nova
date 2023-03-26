using Microsoft.AspNetCore.Mvc.Testing;
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

namespace Nexpo.Tests.Controllers
{
    public class TicketsControllerTest
    {
        [Fact]
        public async Task GetAllTicketsForSignedInUser()
        {
            var client = await TestUtils.Login("student1");
            var response = await client.GetAsync("/api/tickets");
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), "Wrong status code. Expected: OK. Received: " + response.StatusCode.ToString());

            var responseList = JsonConvert.DeserializeObject<List<Ticket>>(await response.Content.ReadAsStringAsync());
            var ticket1 = responseList.Find(r => r.Id == -1);
            var ticket2 = responseList.Find(r => r.Id == -4);

            Assert.True(responseList.Count == 2, "Wrong number of ticket. Expected: 2. Received: " + responseList.Count.ToString());
            Assert.True(ticket1.PhotoOk, "Wrong PhotoOk value. Expected: true. Received: " + ticket1.PhotoOk.ToString());
            Assert.True(!ticket2.PhotoOk, "Wrong PhotoOk value. Expected: false. Received: " + ticket2.PhotoOk.ToString());
        }

        [Fact]
        public async Task GetAllTicketsForCompanyWith0()
        {
            var client = await TestUtils.Login("company1");
            var response = await client.GetAsync("/api/tickets");
            var responseList = JsonConvert.DeserializeObject<List<Ticket>>(await response.Content.ReadAsStringAsync());

            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), "Wrong status code. Expected: OK. Received: " + response.StatusCode.ToString());
            Assert.True(responseList.Count == 0, "Wrong number of ticket. Expected: 0. Receivd: " + responseList.Count.ToString());
        }

        [Fact]
        public async Task GetAllTicketsNotLoggedIn()
        {
            var application = new WebApplicationFactory<Program>();
            var client = application.CreateClient();
            var response = await client.GetAsync("/api/tickets");

            Assert.True(response.StatusCode.Equals(HttpStatusCode.Unauthorized), "Wrong status code. Expected: Unauthorized. Received: " + response.StatusCode.ToString());
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
        public async Task GetSpecificTicketFromAnotherUser()
        {
            var client = await TestUtils.Login("company1");
            var response = await client.GetAsync("/api/tickets/id/-1");

            Assert.True(response.StatusCode.Equals(HttpStatusCode.NotFound), "Wrong status code. Expected: NotFound. Received: " + response.StatusCode.ToString());
        }

        [Fact]
        public async Task GetSpecificTicketLegitimateAsAdmin()
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
        public async Task GetNonExistingTicket()
        {
            var client = await TestUtils.Login("admin");
            var response = await client.GetAsync("/api/tickets/id/-123");

            Assert.True(response.StatusCode.Equals(HttpStatusCode.NotFound), "Wrong status code. Expected: NotFound. Received: " + response.StatusCode.ToString());
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

            Assert.True(response.StatusCode.Equals(HttpStatusCode.Conflict), "Wrong status code. Expected: Conflict. Received: " + response.ToString());
        }

        [Fact]
        public async Task PostTicketToWrongIdEvent()
        {
            var client = await TestUtils.Login("student1");
            var json = new JsonObject
            {
                { "eventid", -123 },
                { "photook", true }
            };
            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("api/tickets", payload);

            Assert.True(response.StatusCode.Equals(HttpStatusCode.NotFound), "Wrong status code. Expected: NotFound. Received: " + response.ToString());
        }

        [Fact]
        public async Task PostTicketToFullEvent()
        {
            var client = await TestUtils.Login("student1");
            var json = new JsonObject
            {
                { "eventid", -4 },
                { "photook", true }
            };
            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("api/tickets", payload);

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

            Assert.True(response.StatusCode.Equals(HttpStatusCode.Unauthorized), "Wrong status code. Expected: Unauthorized. Received: " + response.ToString());
        }

        [Fact]
        public async Task PostTicketLessThanTwoDaysBefore()
        {
            var client = await TestUtils.Login("student1");
            var json = new JsonObject
            {
                { "eventid", -5 },
                { "photook", true }
            };
            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("api/tickets", payload);

            Assert.True(response.StatusCode.Equals(HttpStatusCode.BadRequest), "Wrong status code. Expected: BadRequest. Received: " + response.ToString());
        }

        [Fact]
        public async Task DeleteAnotherUsersTicket()
        {
            var client = await TestUtils.Login("student1");
            var response = await client.DeleteAsync("api/tickets/-2");

            Assert.True(response.StatusCode.Equals(HttpStatusCode.BadRequest), "Wrong status code. Expected: BadRequest. Received: " + response.ToString());
        }

        [Fact]
        public async Task DeleteNonExistingTicket()
        {
            var client = await TestUtils.Login("student1");
            var response = await client.DeleteAsync("api/tickets/-123");

            Assert.True(response.StatusCode.Equals(HttpStatusCode.NotFound), "Wrong status code. Expected: NotFound. Received: " + response.ToString());
        }

        [Fact]
        public async Task DeleteTicketCloseToEvent()
        {
            var client = await TestUtils.Login("student2");
            var response = await client.DeleteAsync("api/tickets/-8");

            Assert.True(response.StatusCode.Equals(HttpStatusCode.BadRequest), "Wrong status code. Expected: BadRequest. Received: " + response.ToString());
        }

        [Fact]
        public async Task PostandDeleteTicketLegitimate()
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
            Assert.True(responseList.Count == 3, "Wrong number of tickets. Expected: 3. Received: " + responseList.Count.ToString());

            var responseList2 = JsonConvert.DeserializeObject<List<Ticket>>((await response5.Content.ReadAsStringAsync()));
            Assert.True(responseList2.Count == 2, "Wrong number of tickets. Expected: 2. Received: " + responseList2.Count.ToString());
        }

        [Fact]
        public async Task AdminPostandDeleteTicketCloseToEvent()
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
        public async Task AdminDeleteTicketConsumed()
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
        public async Task AdminPostTicketWrongEventId()
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

            Assert.True(response.StatusCode.Equals(HttpStatusCode.NotFound), "Wrong status code. Expected: NotFound. Received: " + response.ToString());
         }

        [Fact]
        public async Task AdminPostDublicateTicketToUser()
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

            Assert.True(response.StatusCode.Equals(HttpStatusCode.Conflict), "Wrong status code. Expected: Conflict. Received: " + response.ToString());
        }

        [Fact]
        public async Task StudentPostTicketWrongRoute()
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
        public async Task GetTicketIdAndGuidReturnSameTicket()
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
    }
}
