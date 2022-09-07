using Microsoft.Extensions.Configuration;
using Nexpo.Controllers;
using Nexpo.Services;
using System;
using System.Collections.Generic;
using Xunit;

namespace Nexpo.Tests.Services
{
    public class UsersControllerTest
    {
        private readonly TokenService _tokenService;
        private readonly UsersController _usersController;

        public UsersControllerTest()
        {
            var config = new MockConfig
            {
                SecretKey = "dGVzdGluZ19zZWNyZXRfa2V5", // base64(testing_secret_key)
                JWTIssuer = "http://localhost:5000",
                JWTAudience = "http://localhost:5000",
                JWTExpires = "72",
            };
            _tokenService = new TokenService(config);

            _usersController = new UsersController();
        }


        [Fact]
        public void testTest()
        {

        }

        //[Fact]
        //public void testTest()
        //{
        //    var firstTokenString = _tokenService.SignToken(_testData, DateTime.Now.AddHours(1));

        //    Assert.NotEqual(firstTokenString, secondTokenString);
        //}
    }
}
