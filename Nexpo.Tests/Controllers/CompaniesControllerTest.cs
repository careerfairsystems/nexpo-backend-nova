using Microsoft.AspNetCore.Mvc.Testing;
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
        public async Task GetAllCompanies()
        {
            var application = new WebApplicationFactory<Nexpo.Program>();
            var client = application.CreateClient();
            var response = await client.GetAsync("/api/companies/");

            string responseText = await response.Content.ReadAsStringAsync();
            var responseList = JsonConvert.DeserializeObject<List<PublicCompanyDto>>(responseText);

            var company = responseList.Find(r => r.Id == -4);

            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), "Wrong StatusCode. Expected: OK. Recieved: " + response.StatusCode.ToString());
            Assert.True(responseList.Count == 4, "Wrong number of companies. Expected: 4. Recieved: " + responseList.Count);
            Assert.True(company.Name.Equals("Facebook"), "Wrong company name. Expected: Facebook. Recieved: " + company.Name);
            Assert.True(company.Description.Equals("We have friends in common"), "Wrong description. Recieved: " + company.Description);
            Assert.True(company.DidYouKnow.Equals("Mark zuckerburg is an Alien"), "Wrong DidYouKnow. Recieved: " + company.DidYouKnow);
            Assert.True(company.DesiredDegrees.Contains((int) Degree.PhD), "Missing Desired Degree. Expected: PhD. Recieved: " + company.DesiredDegrees.ToString());
            Assert.True(company.DesiredDegrees.Contains((int)Degree.Master), "Missing Desired Degree. Expected: Master. Recieved: " + company.DesiredDegrees.ToString());
            Assert.True(company.DesiredDegrees.Count == 2, "Wrong number of Desired Degrees. Expected: 2. Recieved: " + company.DesiredDegrees.Count);
            Assert.True(company.DesiredProgramme.Contains((int)Programme.Byggteknik_med_Järnvägsteknik), "Missing Desired Programme. Expected: Byggteknik_med_Järnvägsteknik. Recieved: " + company.DesiredProgramme.ToString());
            Assert.True(company.DesiredProgramme.Contains((int)Programme.Teknisk_Fysik), "Missing Desired Programme. Expected: Teknisk_Fysik. Recieved: " + company.DesiredProgramme.ToString());
            Assert.True(company.DesiredProgramme.Count == 2, "Wrong number of Desired Programmes. Expected: 2. Recieved: " + company.DesiredProgramme.Count);
            Assert.True(company.Industries.Contains((int)Industry.Environment), "Missing Industry. Expected: Environment. Recieved: " + company.Industries.ToString());
            Assert.True(company.Industries.Contains((int)Industry.ElectricityEnergyPower), "Missing Industry. Expected: ElectricityEnergyPower. Recieved: " + company.Industries.ToString());
            Assert.True(company.Industries.Count == 2, "Wrong number of Industries. Expected: 2. Recieved: " + company.Industries.Count);
            Assert.True(company.Positions.Contains((int)Position.Thesis), "Missing Position. Expected: Thesis. Recieved: " + company.Positions.ToString());
            Assert.True(company.Positions.Contains((int)Position.TraineeEmployment), "Missing Industry. Expected: TrainingEmployment. Recieved: " + company.Positions.ToString());
            Assert.True(company.Positions.Count == 2, "Wrong number of Positions. Expected: 2. Recieved: " + company.Positions.Count);
        }

        [Fact]
        public async Task GetCompanySuccesful()
        {
            var application = new WebApplicationFactory<Nexpo.Program>();
            var client = application.CreateClient();
            var response = await client.GetAsync("/api/companies/-4");

            string responseText = await response.Content.ReadAsStringAsync();
            var company = JsonConvert.DeserializeObject<PublicCompanyDto>(responseText);

            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), "Wrong StatusCode. Expected: OK. Recieved: " + response.StatusCode.ToString());
            Assert.True(company.Name.Equals("Facebook"), "Wrong company name. Expected: Facebook. Recieved: " + company.Name);
            Assert.True(company.Description.Equals("We have friends in common"), "Wrong description. Recieved: " + company.Description);
            Assert.True(company.DidYouKnow.Equals("Mark zuckerburg is an Alien"), "Wrong DidYouKnow. Recieved: " + company.DidYouKnow);
            Assert.True(company.DesiredDegrees.Contains((int)Degree.PhD), "Missing Desired Degree. Expected: PhD. Recieved: " + company.DesiredDegrees.ToString());
            Assert.True(company.DesiredDegrees.Contains((int)Degree.Master), "Missing Desired Degree. Expected: Master. Recieved: " + company.DesiredDegrees.ToString());
            Assert.True(company.DesiredDegrees.Count == 2, "Wrong number of Desired Degrees. Expected: 2. Recieved: " + company.DesiredDegrees.Count);
            Assert.True(company.DesiredProgramme.Contains((int)Programme.Byggteknik_med_Järnvägsteknik), "Missing Desired Programme. Expected: Byggteknik_med_Järnvägsteknik. Recieved: " + company.DesiredProgramme.ToString());
            Assert.True(company.DesiredProgramme.Contains((int)Programme.Teknisk_Fysik), "Missing Desired Programme. Expected: Teknisk_Fysik. Recieved: " + company.DesiredProgramme.ToString());
            Assert.True(company.DesiredProgramme.Count == 2, "Wrong number of Desired Programmes. Expected: 2. Recieved: " + company.DesiredProgramme.Count);
            Assert.True(company.Industries.Contains((int)Industry.Environment), "Missing Industry. Expected: Environment. Recieved: " + company.Industries.ToString());
            Assert.True(company.Industries.Contains((int)Industry.ElectricityEnergyPower), "Missing Industry. Expected: ElectricityEnergyPower. Recieved: " + company.Industries.ToString());
            Assert.True(company.Industries.Count == 2, "Wrong number of Industries. Expected: 2. Recieved: " + company.Industries.Count);
            Assert.True(company.Positions.Contains((int)Position.Thesis), "Missing Position. Expected: Thesis. Recieved: " + company.Positions.ToString());
            Assert.True(company.Positions.Contains((int)Position.TraineeEmployment), "Missing Industry. Expected: TrainingEmployment. Recieved: " + company.Positions.ToString());
            Assert.True(company.Positions.Count == 2, "Wrong number of Positions. Expected: 2. Recieved: " + company.Positions.Count);
            Assert.True(company.StudentSessionMotivation.Equals("We are better than Apple!"), "Wrong StudentSessionMotivation. Recieved: " + company.StudentSessionMotivation);
        }

        [Fact]
        public async Task GetCompanyFailure()
        {
            var application = new WebApplicationFactory<Nexpo.Program>();
            var client = application.CreateClient();
            var response = await client.GetAsync("/api/companies/-22");
         
            Assert.True(response.StatusCode.Equals(HttpStatusCode.NotFound), "Wrong StatusCode. Recieved: " + response.StatusCode.ToString());
        }

        [Fact]
        public async Task PutCompany()
        {
            //Setup
            var client = await TestUtils.Login("admin");
            var dto = new UpdateCompanyDto();
            dto.Description = "New description";
            
            //Update Description
            var json = new JsonObject();
            json.Add("description", dto.Description);
            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await client.PutAsync("/api/companies/-3", payload);
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), "Wrong Status Code. Expected: OK. Recieved: " + response.StatusCode.ToString());

            //Restore
            json.Remove("description");
            json.Add("description", "We like music");
            var response2 = await client.PutAsync("/api/companies/-3", new StringContent(json.ToString(), Encoding.UTF8, "application/json"));
            Assert.True(response2.StatusCode.Equals(HttpStatusCode.OK), "Wrong Status Code. Expected: OK. Recieved: " + response2.StatusCode.ToString());

            //Verify
            string responseText = await response.Content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject<PublicCompanyDto>(responseText);

            string responseText2 = await response2.Content.ReadAsStringAsync();
            var responseObject2 = JsonConvert.DeserializeObject<PublicCompanyDto>(responseText2);

            Assert.True(responseObject.Description.Equals("New description"), $"Wrong Description. Description was actually ({responseObject.Description})");
            Assert.True(responseObject2.Description.Equals("We like music"), $"Wrong Description. Description was actually ({responseObject2.Description})");


        }

        [Fact]
        public async Task PutCompanyForbidden()
        {
            var client = await TestUtils.Login("company1");
            var dto = new UpdateCompanyDto();
            dto.Description = "None";
            
            var json = new JsonObject();
            json.Add("description", dto.Description);
            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await client.PutAsync("/api/companies/-1", payload);
            Assert.True(response.StatusCode.Equals(HttpStatusCode.Forbidden), "Wrong Status Code. Expected: Forbidden. Recieved: " + response.StatusCode.ToString());

            string responseText = await response.Content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject<PublicCompanyDto>(responseText);
            Assert.True(responseObject == null, "Returned Object was not null. Recieved: " + responseText);
        }

        [Fact]
        public async Task PutNonExistingCompany()
        {
            var client = await TestUtils.Login("admin");
            var dto = new UpdateCompanyDto();
            dto.Description = "None";
            
            var json = new JsonObject();
            json.Add("description", dto.Description);
            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await client.PutAsync("/api/companies/-22", payload);

            Assert.True(response.StatusCode.Equals(HttpStatusCode.NotFound), "Wrong Status Code. Expected: NotFound. Recieved: " +  response.StatusCode.ToString());
        }

        [Fact]
        public async Task GetMe()
        {
            var client = await TestUtils.Login("company1");
            var response = await client.GetAsync("/api/companies/me");

            string responseText = await response.Content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject<PublicCompanyDto>(responseText);

            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), "Wrong Status Code. Expected: OK. Recieved: " + response.StatusCode.ToString());
            Assert.True(responseObject.Name.Equals("Apple"), "Wrong company name. Expected: Apple. Recieved: " + responseObject.Name);
            Assert.True(responseObject.Description.Equals("A fruit company"), "Wromg Description. Recieved: " + responseObject.Description);
            Assert.True(responseObject.DidYouKnow.Equals("Apples"), "Wrong DidYouKnow. Recieved: " + responseObject.DidYouKnow);
        }

        [Fact]
        public async Task GetIllegalMe()
        {
            var client = await TestUtils.Login("admin");
            var response = await client.GetAsync("/api/companies/me");

            string responseText = await response.Content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject<PublicCompanyDto>(responseText);

            Assert.True(response.StatusCode.Equals(HttpStatusCode.Forbidden), "Wrong Status Code. Expected: Forbidden. Recieved: " + response.StatusCode.ToString());
            Assert.True(responseObject == null, "Returned Object was not null. Recieved: " + responseText);
        }

        [Fact]
        public async Task PutMe()
        {
            var client = await TestUtils.Login("company2");
            var dto = new UpdateCompanyDto();
            dto.Description = "New description";
            
            var json = new JsonObject();
            json.Add("description", dto.Description);
            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await client.PutAsync("/api/companies/me", payload);
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), "Wrong Status Code. Expected: OK. Recieved: " + response.StatusCode.ToString());

            json.Remove("description");
            json.Add("description", "You can find more about us by searching the web");
            var response2 = await client.PutAsync("/api/companies/me", new StringContent(json.ToString(), Encoding.UTF8, "application/json"));
            Assert.True(response2.StatusCode.Equals(HttpStatusCode.OK), "Wrong Status Code. Expected: OK. Recieved: " + response2.StatusCode.ToString());

            string responseText = await response.Content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject<PublicCompanyDto>(responseText);

            string responseText2 = await response2.Content.ReadAsStringAsync();
            var responseObject2 = JsonConvert.DeserializeObject<PublicCompanyDto>(responseText2);

            Assert.True(responseObject.Description.Equals("New description"), $"Wrong Description. Description was actually ({responseObject.Description})");
            Assert.True(responseObject2.Description.Equals("You can find more about us by searching the web"), $"Description was actually ({responseObject2.Description})");
        }

        [Fact]
        public async Task PutMeForbidden()
        {
            var client = await TestUtils.Login("admin");
            var dto = new UpdateCompanyDto();
            dto.Description = "None";
            
            var json = new JsonObject();
            json.Add("description", dto.Description);
            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await client.PutAsync("/api/companies/me", payload);
            Assert.True(response.StatusCode.Equals(HttpStatusCode.Forbidden), "Wrong Status Code. Expected: Forbidden. Recieved: " + response.StatusCode.ToString());

            string responseText = await response.Content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject<PublicCompanyDto>(responseText);
            Assert.True(responseObject == null, "Returned Object was not null. Recieved: " +  responseText);
        }

        [Fact]
        public async Task PostThenDelete()
        {
            var client = await TestUtils.Login("admin");
            
            var json = new JsonObject();
            json.Add("description", "We produce the best and brightest.");
            json.Add("name", "LTH");
            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("/api/companies/", payload);
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), "Wrong Status Code. Expected: OK. Recieved: " + response.StatusCode.ToString());

            string responseText = await response.Content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject<Company>(responseText);
            var id = responseObject.Id;

            response = await client.GetAsync("/api/companies/");
            string responseText2 = await response.Content.ReadAsStringAsync();
            var responseObject2 = JsonConvert.DeserializeObject<List<Company>>(responseText2);
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), "Wrong Status Code. Expected: OK. Recieved: " + response.StatusCode.ToString());

            response = await client.DeleteAsync("/api/companies/LTH");
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), "Wrong Status Code. Expected: OK. Recieved: " + response.StatusCode.ToString());

            response = await client.GetAsync("/api/companies/" + id);
            Assert.True(response.StatusCode.Equals(HttpStatusCode.NotFound), "Wrong Status Code. Expected: NotFound. Recieved: " + response.StatusCode.ToString());

            response = await client.GetAsync("/api/companies/");
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), "Wrong Status Code. Expected: OK. Recieved: " + response.StatusCode.ToString());
            string responseText3 = await response.Content.ReadAsStringAsync();
            var responseObject3 = JsonConvert.DeserializeObject<List<Company>>(responseText3);

            Assert.True(responseObject.Description.Equals("We produce the best and brightest."), "Wrong Description. Recieved: " + responseObject.Description);
            Assert.True(responseObject2.Count == 5, "Wrong number of companies. Expected: 5. Recieved: " + responseObject2.Count);
            Assert.True(responseObject3.Count == 4, "Wrong number of companies. Expected: 4. Recieved: " + responseObject3.Count);
        }
    }
}