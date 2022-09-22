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
using SendGrid;
using static System.Net.Mime.MediaTypeNames;
using Newtonsoft.Json;
using Nexpo.Models;
using System.Collections.Generic;
using System.Runtime.InteropServices;

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

            var firstEvent = responseList.Find(r => r.Id == 1);
            var secondEvent = responseList.Find(r => r.Id == 2);
            var thirdEvent = responseList.Find(r => r.Id == 3);
            var fourthEvent = responseList.Find(r => r.Id == 4);

            Assert.True(firstEvent.Description == "Breakfast with SEB", firstEvent.Description);
            Assert.True(secondEvent.Date == "2021-11-13", secondEvent.Date);
            Assert.True(thirdEvent.Host == "Randstad", thirdEvent.Host);
            Assert.True(fourthEvent.Capacity == 10, fourthEvent.Capacity.ToString());
        }
    }
}