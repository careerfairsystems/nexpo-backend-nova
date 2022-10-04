using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Xunit;
using System.IO;
using Newtonsoft.Json;
using Nexpo.Models;
using System.Collections.Generic;


namespace Nexpo.Tests.Controllers
{
    public class TicketsControllerTest
    {
        private async Task<String> Login(string role, HttpClient client)
        {
            var json = new JsonObject();
            switch (role)
            {
                case "company":
                    json.Add("email", "rep1@company1.example.com");
                    json.Add("password", "password");
                    break;
                case "admin":
                    json.Add("email", "admin@example.com");
                    json.Add("password", "password");
                    break;
                default:
                    json.Add("email", "student1@example.com");
                    json.Add("password", "password");
                    break;
            }

            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("/api/session/signin", payload);
            string token = new StreamReader(response.Content.ReadAsStream()).ReadToEnd();
            var parser = JObject.Parse(token);
            token = "Bearer " + parser.Value<String>("token");
            client.DefaultRequestHeaders.Add("Authorization", token);
            return token;
        }

        [Fact]
        public async Task GetAllTicketsForSignedInUser()
        {
            var application = new WebApplicationFactory<Nexpo.Program>();
            var client = application.CreateClient();
            var token = await Login("", client);

            var response = await client.GetAsync("/api/tickets");
            var responseList = JsonConvert.DeserializeObject<List<Ticket>>((await response.Content.ReadAsStringAsync()));
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), response.StatusCode.ToString());
            Assert.True(responseList.Count == 2, "Nbr. of ticket: " + responseList.Count.ToString());

            var ticket1 = responseList.Find(r => r.Id == -1);
            var ticket2 = responseList.Find(r => r.Id == -3);

            Assert.True(ticket1.PhotoOk, ticket1.PhotoOk.ToString());
            Assert.True(!ticket2.PhotoOk, ticket2.PhotoOk.ToString());
        }

        //May change depending on access decision
        [Fact]
        public async Task GetAllTicketsForCompanyWith0()
        {
            var application = new WebApplicationFactory<Nexpo.Program>();
            var client = application.CreateClient();
            var token = await Login("company", client);

            var response = await client.GetAsync("/api/tickets");
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), response.StatusCode.ToString());
            
            var responseList = JsonConvert.DeserializeObject<List<Ticket>>((await response.Content.ReadAsStringAsync()));
            Assert.True(responseList.Count == 0, responseList.Count.ToString());
        }

        [Fact]
        public async Task GetAllTicketsNotLoggedIn()
        {
            var application = new WebApplicationFactory<Nexpo.Program>();
            var client = application.CreateClient();
            
            var response = await client.GetAsync("/api/tickets");
            Assert.True(response.StatusCode.Equals(HttpStatusCode.Unauthorized), response.StatusCode.ToString());
        }


        [Fact]
        public async Task GetSpecificTicketLegitimate()
        {
            var application = new WebApplicationFactory<Nexpo.Program>();
            var client = application.CreateClient();
            var token = await Login("", client);

            var response = await client.GetAsync("/api/tickets/-1");
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), response.StatusCode.ToString());

            var responseObject = JsonConvert.DeserializeObject<Ticket>((await response.Content.ReadAsStringAsync()));
            Assert.True(responseObject.Id == -1, responseObject.Id.ToString());
            Assert.True(responseObject.PhotoOk, responseObject.PhotoOk.ToString());
            Assert.True(responseObject.EventId == -1, responseObject.EventId.ToString());
            Assert.True(responseObject.UserId == -2, responseObject.UserId.ToString());
        }

        //Should this be possible? 
        [Fact]
        public async Task GetSpecificTicketFromAnotherUser()
        {
            var application = new WebApplicationFactory<Nexpo.Program>();
            var client = application.CreateClient();
            var token = await Login("company", client);

            var response = await client.GetAsync("/api/tickets/-1");
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), response.StatusCode.ToString());

            var responseObject = JsonConvert.DeserializeObject<Ticket>((await response.Content.ReadAsStringAsync()));
            Assert.True(responseObject.Id == -1, responseObject.Id.ToString());
            Assert.True(responseObject.PhotoOk, responseObject.PhotoOk.ToString());
            Assert.True(responseObject.EventId == -1, responseObject.EventId.ToString());
            Assert.True(responseObject.UserId == -2, responseObject.UserId.ToString());
        }

        [Fact]
        public async Task GetNonExistingTicket()
        {
            var application = new WebApplicationFactory<Nexpo.Program>();
            var client = application.CreateClient();
            var token = await Login("", client);

            var response = await client.GetAsync("/api/tickets/-123");
            Assert.True(response.StatusCode.Equals(HttpStatusCode.NotFound), response.StatusCode.ToString());
        }

        [Fact]
        public async Task PostMultipleTicketsToSameEvent()
        {
            var application = new WebApplicationFactory<Nexpo.Program>();
            var client = application.CreateClient();
            var token = await Login("", client);

            var json = new JsonObject();
            json.Add("eventid", -4);
            json.Add("photook", true);

            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("api/tickets", payload);
            Assert.True(response.StatusCode.Equals(HttpStatusCode.Conflict), response.ToString());
        }

        [Fact]
        public async Task PostTicketToWrongIdEvent()
        {
            var application = new WebApplicationFactory<Nexpo.Program>();
            var client = application.CreateClient();
            var token = await Login("", client);

            var json = new JsonObject();
            json.Add("eventid", -12);
            json.Add("photook", true);

            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("api/tickets", payload);
            Assert.True(response.StatusCode.Equals(HttpStatusCode.NotFound), response.ToString());
        }

        [Fact]
        public async Task PostTicketToFullEvent()
        {
            var application = new WebApplicationFactory<Nexpo.Program>();
            var client = application.CreateClient();
            var token = await Login("", client);

            var json = new JsonObject();
            json.Add("eventid", -4);
            json.Add("photook", true);

            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("api/tickets", payload);
            Assert.True(response.StatusCode.Equals(HttpStatusCode.Conflict), response.ToString());
        }

        [Fact]
        public async Task PostTicketUnauthorized()
        {
            var application = new WebApplicationFactory<Nexpo.Program>();
            var client = application.CreateClient();
            
            var json = new JsonObject();
            json.Add("eventid", -1);
            json.Add("photook", true);

            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("api/tickets", payload);
            Assert.True(response.StatusCode.Equals(HttpStatusCode.Unauthorized), response.ToString());
        }

        [Fact]
        public async Task DeleteAnotherUsersTicket()
        {
            var application = new WebApplicationFactory<Nexpo.Program>();
            var client = application.CreateClient();
            var token = await Login("", client);

            var response = await client.DeleteAsync("api/tickets/-2");
            Assert.True(response.StatusCode.Equals(HttpStatusCode.Forbidden), response.ToString());
        }

        [Fact]
        public async Task DeleteNonExistingTicket()
        {
            var application = new WebApplicationFactory<Nexpo.Program>();
            var client = application.CreateClient();
            var token = await Login("", client);

            var response = await client.DeleteAsync("api/tickets/-22");
            Assert.True(response.StatusCode.Equals(HttpStatusCode.NotFound), response.ToString());
        }

        [Fact]
        public async Task PostandDeleteTicketLegitimate()
        {
            var application = new WebApplicationFactory<Nexpo.Program>();
            var client = application.CreateClient();
            var token = await Login("", client);

            //Add new ticket
            var json = new JsonObject();
            json.Add("eventid", -3);
            json.Add("photook", true);

            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("api/tickets", payload);
            Assert.True(response.StatusCode.Equals(HttpStatusCode.Created), response.ToString());
            
            //Check response and db updated
            string responseText = await response.Content.ReadAsStringAsync();
            var parsedContent = JObject.Parse(responseText);
            var newTicketId = parsedContent.Value<string>("id");

            var response2 = await client.GetAsync("/api/tickets/" + newTicketId);
            Assert.True(response2.StatusCode.Equals(HttpStatusCode.OK), response2.StatusCode.ToString());
            var responseObject = JsonConvert.DeserializeObject<Ticket>((await response2.Content.ReadAsStringAsync()));
            Assert.True(responseObject.PhotoOk, responseObject.PhotoOk.ToString());
            Assert.True(responseObject.EventId == -3, responseObject.EventId.ToString());
            Assert.True(responseObject.UserId == -2, responseObject.UserId.ToString());

            var response3 = await client.GetAsync("/api/tickets");
            var responseList = JsonConvert.DeserializeObject<List<Ticket>>((await response3.Content.ReadAsStringAsync()));
            Assert.True(response3.StatusCode.Equals(HttpStatusCode.OK), response3.StatusCode.ToString());
            Assert.True(responseList.Count == 3, "Nbr. of ticket: " + responseList.Count.ToString());

            //Delete created ticket
            var response4 = await client.DeleteAsync("api/tickets/" + newTicketId);
            Assert.True(response4.StatusCode.Equals(HttpStatusCode.NoContent));

            //Check ticket does not exist and users total nbr. of tickets
            var response5 = await client.GetAsync("/api/tickets");
            var responseList2 = JsonConvert.DeserializeObject<List<Ticket>>((await response5.Content.ReadAsStringAsync()));
            Assert.True(response5.StatusCode.Equals(HttpStatusCode.OK), response5.StatusCode.ToString());
            Assert.True(responseList2.Count == 2, "Nbr. of ticket: " + responseList.Count.ToString());

            var response6 = await client.GetAsync("/api/tickets/" + newTicketId);
            Assert.True(response6.StatusCode.Equals(HttpStatusCode.NotFound), response6.StatusCode.ToString());
        }
    }
}
