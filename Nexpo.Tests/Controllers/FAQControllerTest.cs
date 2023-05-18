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
            var client = application.CreateClient();

            // Get response
            var response = await client.GetAsync("/api/companies/");
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), "Wrong StatusCode. Expected: OK. Received: " + response.StatusCode.ToString());

            // Sample result
            var responseList = JsonConvert.DeserializeObject<List<FrequentAskedQuestion>>(await response.Content.ReadAsStringAsync());
            
            var numberOfQuestions = responseList.Count;
            Assert.True(numberOfQuestions == numberOfQuestions + 1, "Wrong number of question. Expected: " + (numberOfQuestions + 1) + ". Received: " + numberOfQuestions);
            
            // Verify result
            
        }
            
        

        [Fact]
        public async Task Get()
        {

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