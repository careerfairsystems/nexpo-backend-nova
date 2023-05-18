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
using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;
using Nexpo.DTO;
using Microsoft.AspNetCore.Mvc;
using Nexpo.Models;
using System.Collections.Generic;

namespace Nexpo.Tests.Controllers
{
    public class FAQControllerTest
    {

        [Fact]
        
        public async Task GetAll()
        {
            // Non logged in
            var application = new WebApplicationFactory<Program>();
            
            var client = await TestUtils.Login("admin");
            // Get response
            var response = await client.GetAsync("/api/faq/");
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), "Wrong StatusCode. Expected: OK. Received: " + response.StatusCode.ToString());

            // Sample result
            var responseList = JsonConvert.DeserializeObject<List<FrequentAskedQuestion>>(await response.Content.ReadAsStringAsync());
            var question = responseList.Find(r => r.Id == -1);
            var question3 = responseList.Find(r => r.Id == -3);
            
            var numberOfQuestions = responseList.Count;
            Assert.True(numberOfQuestions == numberOfQuestions + 1, "Wrong number of question. Expected: " + (numberOfQuestions + 1) + ". Received: " + numberOfQuestions);
            Assert.True(question.Question.Equals("Frequent Asked Question 1"), "Wrong question Expected:  Question 1 Received: " + question.Question);
            Assert.True(question3.Question.Equals("Frequent Asked Question 2"), "Wrong question Expected:  Question 1 Received: " + question.Question);
            
            Assert.True(question.Id==(-1), "Wrong event id. Expected: -1. Received: " + question.Id);
            Assert.True(question3.Id==(-2), "Wrong event id. Expected:-2. Received: " + question3.Id);
    
        }
            
        

        [Fact]
        public async Task Get()
        {
            var client = await TestUtils.Login("admin");
            var response = await client.GetAsync("/api/FAQ/-1");

            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), "Wrong Status Code. Expected: OK. Received: " + response.StatusCode.ToString());

            var responseFAQ = JsonConvert.DeserializeObject<FrequentAskedQuestion>(await response.Content.ReadAsStringAsync());

            Assert.True(responseFAQ.Id == -1, "Wrong question id. Expected: -1. Received: " + responseFAQ.Id.ToString());
            Assert.True(responseFAQ.Question.Equals("Frequent Asked Question 1"), "Wrong question. Expected: Frequent Asked Question 1 Received: " + responseFAQ.Question.ToString());

        }

        [Fact]
        public async Task GetNonExisting()
        {

        }

        [Fact]
        public async Task GetUnautherized()
        {

        }


        [Fact]
        public async Task PostAsAdmin()
        {

        }

        [Fact]
        public async Task PostUnautherized()
        {
            var client = await TestUtils.Login("student1");

            var updateFAQDto = new UpdateFAQDTO
            {
                Question = ""
            };

            var json = JsonConvert.SerializeObject(updateFAQDto);

            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await client.PutAsync("api/faq/-1", payload);

            Assert.True(response.StatusCode.Equals(HttpStatusCode.Forbidden), "Wrong status code. Expected: Forbidden. Received: " + response.StatusCode.ToString());

        }

        [Fact]
        public async Task PostAndDeleteAsAdmin()
        {

        }

        [Fact]
        public async Task PostAndDeleteUnautherized()
        {

        }


    }
}