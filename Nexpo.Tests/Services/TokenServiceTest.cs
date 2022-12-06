using Nexpo.Services;
using System;
using Xunit;

namespace Nexpo.Tests.Services
{
    public class TokenServiceTest
    {
        private readonly TokenService _tokenService;
        private readonly TokenService _tokenService2;
        private readonly TestDataClass _testData;

        public TokenServiceTest()
        {
            var config = new MockConfig
            {
                SecretKey = "dGVzdGluZ19zZWNyZXRfa2V5", // base64(testing_secret_key)
                JWTIssuer = "http://localhost:5000",
                JWTAudience = "http://localhost:5000",
                JWTExpires = "72",
            };
            _tokenService = new TokenService(config);

            var config2 = new MockConfig
            {
                SecretKey = "dGVzdGluZ19zZWNyZXRfa2V5Mg==", // base64(testing_secret_key2)
                JWTIssuer = "http://localhost:5000",
                JWTAudience = "http://localhost:5000",
                JWTExpires = "72",
            };
            _tokenService2 = new TokenService(config2);

            _testData = new TestDataClass
            {
                StringMessage = "String message",
                IntMessage = 123
            };
        }

        

        [Fact]
        public void TokenService_Should_Serialize_and_Deserialize_Successfully()
        {
            var tokenString = _tokenService.SignToken(_testData, DateTime.Now.AddHours(1));
            var token = _tokenService.ValidateToken<TestDataClass>(tokenString);
            Assert.True(token.IsValid);

            var data = token.Value;

            Assert.Equal(_testData.StringMessage, data.StringMessage);
            Assert.Equal(_testData.IntMessage, data.IntMessage);
        }

        [Fact]
        public void ValidateToken_Should_Not_Accept_Invalid_Signature()
        {
            var tokenString = _tokenService.SignToken(_testData, DateTime.Now.AddHours(1));
            var invalidTokenString = $"${tokenString}this_makes_it_invalid";

            var token = _tokenService.ValidateToken<TestDataClass>(invalidTokenString);

            Assert.False(token.IsValid);
        }

        [Fact]
        public void SignToken_Should_Return_Different_Depending_On_SecretKey()
        {
            var firstTokenString = _tokenService.SignToken(_testData, DateTime.Now.AddHours(1));
            var secondTokenString = _tokenService2.SignToken(_testData, DateTime.Now.AddHours(1));

            Assert.NotEqual(firstTokenString, secondTokenString);
        }

        [Fact]
        public void ValidateToken_Should_Return_Invalid_If_Expired()
        {
            var tokenString = _tokenService.SignToken(_testData, DateTime.Now.AddHours(-1));
            var token = _tokenService.ValidateToken<TestDataClass>(tokenString);

            Assert.False(token.IsValid);
            Assert.True(token.Expired);
        }

        private class TestDataClass
        {
            public string StringMessage { get; set; }
            public int IntMessage { get; set; }
        }
    }
}
