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


namespace Nexpo.Tests.Controllers
{
    public class ContactControllerTests
    {
        [Fact]
        public async Task adminCreateAndDeleteContact()
        {
            //Setup
            var client = await TestUtils.Login("admin");

            //Create contact
            var DTO = new CreateContactDTO()
            {
                FirstName = "Test",
                LastName = "Testsson",
                PhoneNumber = "123-456 78 90",
                Email = "test.testsson@example.com",
                RoleInArkad = "Tester"
            };

            //get number of contacts
            var getResponse = await client.GetAsync("/api/contacts/");
            var contacts = JsonConvert.DeserializeObject<List<Contact>>(await getResponse.Content.ReadAsStringAsync());
            var numberOfContacts = contacts.Count;

            var json = JsonConvert.SerializeObject(DTO);
            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("/api/contacts/add", payload);
            Assert.True(response.StatusCode.Equals(HttpStatusCode.Created), "Wrong Status Code. Expected: Created. Received: " + response.StatusCode.ToString());

            //Deserialize response
            var responseContact = JsonConvert.DeserializeObject<Contact>(await response.Content.ReadAsStringAsync());

            //Check contact
            Assert.True(responseContact.FirstName == "Test", "Wrong first name. Expected: Test. Received: " + responseContact.FirstName);
            Assert.True(responseContact.PhoneNumber == "123-456 78 90", "Wrong phone number. Expected: 123-456 78 90. Received: " + responseContact.PhoneNumber);

            //Check that number of contacts has increased
            getResponse = await client.GetAsync("/api/contacts/");
            contacts = JsonConvert.DeserializeObject<List<Contact>>(await getResponse.Content.ReadAsStringAsync());
            Assert.True(contacts.Count == numberOfContacts + 1, "Wrong number of contacts. Expected: " + (numberOfContacts + 1) + ". Received: " + contacts.Count);

            //Delete contact
            var deleteResponse = await client.DeleteAsync("/api/contacts/" + responseContact.Id);
            Assert.True(deleteResponse.StatusCode.Equals(HttpStatusCode.NoContent), "Wrong Status Code. Expected: NoContent. Received: " + deleteResponse.StatusCode.ToString());

            //Check that contact is deleted
            getResponse = await client.GetAsync("/api/contacts/" + responseContact.Id);
            Assert.True(getResponse.StatusCode.Equals(HttpStatusCode.NotFound), "Wrong Status Code. Expected: NotFound. Received: " + getResponse.StatusCode.ToString());

            //Check that number of contacts has decreased
            getResponse = await client.GetAsync("/api/contacts/");
            contacts = JsonConvert.DeserializeObject<List<Contact>>(await getResponse.Content.ReadAsStringAsync());
            Assert.True(contacts.Count == numberOfContacts, "Wrong number of contacts. Expected: " + numberOfContacts + ". Received: " + contacts.Count);

        }

        [Fact]
        public async Task addingDuplicateContact()
        {
            //Setup
            var client = await TestUtils.Login("admin");

            //Create contact
            var DTO = new CreateContactDTO()
            {
                FirstName = "Back",
                LastName = "End",
                PhoneNumber = "004-444 44 44",
                Email = "contact4@example.com",
                RoleInArkad = "Backend Manager"
            };

            //add duplicate contact
            var json = JsonConvert.SerializeObject(DTO);
            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("/api/contacts/add", payload);
            Assert.True(response.StatusCode.Equals(HttpStatusCode.Conflict), "Wrong Status Code. Expected: Conflict. Received: " + response.StatusCode.ToString());
        }

        [Fact]
        public async Task volunteerCreateContact()
        {
            //Login
            var client = await TestUtils.Login("volunteer");

            var DTO = new CreateContactDTO()
            {
                FirstName = "Test",
                LastName = "Testsson",
                PhoneNumber = "123-456 78 90",
                Email = "test.testsson@example.com",
                RoleInArkad = "Tester"
            };

            //serialize json
            var payload = new StringContent(DTO.ToString(), Encoding.UTF8, "application/json");

            //Get contact and check status code
            var response = await client.PutAsync("/api/contacts/add", payload);
            Assert.True(response.StatusCode.Equals(HttpStatusCode.Forbidden), "Wrong Status Code. Expected: Forbidden. Received: " + response.StatusCode.ToString());
        }

        [Fact]
        public async Task volunteerGetContact()
        {
            //Login
            var client = await TestUtils.Login("volunteer");

            //Get contact and check status code
            var response = await client.GetAsync("/api/contacts/-3");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            //Deserialize response
            var responseContact = JsonConvert.DeserializeObject<Contact>(await response.Content.ReadAsStringAsync());

            //Check contact
            Assert.True(responseContact.FirstName == "Front", "Wrong first name. Expected: Front. Received: " + responseContact.FirstName);
            Assert.True(responseContact.PhoneNumber == "003-333 33 33", "Wrong phone number. Expected: 003-333 33 33. Received: " + responseContact.PhoneNumber);
        }

        [Fact]
        public async Task companyHostGetContact()
        {
            // Make sure that CompanyHosts can access contacts, even though it's only explicitly authorized for Volunteers

            //Login as companyHost
            var client = await TestUtils.Login("companyhost");

            //Get contact and simply check status code
            var response = await client.GetAsync("/api/contacts/-3");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task companyHostPutContact()
        {
            //Login as companyHost
            var client = await TestUtils.Login("companyhost");

            var DTO = new CreateContactDTO()
            {
                FirstName = "Test",
                LastName = "Testsson",
                PhoneNumber = "123-456 78 90",
                Email = "test.testsson@example.com",
                RoleInArkad = "Tester"
            };

            //serialize json
            var payload = new StringContent(DTO.ToString(), Encoding.UTF8, "application/json");

            //Get contact and check status code
            var response = await client.PutAsync("/api/contacts/add", payload);

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);

        }
        

        [Fact]
        public async Task nonAuthorizedGetContact()
        {
            //Login
            var client = await TestUtils.Login("student1");

            //Get contact and check status code
            var response = await client.GetAsync("/api/contacts/-1");
            Assert.True(response.StatusCode.Equals(HttpStatusCode.Forbidden), "Wrong Status Code. Expected: Forbidden. Received: " + response.StatusCode.ToString());
        }

        [Fact]
        public async Task retrieveNonExistingContact()
        {
            //Login
            var client = await TestUtils.Login("volunteer");

            //Get contact and check status code
            var response = await client.GetAsync("/api/contacts/-100");
            Assert.True(response.StatusCode.Equals(HttpStatusCode.NotFound), "Wrong Status Code. Expected: NotFound. Received: " + response.StatusCode.ToString());

        }

        [Fact]
        public async Task getAllContactsAsVolunteerTest()
        {
            //Login
            var client = await TestUtils.Login("volunteer");

            //Get all contacts amd check status code
            var response = await client.GetAsync("/api/contacts");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            //Deserialize response
            var responseList = JsonConvert.DeserializeObject<List<Contact>>(await response.Content.ReadAsStringAsync());
            Assert.True(responseList.Count == 4, "Wrong number of contacts. Expected: 4. Received: " + responseList.Count);

            //Sample results
            var firstContact = responseList.Find(contacts => contacts.Id == -1);
            var secondContact = responseList.Find(contacts => contacts.Id == -2);

            //Check contact 1
            Assert.True(firstContact.FirstName == "PL", "Wrong first name. Expected: PL. Received: " + firstContact.FirstName);
            Assert.True(firstContact.PhoneNumber == "001-111 11 11", "Wrong phone number. Expected: 001-111 11 11. Received: " + firstContact.PhoneNumber);

            //Check contact 2
            Assert.True(secondContact.LastName == "Van IT", "Wrong last name. Expected: Van It. Received: " + secondContact.LastName);
            Assert.True(secondContact.Email == "contact2@example.com", "Wrong email. Expected: contact2@example.com. Received: " + secondContact.Email);
            Assert.True(secondContact.RoleInArkad == "Head of IT", "Wrong role in arkad. Expected: Head of IT. Received: " + secondContact.RoleInArkad);
        }

        [Fact]
        public async Task getAllContactsAsAdminTest()
        {
            //Login
            var client = await TestUtils.Login("admin");

            //Get all contacts anc check status code
            var response = await client.GetAsync("/api/contacts");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            //Deserialize response
            var responseList = JsonConvert.DeserializeObject<List<Contact>>(await response.Content.ReadAsStringAsync());

            //Sample results
            var thirdContact = responseList.Find(contacts => contacts.Id == -3);
            var fourthContact = responseList.Find(contacts => contacts.Id == -4);

            //Check contact 3
            Assert.True(thirdContact.FirstName == "Front", "Wrong first name. Expected: Front. Received: " + thirdContact.FirstName);
            Assert.True(thirdContact.PhoneNumber == "003-333 33 33", "Wrong phone number. Expected: 003-333 33 33. Received: " + thirdContact.PhoneNumber);

            //Check contact 4
            Assert.True(fourthContact.LastName == "End", "Wrong last name. Expected: End. Received: " + fourthContact.LastName);
            Assert.True(fourthContact.Email == "contact4@example.com", "Wrong email. Expected: contact4@example.com. Received: " + fourthContact.Email);
            Assert.True(fourthContact.RoleInArkad == "Backend Manager", "Wrong role in arkad. Expected: Backend Manager. Received: " + fourthContact.RoleInArkad);
        }

        [Fact]
        public async Task getAllContactsAsStudentTest()
        {
            //Login
            var client = await TestUtils.Login("student1");

            //Disallow access for student
            var response = await client.GetAsync("/api/contacts");
            Assert.True(response.StatusCode.Equals(HttpStatusCode.Forbidden), "Wrong status code. Expected: Forbidden. Received: " + response.StatusCode.ToString());

        }

        [Fact]
        public async Task getAllContactsWhileNotLoggedInTest()
        {
            //Create program and client
            var application = new WebApplicationFactory<Program>();
            var client = application.CreateClient();

            //Disallowed access for not being logged in
            var response = await client.GetAsync("/api/contacts");
            Assert.True(response.StatusCode.Equals(HttpStatusCode.Unauthorized), "Wrong status code. Expected: Unauthorized. Received: " + response.StatusCode.ToString());
        }

        [Fact]
        public async Task NonAdminUpdateContactTest()
        {
            //Login
            var client = await TestUtils.Login("volunteer");

            //Update information
            var json = new JsonObject
            {
                { "firstName", "This is" },
                { "lastName", "not allowed" },
                { "phoneNumber", "000-000 00 00" },
                { "email", "not.ok@example.com" },
                { "roleInArkad", "This is not allowed" }
            };

            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await client.PutAsync("api/contacts/-2", payload);

            Assert.True(response.StatusCode.Equals(HttpStatusCode.Forbidden), "Wrong status code. Expected: Forbidden. Received: " + response.StatusCode.ToString());
        }

        [Fact]
        public async Task updateContactAsAdminTest()
        {
            //Login
            var application = new WebApplicationFactory<Program>();
            var client = await TestUtils.Login("admin");

            //Update information
            var json = new JsonObject
            {
                { "firstName", "Other" },
                { "lastName", "Contact" },
                { "phoneNumber", "000-000 00 00" },
                { "email", "other.contact@example.com" },
                { "roleInArkad", "Other Role" }
            };

            // Verify that the contact is updated
            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await client.PutAsync("api/contacts/-2", payload);
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), "Wrong Status Code. Expected: OK. Received: " + response.ToString());

            //Restore information
            var json2 = new JsonObject
            {
                { "firstName", "Head" },
                { "lastName", "Van IT" },
                { "phoneNumber", "002-222 22 22" },
                { "email", "contact2@example.com" },
                { "roleInArkad", "Head of IT" }
            };

            // Restore from information
            var payload2 = new StringContent(json2.ToString(), Encoding.UTF8, "application/json");
            var response2 = await client.PutAsync("api/contacts/-2", payload2);
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), "Wrong status code. Expected: OK. Received: " + response2.StatusCode.ToString());

            var responseObject = JsonConvert.DeserializeObject<Contact>(await response.Content.ReadAsStringAsync());
            var responseObject2 = JsonConvert.DeserializeObject<Contact>(await response2.Content.ReadAsStringAsync());

            //Assertions of response
            Assert.True(responseObject.Id == -2, "Wrong Contact. Expected: -2. Received: " + responseObject.Id.ToString());
            Assert.True(responseObject.FirstName == "Other", "Wrong first name. Expected: Other. Received: " + responseObject.FirstName);
            Assert.True(responseObject.LastName == "Contact", "Wrong last name. Expected: Contact. Received: " + responseObject.LastName);
            Assert.True(responseObject.PhoneNumber == "000-000 00 00", "Wrong phone number. Expected: 000-000 00 00. Received: " + responseObject.PhoneNumber);
            Assert.True(responseObject.Email == "other.contact@example.com", "Wrong email. Expected: other.contact@example.com. Received: " + responseObject.Email);
            Assert.True(responseObject.RoleInArkad == "Other Role", "Wrong role in arkad. Expected: Other Role. Received: " + responseObject.RoleInArkad);

            //Assertions of restoration
            Assert.True(responseObject2.Id == -2, "Wrong Contact. Expected: -2. Received: " + responseObject2.Id.ToString());
            Assert.True(responseObject2.FirstName == "Head", "Wrong first name. Expected: Head. Received: " + responseObject2.FirstName);
            Assert.True(responseObject2.LastName == "Van IT", "Wrong last name. Expected: Van IT. Received: " + responseObject2.LastName);
            Assert.True(responseObject2.PhoneNumber == "002-222 22 22", "Wrong phone number. Expected: 002-222 22 22. Received: " + responseObject2.PhoneNumber);
            Assert.True(responseObject2.Email == "contact2@example.com", "Wrong email. Expected: contact2@example.com. Received: " + responseObject2.Email);
            Assert.True(responseObject2.RoleInArkad == "Head of IT", "Wrong role in arkad. Expected: Head of IT. Received: " + responseObject2.RoleInArkad);
        }
    }
}