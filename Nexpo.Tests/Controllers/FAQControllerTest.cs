using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Nexpo.Models;
using Nexpo.DTO;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;


namespace Nexpo.Tests.Controllers
{
    public class FAQControllerTest
    {

        [Fact]
        public async Task GetAll()
        {   
            //var client = await TestUtils.Login("admin");
            var application = new WebApplicationFactory<Program>();
            var client = application.CreateClient();

            var response = await client.GetAsync("/api/FAQ/");
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), "Wrong StatusCode. Expected: OK. Received: " + response.StatusCode.ToString());

            var responseList = JsonConvert.DeserializeObject<List<FrequentAskedQuestion>>(await response.Content.ReadAsStringAsync());
            var question1 = responseList.Find(r => r.Id == -1);
            var question2 = responseList.Find(r => r.Id == -2);
            var question3 = responseList.Find(r => r.Id == -3);
            var question4 = responseList.Find(r => r.Id == -4);
            
            var numberOfFAQ = responseList.Count;
            Assert.True(numberOfFAQ == 4, "Wrong number of question. Expected: 4. Received: " + numberOfFAQ);

            Assert.True(question1.Question.Equals("Frequent Asked Question 1"), "Wrong question Expected: Frequent Asked Question 1. Received: " + question1.Question);
            Assert.True(question2.Question.Equals("Frequent Asked Question 2"), "Wrong question Expected: Frequent Asked Question 2. Received: " + question2.Question);
            Assert.True(question3.Question.Equals("Frequent Asked Question 3"), "Wrong question Expected: Frequent Asked Question 3. Received: " + question3.Question);
            Assert.True(question4.Question.Equals("Frequent Asked Question 4"), "Wrong question Expected: Frequent Asked Question 4. Received: " + question4.Question);

            Assert.True(question1.Id == -1, "Wrong Id. Expected: -1. Received: " + question1.Id);
            Assert.True(question2.Id == -2, "Wrong Id. Expected: -2. Received: " + question2.Id);
            Assert.True(question3.Id == -3, "Wrong Id. Expected: -3. Received: " + question3.Id);
            Assert.True(question4.Id == -4, "Wrong Id. Expected: -4. Received: " + question4.Id);
    
        }
            
        

        [Fact]
        public async Task Get()
        {
            //var client = await TestUtils.Login("admin");
            var application = new WebApplicationFactory<Program>();
            var client = application.CreateClient();
            var response = await client.GetAsync("/api/FAQ/-1");

            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), "Wrong Status Code. Expected: OK. Received: " + response.StatusCode.ToString());

            var faq = JsonConvert.DeserializeObject<FrequentAskedQuestion>(await response.Content.ReadAsStringAsync());

            Assert.True(faq.Id == -1, "Wrong question id. Expected: -1. Received: " + faq.Id.ToString());
            Assert.True(faq.Question.Equals("Frequent Asked Question 1"), "Wrong question. Expected: Frequent Asked Question 1 Received: " + faq.Question.ToString());

        }

        [Fact]
        public async Task GetNonExisting()
        {
            var application = new WebApplicationFactory<Program>();
            var client = application.CreateClient();
            var response = await client.GetAsync("/api/FAQ/-123");
        
            Assert.True(response.StatusCode.Equals(HttpStatusCode.NotFound), "Wrong StatusCode. Expected: NotFound. Received: " + response.StatusCode.ToString());

        }

        [Fact]
        public async Task PutNonExisting()
        {
            var client = await TestUtils.Login("admin");

            var updateFAQDto = new UpdateFAQDTO
            {
                Question = "New Question"
            };

            var json = JsonConvert.SerializeObject(updateFAQDto);
            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await client.PutAsync("api/FAQ/-123", payload);

            Assert.True(response.StatusCode.Equals(HttpStatusCode.NotFound), "Wrong status code. Expected: NotFound. Received: " + response.StatusCode.ToString());

        }


        [Fact]
        public async Task PutAsAdmin()
        {
            var client = await TestUtils.Login("admin");

            var updateFAQDto = new UpdateFAQDTO
            {
                Question = "New Question"
            };

            var json = JsonConvert.SerializeObject(updateFAQDto);
            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await client.PutAsync("api/FAQ/-1", payload);

            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK),"Wrong status code. Expected: OK. Received: " + response.StatusCode.ToString());

            var faq = JsonConvert.DeserializeObject<FrequentAskedQuestion>(await response.Content.ReadAsStringAsync());
            
            Assert.True(faq.Question.Equals("New Question"), "Wrong Question Expected: New Question. Received: " + faq.Question.ToString());

            updateFAQDto = new UpdateFAQDTO
            {
                Question = "Frequent Asked Question 1"
            };

            json = JsonConvert.SerializeObject(updateFAQDto);
            payload = new StringContent(json, UnicodeEncoding.UTF8, "application/json");
            response = await client.PutAsync("api/FAQ/-1", payload);

            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK),"Wrong status code. Expected: OK. Received: " + response.StatusCode.ToString());

            faq = JsonConvert.DeserializeObject<FrequentAskedQuestion>(await response.Content.ReadAsStringAsync());
            
            Assert.True(faq.Question.Equals("Frequent Asked Question 1"), "Wrong Question. Expected: Frequent Asked Question 1. Received: " + faq.Question.ToString());

        }

        [Fact]
        public async Task PutAsStudent()
        {
            var client = await TestUtils.Login("student1");

            var updateFAQDto = new UpdateFAQDTO
            {
                Question = "New Question"
            };

            var json = JsonConvert.SerializeObject(updateFAQDto);
            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await client.PutAsync("api/FAQ/-1", payload);

            Assert.True(response.StatusCode.Equals(HttpStatusCode.Forbidden), "Wrong status code. Expected: Forbidden. Received: " + response.StatusCode.ToString());
            
        }

        [Fact]
        public async Task PutLoggedIn()
        {
            var application = new WebApplicationFactory<Program>();
            var client = application.CreateClient();

            var updateFAQDto = new UpdateFAQDTO
            {
                Question = "New Question"
            };

            var json = JsonConvert.SerializeObject(updateFAQDto);
            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await client.PutAsync("api/FAQ/-1", payload);

            Assert.True(response.StatusCode.Equals(HttpStatusCode.Unauthorized), "Wrong status code. Expected: Unauthorized. Received: " + response.StatusCode.ToString());

        }

        [Fact]
        public async Task PostAndDeleteAsAdmin()
        {
            //Setup
            var client = await TestUtils.Login("admin");
            
            //Create FAQ
            var createFAQDto = new CreateFAQDTO()
            {
                Id = -5,
                Question = "Frequent Asked Question 5",
            };

            //Get nbr of FAQ
            var response = await client.GetAsync("/api/faq/");
            var allFAQ = JsonConvert.DeserializeObject<List<FrequentAskedQuestion>>(await response.Content.ReadAsStringAsync());
            var numberOfFAQ = allFAQ.Count;

            var json = JsonConvert.SerializeObject(createFAQDto);
            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");

            response = await client.PostAsync("/api/faq/add", payload);
            Assert.True(response.StatusCode.Equals(HttpStatusCode.Created), "Wrong Status Code. Expected: Created. Received: " + response.StatusCode.ToString());

            //Deserialize response and verify FAQ
            var faq = JsonConvert.DeserializeObject<FrequentAskedQuestion>(await response.Content.ReadAsStringAsync());
            
            Assert.True(faq.Id == -5, "Wrong Id. Expected: -5. Received: " + faq.Id);
            Assert.True(faq.Question.Equals("Frequent Asked Question 5"), "Wrong Question. Expected: Frequent Asked Question 5. Received: " + faq.Question);

            //Check that the nbr of FAQ has increased
            response = await client.GetAsync("/api/faq/");
            allFAQ = JsonConvert.DeserializeObject<List<FrequentAskedQuestion>>(await response.Content.ReadAsStringAsync());
            Assert.True(allFAQ.Count == numberOfFAQ + 1, "Wrong number of faq. Expected: " + (numberOfFAQ + 1) + ". Received: " + allFAQ.Count);

            //Delete the FAQ
            response = await client.DeleteAsync("/api/faq/" + faq.Id);
            Assert.True(response.StatusCode.Equals(HttpStatusCode.NoContent), "Wrong Status Code. Expected: NoContent. Received: " + response.StatusCode.ToString());

            //Check that the FAQ is deleted
            response = await client.GetAsync("/api/faq/" + faq.Id);
            Assert.True(response.StatusCode.Equals(HttpStatusCode.NotFound), "Wrong Status Code. Expected: NotFound. Received: " + response.StatusCode.ToString());

            //Check that the nbr of FAQ has decreased
            response = await client.GetAsync("/api/faq/");
            allFAQ = JsonConvert.DeserializeObject<List<FrequentAskedQuestion>>(await response.Content.ReadAsStringAsync());
            Assert.True(allFAQ.Count == numberOfFAQ, "Wrong number of faq. Expected: " + numberOfFAQ + ". Received: " + allFAQ.Count);

        }

        [Fact]
        public async Task PostAndDeleteAsStudent()
        {
            var client = await TestUtils.Login("student1");

            var createFAQDto = new CreateFAQDTO()
            {
                Id = -5,
                Question = "Frequent Asked Question 5",
            };

            var json = JsonConvert.SerializeObject(createFAQDto);
            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");

            var response = await client.PostAsync("/api/faq/add", payload);
            Assert.True(response.StatusCode.Equals(HttpStatusCode.Forbidden), "Wrong Status Code. Expected: Forbidden. Received: " + response.StatusCode.ToString());
           

            response = await client.DeleteAsync("/api/faq/-1");
            Assert.True(response.StatusCode.Equals(HttpStatusCode.Forbidden), "Wrong Status Code. Expected: Forbidden. Received: " + response.StatusCode.ToString());

        }

        [Fact]
        public async Task PostAndDeleteNotLoggedIn()
        {
            var application = new WebApplicationFactory<Program>();
            var client = application.CreateClient();

            var createFAQDto = new CreateFAQDTO()
            {
                Id = -5,
                Question = "Frequent Asked Question 5",
            };

            var json = JsonConvert.SerializeObject(createFAQDto);
            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");

            var response = await client.PostAsync("/api/faq/add", payload);
            Assert.True(response.StatusCode.Equals(HttpStatusCode.Unauthorized), "Wrong Status Code. Expected: Unauthorized. Received: " + response.StatusCode.ToString());
           

            response = await client.DeleteAsync("/api/faq/-1");
            Assert.True(response.StatusCode.Equals(HttpStatusCode.Unauthorized), "Wrong Status Code. Expected: Unauthorized. Received: " + response.StatusCode.ToString());

        }


        [Fact]
        public async Task DeleteNonExisting()
        {
            //Set up
            var client = await TestUtils.Login("admin");

            //Get nbr of FAQ
            var response = await client.GetAsync("/api/faq/");
            var allFAQ = JsonConvert.DeserializeObject<List<FrequentAskedQuestion>>(await response.Content.ReadAsStringAsync());
            var numberOfFAQ = allFAQ.Count;
            
            //Try to delete a non existing FAQ
            response = await client.DeleteAsync("/api/faq/-123");
            Assert.True(response.StatusCode.Equals(HttpStatusCode.NotFound), "Wrong Status Code. Expected: NotFound. Received: " + response.StatusCode.ToString());

            //Check that the nbr of FAQ is the same
            response = await client.GetAsync("/api/faq/");
            allFAQ = JsonConvert.DeserializeObject<List<FrequentAskedQuestion>>(await response.Content.ReadAsStringAsync());
            Assert.True(allFAQ.Count == numberOfFAQ, "Wrong number of faq. Expected: " + numberOfFAQ + ". Received: " + allFAQ.Count);
        }

        [Fact]
        public async Task PostExistingId()
        {
            //Set up
            var client = await TestUtils.Login("admin");

            //Create FAQ
            var createFAQDto = new CreateFAQDTO()
            {
                Id = -1,
                Question = " ",
            };

            //Get nbr of FAQ
            var response = await client.GetAsync("/api/faq/");
            var allFAQ = JsonConvert.DeserializeObject<List<FrequentAskedQuestion>>(await response.Content.ReadAsStringAsync());
            var numberOfFAQ = allFAQ.Count;
            
            //Try to post FAQ with id -1
            var json = JsonConvert.SerializeObject(createFAQDto);
            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");

            response = await client.PostAsync("/api/faq/add", payload);
            Assert.True(response.StatusCode.Equals(HttpStatusCode.BadRequest), "Wrong Status Code. Expected: BadRequest. Received: " + response.StatusCode.ToString());


            //Check that the nbr of FAQ is the same
            response = await client.GetAsync("/api/faq/");
            allFAQ = JsonConvert.DeserializeObject<List<FrequentAskedQuestion>>(await response.Content.ReadAsStringAsync());
            
            Assert.True(allFAQ.Count == numberOfFAQ, "Wrong number of faq. Expected: " + numberOfFAQ + ". Received: " + allFAQ.Count);
            
            
            //Check that the FAQ with id -1 is unchanged
            response = await client.GetAsync("/api/FAQ/-1");
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), "Wrong Status Code. Expected: OK. Received: " + response.StatusCode.ToString());

            var faq = JsonConvert.DeserializeObject<FrequentAskedQuestion>(await response.Content.ReadAsStringAsync());

            Assert.True(faq.Id == -1, "Wrong question id. Expected: -1. Received: " + faq.Id.ToString());
            Assert.True(faq.Question.Equals("Frequent Asked Question 1"), "Wrong question. Expected: Frequent Asked Question 1 Received: " + faq.Question.ToString());

        }
    }
}
