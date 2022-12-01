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

namespace Nexpo.Tests.Controllers
{
    public class SessionControllerTest
    {
        [Fact]
        public async Task PostSignInWithCorrectCredentialsStudent()
        {
            var application = new WebApplicationFactory<Nexpo.Program>();
            var client = application.CreateClient();
            var json = new JsonObject();
            json.Add("email", "student1@example.com");
            json.Add("password", "password");

            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("/api/session/signin", payload);
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), "Login failed, returned: " + response.StatusCode.ToString());
        }

        [Fact]
        public async Task PostSignInWithCorrectCredentialsCompany()
        {
            var application = new WebApplicationFactory<Nexpo.Program>();
            var client = application.CreateClient();
            var json = new JsonObject();
            json.Add("email", "rep1@company1.example.com");
            json.Add("password", "password");

            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("/api/session/signin", payload);
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), "Login failed, returned: " + response.StatusCode.ToString());
        }

        [Fact]
        public async Task PostSignInWithCorrectCredentialsAdministrator()
        {
            var application = new WebApplicationFactory<Nexpo.Program>();
            var client = application.CreateClient();
            var json = new JsonObject();
            json.Add("email", "admin@example.com");
            json.Add("password", "password");

            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("/api/session/signin", payload);
            Assert.True(response.StatusCode.Equals(HttpStatusCode.OK), "Login failed, returned: " + response.StatusCode.ToString());
        }

        [Fact]
        public async Task PostSignInWithIncorrectPassword()
        {
            var application = new WebApplicationFactory<Nexpo.Program>();
            var client = application.CreateClient();
            var json = new JsonObject();
            json.Add("email", "student1@example.com");
            json.Add("password", "Password");

            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("/api/session/signin", payload);
            Assert.True(response.StatusCode.Equals(HttpStatusCode.BadRequest), "Unauthorized Login, returned: " + response.StatusCode.ToString());
        }

        [Fact]
        public async Task PostSignInWithIncorrectEmail()
        {
            var application = new WebApplicationFactory<Nexpo.Program>();
            var client = application.CreateClient();
            var json = new JsonObject();
            json.Add("email", "student1@example.se");
            json.Add("password", "password");

            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("/api/session/signin", payload);
            Assert.True(response.StatusCode.Equals(HttpStatusCode.BadRequest), "Unauthorized Login, returned: " + response.StatusCode.ToString());
        }

        [Fact]
        public async Task PostForgotPasswordShouldReturnNoContentCorrectEmail()
        {
            var application = new WebApplicationFactory<Nexpo.Program>();
            var client = application.CreateClient();
            var json = new JsonObject();
            json.Add("email", "student1@example.com");

            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("/api/session/forgot_password", payload);
            Assert.True(response.StatusCode.Equals(HttpStatusCode.NoContent), "Wrong return code, returned: " + response.StatusCode.ToString());
        }

        [Fact]
        public async Task PostForgotPasswordShouldReturnNoContentIncorrectEmail()
        {
            var application = new WebApplicationFactory<Nexpo.Program>();
            var client = application.CreateClient();
            var json = new JsonObject();
            json.Add("email", "student1@example.se");

            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("/api/session/forgot_password", payload);
            Assert.True(response.StatusCode.Equals(HttpStatusCode.NoContent), "Wrong return code, returned: " + response.StatusCode.ToString());
        }

        [Fact]
        public async Task PostResetPasswordWithIllegitToken()
        {
            var application = new WebApplicationFactory<Nexpo.Program>();
            var client = application.CreateClient();
            var json = new JsonObject();
            json.Add("email", "student1@example.com");
            json.Add("password", "password");

            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("/api/session/signin", payload);

            string token = new StreamReader(response.Content.ReadAsStream()).ReadToEnd();
            var parser = JObject.Parse(token);
            token = "Bearer " + parser.Value<String>("token");
            client.DefaultRequestHeaders.Add("Authorization", token);

            var json2 = new JsonObject();
            json2.Add("token", token);
            json2.Add("password", "newPassword123");

            var payload2 = new StringContent(json2.ToString(), Encoding.UTF8, "application/json");
            var response2 = await client.PostAsync("/api/session/reset_password", payload2);
            Assert.True(response2.StatusCode.Equals(HttpStatusCode.Forbidden), "Unvalid response, returned: " + response2.ToString());
        }

        [Fact]
        public async Task PostResetPasswordWithIllegitTokenAndWeakPassword()
        {
            var application = new WebApplicationFactory<Nexpo.Program>();
            var client = application.CreateClient();
            var json = new JsonObject();
            json.Add("email", "student1@example.com");
            json.Add("password", "password");

            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("/api/session/signin", payload);

            string token = new StreamReader(response.Content.ReadAsStream()).ReadToEnd();
            var parser = JObject.Parse(token);
            token = "Bearer " + parser.Value<String>("token");
            client.DefaultRequestHeaders.Add("Authorization", token);

            var json2 = new JsonObject();
            json2.Add("token", token);
            json2.Add("password", "newP");

            var payload2 = new StringContent(json2.ToString(), Encoding.UTF8, "application/json");
            var response2 = await client.PostAsync("/api/session/reset_password", payload2);
            Assert.True(response2.StatusCode.Equals(HttpStatusCode.Forbidden), "Unvalid response, returned: " + response2.ToString());
        }
    }
}
