using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Nexpo.Models;
using Nexpo.DTO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Nexpo.Tests.Controllers
{
    public class RoleControllerTest
    {
        [Fact]
        public async Task UpdateRoleAsAdmin()
        {
            var client = await TestUtils.Login("admin");

            var updateRoleDto = new UpdateRoleDTO
            {
                Role = Role.Volunteer
            };

            var json = JsonConvert.SerializeObject(updateRoleDto);
            var payload = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

            var response = await client.PutAsync("api/users/-5", payload);

            Assert.True(
                response.StatusCode.Equals(HttpStatusCode.OK),
                "Wrong status code. Expected: OK. Received: " + response.StatusCode.ToString()
            );

            var serializedUser = await response.Content.ReadAsStringAsync();
            var user = JsonConvert.DeserializeObject<User>(serializedUser);
            
            Assert.True(
                user.Role.Equals(Role.Volunteer), 
                "Wrong role. Expected: CompanyRepresentative. Received: " + user.Role.ToString()
            );

            var updateRoleDto2 = new UpdateUserDTO
            {
                Role = Role.CompanyRepresentative
            };

            var json2 = JsonConvert.SerializeObject(updateRoleDto2);
            var payload2 = new StringContent(json2, UnicodeEncoding.UTF8, "application/json");

            var response2 = await client.PutAsync("api/users/-5", payload2);
            Assert.True(
                response2.StatusCode.Equals(HttpStatusCode.OK), 
                "Wrong status code. Expected: OK. Received: " + response2.StatusCode.ToString()
            );

            var user2 = JsonConvert.DeserializeObject<User>(await response2.Content.ReadAsStringAsync());
            Assert.True(
                user2.Role.Equals(Role.CompanyRepresentative), 
                "Wrong role. Expected: CompanyRepresentative. Received: " + user2.Role.ToString()
            );

        }

        [Fact]
        public async Task UpdateRoleNonExistingUserAsAdmin(){
            var client = await TestUtils.Login("admin");
            var updateRoleDto = new UpdateRoleDTO
            {
                Role = Role.Volunteer
            };

            var json = JsonConvert.SerializeObject(updateRoleDto);
            var payload = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

            var response = await client.PutAsync("api/users/-100", payload);

            Assert.True(
                response.StatusCode.Equals(HttpStatusCode.NotFound),
                "Wrong status code. Expected: NotFound. Received: " + response.StatusCode.ToString()
            );
        }

        [Fact]
        public async Task UpdateRoleAsNonAdmin(){
            var client = await TestUtils.Login("student1");
            var updateRoleDto = new UpdateRoleDTO
            {
                Role = Role.Volunteer
            };

            var json = JsonConvert.SerializeObject(updateRoleDto);
            var payload = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

            var response = await client.PutAsync("api/users/-5", payload);

            Assert.True(
                response.StatusCode.Equals(HttpStatusCode.Forbidden),
                "Wrong status code. Expected: Forbidden. Received: " + response.StatusCode.ToString()
            );

            client = await TestUtils.Login("company1");
            updateRoleDto = new UpdateRoleDTO
            {
                Role = Role.Volunteer
            };

            json = JsonConvert.SerializeObject(updateRoleDto);
            payload = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

            response = await client.PutAsync("api/users/-5", payload);

            Assert.True(
                response.StatusCode.Equals(HttpStatusCode.Forbidden),
                "Wrong status code. Expected: Forbidden. Received: " + response.StatusCode.ToString()
            );
        }

        [Fact]
        public async Task UpdateRoleAsUnautherized()
        {
            var application = new WebApplicationFactory<Program>();
            var client = application.CreateClient();
            var updateRoleDto = new UpdateRoleDTO
            {
                Role = Role.Volunteer
            };

            var json = JsonConvert.SerializeObject(updateRoleDto);
            var payload = new StringContent(json, UnicodeEncoding.UTF8, "application/json");
            var response = await client.PutAsync("api/users/-5", payload);

            Assert.True(response.StatusCode.Equals(HttpStatusCode.Unauthorized), "Wrong Status Code. Expected: Unauthorized. Received: " + response.ToString());
        }
    }
}