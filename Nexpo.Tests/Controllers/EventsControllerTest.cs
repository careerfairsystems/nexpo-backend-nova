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
using Nexpo.DTO;

namespace Nexpo.Tests.Controllers
{
	public class EventsControllerTests
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
		public async Task GetAllEventsNotLoggedIn()
		{
            var application = new WebApplicationFactory<Nexpo.Program>();
            var client = application.CreateClient();
            var response = await client.GetAsync("/api/events");

            string responseText = await response.Content.ReadAsStringAsync();
            var responseList = JsonConvert.DeserializeObject<List<Event>>(responseText);
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK));
            Assert.True(responseList.Count == 4, responseText.ToString());

            var firstEvent = responseList.Find(r => r.Id == -1);
            var secondEvent = responseList.Find(r => r.Id == -2);
            var thirdEvent = responseList.Find(r => r.Id == -3);
            var fourthEvent = responseList.Find(r => r.Id == -4);

            Assert.True(firstEvent.Description == "Breakfast with SEB", firstEvent.Description);
            Assert.True(secondEvent.Date == "2021-11-13", secondEvent.Date);
            Assert.True(thirdEvent.Host == "Randstad", thirdEvent.Host);
            Assert.True(fourthEvent.Capacity == 2, fourthEvent.Capacity.ToString());
        }

        [Fact]
        public async Task GetSpecificEventNotLoggedIn()
        {
            var application = new WebApplicationFactory<Nexpo.Program>();
            var client = application.CreateClient();
            var response = await client.GetAsync("/api/events/-3");

            string responseText = await response.Content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject<Event>(responseText);
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK));

            Assert.True(responseObject.Name == "CV Workshop with Randstad", responseText);
            Assert.True(responseObject.Date == "2021-11-14", responseText);
            Assert.True(responseObject.End == "15:00", responseText);
            Assert.True(responseObject.Language == "Swedish", responseText);
        }

        [Fact]
        public async Task GetEventWithIncorrectIdNotLoggedIn()
        {
            var application = new WebApplicationFactory<Nexpo.Program>();
            var client = application.CreateClient();
            var response = await client.GetAsync("/api/events/123");

            Assert.True(response.StatusCode.Equals(HttpStatusCode.NotFound), response.StatusCode.ToString());
        }

        [Fact]
        public async Task GetAllTicketsNotLoggedIn()
        {
            var application = new WebApplicationFactory<Nexpo.Program>();
            var client = application.CreateClient();
            var response = await client.GetAsync("/api/events/2/tickets");

            Assert.True(response.StatusCode.Equals(HttpStatusCode.Unauthorized), response.StatusCode.ToString());
        }

        [Fact]
        public async Task GetAllTicketsAsCompRep()
        {
            var application = new WebApplicationFactory<Nexpo.Program>();
            var client = application.CreateClient();
            var token = Login("company", client);

            var response = await client.GetAsync("/api/events/2/tickets");
            Assert.True(response.StatusCode.Equals(HttpStatusCode.Unauthorized), response.StatusCode.ToString());
        }


        [Fact]
        public async Task GetAllTicketsAsAdmin()
        {
            var application = new WebApplicationFactory<Nexpo.Program>();
            var client = application.CreateClient();
            var token = await Login("admin", client);

            var response = await client.GetAsync("/api/events/-1/tickets");
            string responseText = await response.Content.ReadAsStringAsync();
            var responseList = JsonConvert.DeserializeObject<List<NamedTicketDto>>(responseText);
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), response.StatusCode.ToString());
            Assert.True(responseList.Count == 3, responseText.ToString());

            var firstTicket = responseList.Find(r => r.ticket.Id == -1);
            var seventhTicket = responseList.Find(r => r.ticket.Id == -7);

            Assert.True(firstTicket.userFirstName == "Alpha", firstTicket.ticket.EventId.ToString());
            Assert.True(firstTicket.userLastName == "Student", firstTicket.ticket.EventId.ToString());
            Assert.True(firstTicket.ticket.EventId == -1, firstTicket.ticket.EventId.ToString());
            Assert.True(firstTicket.ticket.UserId == -2, firstTicket.ticket.UserId.ToString());
            Assert.True(seventhTicket.ticket.UserId == -4, seventhTicket.ticket.UserId.ToString());
        }

        [Fact]
        public async Task GetAllTicketsAsAdminWithWrongId()
        {
            var application = new WebApplicationFactory<Nexpo.Program>();
            var client = application.CreateClient();
            var token = await Login("admin", client);

            var response = await client.GetAsync("/api/events/55/tickets");
            Assert.True(response.StatusCode.Equals(HttpStatusCode.NotFound), response.StatusCode.ToString());
        }
        
        [Fact]
        public async Task PutEvent()
        {
            var application = new WebApplicationFactory<Nexpo.Program>();
            var client = application.CreateClient();
            var token = await Login("admin", client);
            var dto = new AddEventDto();
            dto.Description = "New description";
            dto.Date = "2011-03-07";
            dto.End = "17:00";
            dto.Language = "English";
            dto.Capacity = 25;
            
            var json = new JsonObject();
            json.Add("description", dto.Description);
            json.Add("date", dto.Date);
            json.Add("end", dto.End);
            json.Add("language", dto.Language);
            json.Add("capacity", dto.Capacity);
            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await client.PutAsync("/api/events/-1", payload);

            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), response.StatusCode.ToString());

            string responseText = await response.Content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject<AddEventDto>(responseText);
         
            Assert.True(responseObject.Description == "New description", $"Description was actually ({responseObject.Description})");
            Assert.True(responseObject.Date == "2011-03-07", $"Description was actually ({responseObject.Date})");
            Assert.True(responseObject.End == "17:00", $"Description was actually ({responseObject.End})");
            Assert.True(responseObject.Language == "English", $"Description was actually ({responseObject.Language})");
            Assert.True(responseObject.Capacity == 25, $"Description was actually ({responseObject.Capacity})");
            Assert.True(responseObject.Name == "Breakfast Mingle", $"Description was actually ({responseObject.Name})");
            Assert.True(responseObject.Start == "08:15", $"Description was actually ({responseObject.Start})");
            json.Remove("description");
            json.Remove("date");
            json.Remove("end");
            json.Remove("language");
            json.Remove("capacity");
            json.Add("description", "Breakfast with SEB");
            json.Add("date", "2021-11-12");
            json.Add("end", "10:00");
            json.Add("language", "Swedish");
            json.Add("capacity", 30);
            await client.PutAsync("/api/events/-1", new StringContent(json.ToString(), Encoding.UTF8, "application/json"));
        }

        [Fact]
        public async Task PutUnauthorized()
        {
            var application = new WebApplicationFactory<Nexpo.Program>();
            var client = application.CreateClient();
            var token = await Login("company", client);
            var dto = new AddEventDto();
            dto.Description = "None";
            
            var json = new JsonObject();
            json.Add("description", dto.Description);
            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await client.PutAsync("/api/events/-1", payload);

            Assert.True(response.StatusCode.Equals(HttpStatusCode.Forbidden), response.StatusCode.ToString());

            string responseText = await response.Content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject<AddEventDto>(responseText);
         
            Assert.True(responseObject == null, "Object was not null");
        }

        [Fact]
        public async Task PutEmpty()
        {
            var application = new WebApplicationFactory<Nexpo.Program>();
            var client = application.CreateClient();
            var token = await Login("admin", client);
            var dto = new AddEventDto();
            
            var json = new JsonObject();
            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await client.PutAsync("/api/events/-1", payload);

            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), response.StatusCode.ToString());

            string responseText = await response.Content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject<AddEventDto>(responseText);
         
            Assert.True(responseObject.Description == "Breakfast with SEB", $"Description was actually ({responseObject.Description})");
            Assert.True(responseObject.Date == "2021-11-12", $"Description was actually ({responseObject.Date})");
            Assert.True(responseObject.End == "10:00", $"Description was actually ({responseObject.End})");
            Assert.True(responseObject.Language == "Swedish", $"Description was actually ({responseObject.Language})");
            Assert.True(responseObject.Capacity == 30, $"Description was actually ({responseObject.Capacity})");
            Assert.True(responseObject.Name == "Breakfast Mingle", $"Description was actually ({responseObject.Name})");
            Assert.True(responseObject.Start == "08:15", $"Description was actually ({responseObject.Start})");
            Assert.True(responseObject.Location == "Cornelis", $"Description was actually ({responseObject.Location})");
        }

        [Fact]
        public async Task TestFoodPref()
        {
            var application = new WebApplicationFactory<Nexpo.Program>();
            var client = application.CreateClient();
            var token = await Login("company", client);

            var response = await client.GetAsync("/api/events/2/foodprefs");

            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), response.StatusCode.ToString());
            




        }

    }
}