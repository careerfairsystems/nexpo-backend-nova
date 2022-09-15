using Microsoft.AspNetCore.Mvc.Testing;
using Nexpo.Controllers;
using Nexpo.Repositories;
using Nexpo.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Nexpo.Tests.Controllers
{ 
    public class UserControllerTest
    {

        [Fact]
        public async Task TestGetUsers()
        {
            var application = new WebApplicationFactory<Nexpo.Program>();
            var client = application.CreateClient();
            var response = await client.GetAsync("/api/users");
            Assert.True(response.StatusCode.Equals(HttpStatusCode.Unauthorized), response.StatusCode.ToString());

        }
    }
}
