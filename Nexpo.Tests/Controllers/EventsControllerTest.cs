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

namespace Nexpo.Tests.Controllers
{
	public class EventsControllerTests
	{

		[Fact]
		public async Task GetAllEventsNotLoggedIn()
		{
            var application = new WebApplicationFactory<Nexpo.Program>();
            var client = application.CreateClient();
            var response = await client.GetAsync("/api/events");

            string responseText = await response.Content.ReadAsStringAsync();
            var responseList = JsonConvert.DeserializeObject<List<Event>>(responseText);

            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), "Wrong Status Code. Expected: OK. Recieved: " + response.StatusCode.ToString());
            Assert.True(responseList.Count == 5, "Wrong number of events. Expected: 5. Recieved: " + responseList.Count);

            var firstEvent = responseList.Find(r => r.Id == -1);
            var secondEvent = responseList.Find(r => r.Id == -2);
            var thirdEvent = responseList.Find(r => r.Id == -3);
            var fourthEvent = responseList.Find(r => r.Id == -4);

            Assert.True(firstEvent.Description.Equals("Breakfast with SEB"), "Wrong Description. Expected: Breakfast with SEB. Recieved: " + firstEvent.Description);
            Assert.True(secondEvent.Date.Equals("2022-11-13"), "Wrong Date. Expexted: 2022-11-13. Recieved: " + secondEvent.Date);
            Assert.True(thirdEvent.Host.Equals("Randstad"), "Wrong Host. Expected: Ranstad. Recieved: " + thirdEvent.Host);
            Assert.True(fourthEvent.Capacity == 2, "Wrong capacity. Expected: 2. Recieved: " + fourthEvent.Capacity.ToString());
        }

        [Fact]
        public async Task GetSpecificEventNotLoggedIn()
        {
            var application = new WebApplicationFactory<Nexpo.Program>();
            var client = application.CreateClient();
            var response = await client.GetAsync("/api/events/-3");

            string responseText = await response.Content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject<Event>(responseText);

            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), "Wrong Status Code. Expected: OK. Recieved: " + response.StatusCode.ToString());
            Assert.True(responseObject.Name.Equals("CV Workshop with Randstad"), "Wrong event name. Expected: CV Workshop with Randstad. Recieved: " + responseObject.Name);
            Assert.True(responseObject.Date.Equals("2022-11-14"), "Wrong Date. Expected: 2022-11-14. Recieved: " + responseObject.Date);
            Assert.True(responseObject.End.Equals("15:00"), "Wrong end time. Expected: 15:00. Recieved: "  + responseObject.End );
            Assert.True(responseObject.Language.Equals("Swedish"), "Wrong Language. Expected: Swedish. Recieved: " + responseObject.Language);
        }

        [Fact]
        public async Task GetEventWithIncorrectIdNotLoggedIn()
        {
            var application = new WebApplicationFactory<Nexpo.Program>();
            var client = application.CreateClient();
            var response = await client.GetAsync("/api/events/-123");

            Assert.True(response.StatusCode.Equals(HttpStatusCode.NotFound), "Wrong Status Code. Expected: NotFound. Recieved: " + response.StatusCode.ToString());
        }

        [Fact]
        public async Task GetAllTicketsNotLoggedIn()
        {
            var application = new WebApplicationFactory<Nexpo.Program>();
            var client = application.CreateClient();
            var response = await client.GetAsync("/api/events/2/tickets");

            Assert.True(response.StatusCode.Equals(HttpStatusCode.Unauthorized), "Wrong Status Code. Expected: Unautherized. Recieved: " + response.StatusCode.ToString());
        }

        [Fact]
        public async Task GetAllTicketsAsCompRep()
        {
            var client = await TestUtils.Login("company1");
            var response = await client.GetAsync("/api/events/-1/tickets");

            Assert.True(response.StatusCode.Equals(HttpStatusCode.Forbidden), "Wrong Status Code. Expected: Forbidden. Recieved: " + response.StatusCode.ToString());
        }


        [Fact]
        public async Task GetAllTicketsAsAdmin()
        {
            var client = await TestUtils.Login("admin");

            var response = await client.GetAsync("/api/events/-1/tickets");
            string responseText = await response.Content.ReadAsStringAsync();
            var responseList = JsonConvert.DeserializeObject<List<NamedTicketDto>>(responseText);

            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), "Wrong status code. Expected: OK. Recieved: " + response.StatusCode.ToString());
            Assert.True(responseList.Count == 3, "Wrong number of tickets. Expected: 3. Recieved: " + responseList.Count);

            var firstTicket = responseList.Find(r => r.ticket.Id == -1);
            var seventhTicket = responseList.Find(r => r.ticket.Id == -7);

            Assert.True(firstTicket.userFirstName.Equals("Alpha"), "Wrong ticket first name. Expected: Alpha. Recieved: " + firstTicket.userFirstName.ToString());
            Assert.True(firstTicket.userLastName.Equals("Student"), "Wrong ticket last name. Expected: Student. Recieved: " + firstTicket.userLastName.ToString());
            Assert.True(firstTicket.ticket.EventId == -1, "Wrong event id. Expected: -1. Recieved: " +  firstTicket.ticket.EventId.ToString());
            Assert.True(firstTicket.ticket.UserId == -2, "Wrong userID. Expected: -2. Recieved: " + firstTicket.ticket.UserId.ToString());
            Assert.True(seventhTicket.ticket.UserId == -4, "Wrong userID, Expected: -4. Reciéved: " + seventhTicket.ticket.UserId.ToString());
        }

        [Fact]
        public async Task GetAllTicketsAsAdminWithWrongId()
        {
            var client = await TestUtils.Login("admin");
            var response = await client.GetAsync("/api/events/-55/tickets");

            Assert.True(response.StatusCode.Equals(HttpStatusCode.NotFound), "Wrong status code. Expected: NotFound. Recieved: " + response.StatusCode.ToString());
        }
        
        [Fact]
        public async Task PutEvent()
        {
            var client = await TestUtils.Login("admin");
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

            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), "Wrong status code. Expected: OK. Recieved: " + response.StatusCode.ToString());

            json.Remove("description");
            json.Remove("date");
            json.Remove("end");
            json.Remove("language");
            json.Remove("capacity");
            json.Add("description", "Breakfast with SEB");
            json.Add("date", "2022-11-12");
            json.Add("end", "10:00");
            json.Add("language", "Swedish");
            json.Add("capacity", 30);
            var response2 = await client.PutAsync("/api/events/-1", new StringContent(json.ToString(), Encoding.UTF8, "application/json"));

            Assert.True(response2.StatusCode.Equals(HttpStatusCode.OK), "Wrong status code. Expected: OK. Recieved: " + response2.StatusCode.ToString());

            string responseText = await response.Content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject<AddEventDto>(responseText);
         
            Assert.True(responseObject.Description.Equals("New description"), $"Wrong description. Description was actually ({responseObject.Description})");
            Assert.True(responseObject.Date.Equals("2011-03-07"), $"Wrong date. Was actually ({responseObject.Date})");
            Assert.True(responseObject.End.Equals("17:00"), $"Wrong end time. Was actually ({responseObject.End})");
            Assert.True(responseObject.Language.Equals("English"), $"Wrong language. Was actually ({responseObject.Language})");
            Assert.True(responseObject.Capacity == 25, $"Wrong Capacity. Was actually ({responseObject.Capacity})");
            Assert.True(responseObject.Name.Equals("Breakfast Mingle"), $"Wrong name. Was actually ({responseObject.Name})");
            Assert.True(responseObject.Start.Equals("08:15"), $"Wrong starttime. Was actually ({responseObject.Start})");
        }

        [Fact]
        public async Task PutUnauthorized()
        {
            var client = await TestUtils.Login("company1");
            var dto = new AddEventDto();
            dto.Description = "None";
            
            var json = new JsonObject();
            json.Add("description", dto.Description);
            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await client.PutAsync("/api/events/-1", payload);
           
            Assert.True(response.StatusCode.Equals(HttpStatusCode.Forbidden), "Wrong status code. Expected: Forbidden. Recieved: " + response.StatusCode.ToString());

            string responseText = await response.Content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject<AddEventDto>(responseText);

            Assert.True(responseObject == null, "Returned Object was not null. Recieved: " + responseText);
        }

        [Fact]
        public async Task PutEmpty()
        {
            var client = await TestUtils.Login("admin");
            var dto = new AddEventDto();
            
            var json = new JsonObject();
            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await client.PutAsync("/api/events/-4", payload);

            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), response.StatusCode.ToString());

            string responseText = await response.Content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject<AddEventDto>(responseText);
         
            Assert.True(responseObject.Description.Equals("Get inspired and expand your horizons"), $"Wrong description. Description was actually ({responseObject.Description})");
            Assert.True(responseObject.Date.Equals("2022-11-15"), $"Wrong date. Date was actually ({responseObject.Date})");
            Assert.True(responseObject.End.Equals("13:00"), $"Wrong end time. Was actually ({responseObject.End})");
            Assert.True(responseObject.Language.Equals("Swedish"), $"Wrong Language. Was actually ({responseObject.Language})");
            Assert.True(responseObject.Capacity == 2, $"Wrong capacity. Was actually ({responseObject.Capacity})");
            Assert.True(responseObject.Name.Equals("Inspirational lunch lecture"), $"Wrong name. Was actually ({responseObject.Name})");
            Assert.True(responseObject.Start.Equals("12:15"), $"Wrong start time. Was actually ({responseObject.Start})");
            Assert.True(responseObject.Location.Equals("MA:3"), $"Wrong location. Was actually ({responseObject.Location})");
        }
    }
}