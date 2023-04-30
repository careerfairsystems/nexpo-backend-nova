using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Xunit;
using Newtonsoft.Json;
using Nexpo.Models;
using Nexpo.DTO;
using System.Collections.Generic;

namespace Nexpo.Tests.Controllers 
{
    public class CompaniesControllerTest
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
            var responseList = JsonConvert.DeserializeObject<List<PublicCompanyDTO>>(await response.Content.ReadAsStringAsync());
            var company = responseList.Find(r => r.Id == -4);

            // Verify result
            Assert.True(responseList.Count == 4, "Wrong number of companies. Expected: 4. Received: " + responseList.Count);

            Assert.True(company.Name.Equals("Facebook"), "Wrong company name. Expected: Facebook. Received: " + company.Name);
            Assert.True(company.Description.Equals("We have friends in common"), "Wrong description. Received: " + company.Description);
            Assert.True(company.DidYouKnow.Equals("Mark zuckerburg is an Alien"), "Wrong DidYouKnow. Received: " + company.DidYouKnow);

            Assert.True(company.DesiredDegrees.Contains((int) Degree.PhD), "Missing Desired Degree. Expected: PhD. Received: " + company.DesiredDegrees.ToString());
            Assert.True(company.DesiredDegrees.Contains((int)Degree.Master), "Missing Desired Degree. Expected: Master. Received: " + company.DesiredDegrees.ToString());
            Assert.True(company.DesiredDegrees.Count == 2, "Wrong number of Desired Degrees. Expected: 2. Received: " + company.DesiredDegrees.Count);

            Assert.True(company.DesiredProgramme.Contains((int)Programme.Byggteknik_med_Järnvägsteknik), "Missing Desired Programme. Expected: Byggteknik_med_Järnvägsteknik. Received: " + company.DesiredProgramme.ToString());
            Assert.True(company.DesiredProgramme.Contains((int)Programme.Teknisk_Fysik), "Missing Desired Programme. Expected: Teknisk_Fysik. Received: " + company.DesiredProgramme.ToString());
            Assert.True(company.DesiredProgramme.Count == 2, "Wrong number of Desired Programmes. Expected: 2. Received: " + company.DesiredProgramme.Count);

            Assert.True(company.Industries.Contains((int)Industry.Environment), "Missing Industry. Expected: Environment. Received: " + company.Industries.ToString());
            Assert.True(company.Industries.Contains((int)Industry.ElectricityEnergyPower), "Missing Industry. Expected: ElectricityEnergyPower. Received: " + company.Industries.ToString());
            Assert.True(company.Industries.Count == 2, "Wrong number of Industries. Expected: 2. Received: " + company.Industries.Count);

            Assert.True(company.Positions.Contains((int)Position.Thesis), "Missing Position. Expected: Thesis. Received: " + company.Positions.ToString());
            Assert.True(company.Positions.Contains((int)Position.TraineeEmployment), "Missing Industry. Expected: TrainingEmployment. Received: " + company.Positions.ToString());
            Assert.True(company.Positions.Count == 2, "Wrong number of Positions. Expected: 2. Received: " + company.Positions.Count);
        }

        [Fact]
        public async Task Get()
        {
            // Non logged in
            var application = new WebApplicationFactory<Program>();
            var client = application.CreateClient();

            // Get response
            var response = await client.GetAsync("/api/companies/-4");
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), "Wrong StatusCode. Expected: OK. Received: " + response.StatusCode.ToString());

            // Check result
            var company = JsonConvert.DeserializeObject<PublicCompanyDTO>(await response.Content.ReadAsStringAsync());

            // Verify result
            Assert.True(company.Name.Equals("Facebook"), "Wrong company name. Expected: Facebook. Received: " + company.Name);
            Assert.True(company.Description.Equals("We have friends in common"), "Wrong description. Received: " + company.Description);
            Assert.True(company.DidYouKnow.Equals("Mark zuckerburg is an Alien"), "Wrong DidYouKnow. Received: " + company.DidYouKnow);

            Assert.True(company.DaysAtArkad.Count == 2, "Wrong, Received: " + company.DaysAtArkad.Count);
            Assert.True(company.DaysAtArkad.Contains(DateTime.Parse("2023-01-01 00:00")), "Wrong, Received: " + company.DaysAtArkad[0]);
            Assert.True(company.DaysAtArkad.Contains(DateTime.Parse("2023-01-02 00:00")), "Wrong, Received: " + company.DaysAtArkad[1]);


            Assert.True(company.DesiredDegrees.Contains((int)Degree.PhD), "Missing Desired Degree. Expected: PhD. Received: " + company.DesiredDegrees.ToString());
            Assert.True(company.DesiredDegrees.Contains((int)Degree.Master), "Missing Desired Degree. Expected: Master. Received: " + company.DesiredDegrees.ToString());
            Assert.True(company.DesiredDegrees.Count == 2, "Wrong number of Desired Degrees. Expected: 2. Received: " + company.DesiredDegrees.Count);

            Assert.True(company.DesiredProgramme.Contains((int)Programme.Byggteknik_med_Järnvägsteknik), "Missing Desired Programme. Expected: Byggteknik_med_Järnvägsteknik. Received: " + company.DesiredProgramme.ToString());
            Assert.True(company.DesiredProgramme.Contains((int)Programme.Teknisk_Fysik), "Missing Desired Programme. Expected: Teknisk_Fysik. Received: " + company.DesiredProgramme.ToString());
            Assert.True(company.DesiredProgramme.Count == 2, "Wrong number of Desired Programmes. Expected: 2. Received: " + company.DesiredProgramme.Count);

            Assert.True(company.Industries.Contains((int)Industry.Environment), "Missing Industry. Expected: Environment. Received: " + company.Industries.ToString());
            Assert.True(company.Industries.Contains((int)Industry.ElectricityEnergyPower), "Missing Industry. Expected: ElectricityEnergyPower. Received: " + company.Industries.ToString());
            Assert.True(company.Industries.Count == 2, "Wrong number of Industries. Expected: 2. Received: " + company.Industries.Count);

            Assert.True(company.Positions.Contains((int)Position.Thesis), "Missing Position. Expected: Thesis. Received: " + company.Positions.ToString());
            Assert.True(company.Positions.Contains((int)Position.TraineeEmployment), "Missing Industry. Expected: TrainingEmployment. Received: " + company.Positions.ToString());
            Assert.True(company.Positions.Count == 2, "Wrong number of Positions. Expected: 2. Received: " + company.Positions.Count);

            Assert.True(company.StudentSessionMotivation.Equals("We are better than Apple!"), "Wrong StudentSessionMotivation. Received: " + company.StudentSessionMotivation);
        }

        [Fact]
        public async Task GetNonExisting()
        {
            // Non logged in
            var application = new WebApplicationFactory<Program>();
            var client = application.CreateClient();
            var response = await client.GetAsync("/api/companies/-123");
        
            // Verify response - -123 is not in the database
            Assert.True(response.StatusCode.Equals(HttpStatusCode.NotFound), "Wrong StatusCode. Expected: NotFound. Received: " + response.StatusCode.ToString());
        }


        [Fact]
        public async Task GetMe()
        {
            // Login
            var client = await TestUtils.Login("company1");

            // Get response
            var response = await client.GetAsync("/api/companies/me");
            var responseObject = JsonConvert.DeserializeObject<PublicCompanyDTO>(await response.Content.ReadAsStringAsync());

            // Verify response
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), "Wrong Status Code. Expected: OK. Received: " + response.StatusCode.ToString());
            Assert.True(responseObject.Name.Equals("Apple"), "Wrong company name. Expected: Apple. Received: " + responseObject.Name);
            Assert.True(responseObject.Description.Equals("A fruit company"), "Wromg Description. Received: " + responseObject.Description);
            Assert.True(responseObject.DidYouKnow.Equals("Apples"), "Wrong DidYouKnow. Received: " + responseObject.DidYouKnow);
            
            
            Assert.True(responseObject.DaysAtArkad.Count == 2, "Wrong, Received: " + responseObject.DaysAtArkad.Count);
            Assert.True(responseObject.DaysAtArkad.Contains(DateTime.Parse("2023-01-01 00:00")), "Wrong, Received: " + responseObject.DaysAtArkad[0]);
            Assert.True(responseObject.DaysAtArkad.Contains(DateTime.Parse("2023-01-02 00:00")), "Wrong, Received: " + responseObject.DaysAtArkad[1]);
      
      
        }

        [Fact]
        public async Task GetMeForbidden()
        {
            // Login
            var client = await TestUtils.Login("admin");

            // Get response
            var response = await client.GetAsync("/api/companies/me");
            string responseText = await response.Content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject<PublicCompanyDTO>(responseText);

            // Verify response - Admin is not a company
            Assert.True(response.StatusCode.Equals(HttpStatusCode.Forbidden), "Wrong Status Code. Expected: Forbidden. Received: " + response.StatusCode.ToString());
            Assert.True(responseObject == null, "Returned Object was not null. Received: " + responseText);
        }


        [Fact]
        public async Task Put()
        {
            // Setup
            var client = await TestUtils.Login("admin");
            var json = new JsonObject
            {
                { "description", "New description" }
            };

            // Update Description
            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await client.PutAsync("/api/companies/-3", payload);
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), "Wrong Status Code. Expected: OK. Received: " + response.StatusCode.ToString());

            // Restore
            json.Remove("description");
            json.Add("description", "We like music");
            var response2 = await client.PutAsync("/api/companies/-3", new StringContent(json.ToString(), Encoding.UTF8, "application/json"));

            // Verify response
            Assert.True(response2.StatusCode.Equals(HttpStatusCode.OK), "Wrong Status Code. Expected: OK. Received: " + response2.StatusCode.ToString());

            // Get response
            var responseObject = JsonConvert.DeserializeObject<PublicCompanyDTO>(await response.Content.ReadAsStringAsync());
            var responseObject2 = JsonConvert.DeserializeObject<PublicCompanyDTO>(await response2.Content.ReadAsStringAsync());

            // Verify response
            Assert.True(responseObject.Description.Equals("New description"), $"Wrong Description. Description was actually ({responseObject.Description})");
            Assert.True(responseObject2.Description.Equals("We like music"), $"Wrong Description. Description was actually ({responseObject2.Description})");


        }

        [Fact]
        public async Task PutForbidden()
        {
            // Setup
            var client = await TestUtils.Login("company1");
            var json = new JsonObject
            {
                { "description", "None" }
            };

            // Update Description
            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await client.PutAsync("/api/companies/-1", payload);

            // Verify response - Company1 is not admin
            Assert.True(response.StatusCode.Equals(HttpStatusCode.Forbidden), "Wrong Status Code. Expected: Forbidden. Received: " + response.StatusCode.ToString());

            // Get response
            string responseText = await response.Content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject<PublicCompanyDTO>(responseText);

            // Verify response
            Assert.True(responseObject == null, "Returned Object was not null. Received: " + responseText);
        }

        [Fact]
        public async Task PutNonExisting()
        {
            // Setup
            var client = await TestUtils.Login("admin");
            var json = new JsonObject
            {
                { "description", "None" }
            };

            // Update Description
            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await client.PutAsync("/api/companies/-22", payload);

            // Verify response - -22 is not in the database
            Assert.True(response.StatusCode.Equals(HttpStatusCode.NotFound), "Wrong Status Code. Expected: NotFound. Received: " +  response.StatusCode.ToString());
        }

        [Fact]
        public async Task PutMe()
        {
            // Setup
            var client = await TestUtils.Login("company2");
            var json = new JsonObject
            {
                { "description", "New description" }
            };

            // Update Description
            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await client.PutAsync("/api/companies/me", payload);

            // Verify response
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), "Wrong Status Code. Expected: OK. Received: " + response.StatusCode.ToString());

            // Restore
            json.Remove("description");
            json.Add("description", "You can find more about us by searching the web");
            var response2 = await client.PutAsync("/api/companies/me", new StringContent(json.ToString(), Encoding.UTF8, "application/json"));
            
            // Verify restore
            Assert.True(response2.StatusCode.Equals(HttpStatusCode.OK), "Wrong Status Code. Expected: OK. Received: " + response2.StatusCode.ToString());

            // Get response
            var responseObject = JsonConvert.DeserializeObject<PublicCompanyDTO>(await response.Content.ReadAsStringAsync());
            var responseObject2 = JsonConvert.DeserializeObject<PublicCompanyDTO>(await response2.Content.ReadAsStringAsync());

            // Verify changes
            Assert.True(responseObject.Description.Equals("New description"), $"Wrong Description. Description was actually ({responseObject.Description})");
            Assert.True(responseObject2.Description.Equals("You can find more about us by searching the web"), $"Description was actually ({responseObject2.Description})");
        }

        [Fact]
        public async Task PutMeForbidden()
        {
            // Setup
            var client = await TestUtils.Login("admin");
            var json = new JsonObject
            {
                { "description", "None" }
            };

            // Update Description
            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await client.PutAsync("/api/companies/me", payload);

            // Verify response - Admin is not a company
            Assert.True(response.StatusCode.Equals(HttpStatusCode.Forbidden), "Wrong Status Code. Expected: Forbidden. Received: " + response.StatusCode.ToString());

            // Verify response
            string responseText = await response.Content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject<PublicCompanyDTO>(responseText);

            Assert.True(responseObject == null, "Returned Object was not null. Received: " +  responseText);
        }

        [Fact]
        public async Task PostThenDelete()
        {
            // Setup
            var client = await TestUtils.Login("admin");
            var json = new JsonObject
            {
                { "description", "We produce the best and brightest." },
                { "name", "LTH" }
            };

            // Create Company
            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("/api/companies/", payload);

            // Verify response
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), "Wrong Status Code. Expected: OK. Received: " + response.StatusCode.ToString());

            // Read response
            var responseObject = JsonConvert.DeserializeObject<Company>(await response.Content.ReadAsStringAsync());
            var id = responseObject.Id;

            // Verify response
            response = await client.GetAsync("/api/companies/");
            var responseObject2 = JsonConvert.DeserializeObject<List<Company>>(await response.Content.ReadAsStringAsync());
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), "Wrong Status Code. Expected: OK. Received: " + response.StatusCode.ToString());

            // Delete Company
            response = await client.DeleteAsync("/api/companies/LTH");
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), "Wrong Status Code. Expected: OK. Received: " + response.StatusCode.ToString());

            // Verify delete
            response = await client.GetAsync("/api/companies/" + id);
            Assert.True(response.StatusCode.Equals(HttpStatusCode.NotFound), "Wrong Status Code. Expected: NotFound. Received: " + response.StatusCode.ToString());

            // Verify delete
            response = await client.GetAsync("/api/companies/");
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), "Wrong Status Code. Expected: OK. Received: " + response.StatusCode.ToString());

            // Verify delete
            var responseObject3 = JsonConvert.DeserializeObject<List<Company>>(await response.Content.ReadAsStringAsync());
            Assert.True(responseObject.Description.Equals("We produce the best and brightest."), "Wrong Description. Received: " + responseObject.Description);
            Assert.True(responseObject2.Count == 5, "Wrong number of companies. Expected: 5. Received: " + responseObject2.Count);
            Assert.True(responseObject3.Count == 4, "Wrong number of companies. Expected: 4. Received: " + responseObject3.Count);
        }
    }
}

//               { "start", DateTime.Parse("2021-11-15 12:45") },
//               { "end", DateTime.Parse("2021-11-15 13:15") },