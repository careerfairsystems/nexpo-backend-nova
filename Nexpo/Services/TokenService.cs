using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Nexpo.Services
{
    /// <summary>
    /// A class with methods for token signing and validation. Used to create stateless proofs that the object originated from this server
    /// </summary>
    public class TokenService
    {

        /// <summary>
        /// The divider that splits the token and the signature
        /// </summary>
        private readonly char Divider = '.';

        private readonly IConfig _config;

        public TokenService(IConfig iConfig)
        {
            _config = iConfig;
        }

        private string Sign(string message)
        {
            var hmac = new HMACSHA256(Convert.FromBase64String(_config.SecretKey));
            var signature = Convert.ToBase64String(hmac.ComputeHash(Encoding.Default.GetBytes(message)));
            return signature;
        }

        /// <summary>
        /// Serialize and sign an object in a token
        /// </summary>
        public string SignToken<T>(T message, DateTime expires)
        {
            // Wrap the data in an object with metadata
            var token = new Token<T> 
            {
                Value = message,
                Expires = expires
            };
            // Serialize the token to a JSON string
            var jsonToken = JsonConvert.SerializeObject(token);
            // Base64 encode the string so we have full control over what characters are used (so they don't interfer with the divider)
            var base64Token = Convert.ToBase64String(Encoding.Default.GetBytes(jsonToken));

            // Sign token
            var signature = Sign(base64Token);

            var signedToken = $"{base64Token}{Divider}{signature}";
            return signedToken;
        }

        /// <summary>
        /// Validates and parses a signed string containing a token. Returns default(T) (usually null) if something failed
        /// </summary>
        /// <returns>the deserialized object or default(T) if anything is wrong</returns>
        public Token<T> ValidateToken<T>(string signedString)
        {
            // Divide the raw string into token and signature
            var token = signedString.Split(Divider).FirstOrDefault();
            var signature = signedString.Split(Divider).LastOrDefault();

            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(signature))
            {
                // The signed string was invalid
                return new Token<T> { IsValid = false };
            }

            // Signature is invalid
            var actualSignature = Sign(token);
            if (signature != actualSignature)
            {
                return new Token<T> { IsValid = false };
            }

            // Convert from Base64 to a normal JSON string
            var jsonToken = Encoding.Default.GetString(Convert.FromBase64String(token));
            // Deserialize from JSON to Token<T>
            var parsedToken = JsonConvert.DeserializeObject<Token<T>>(jsonToken);
            if (parsedToken == null)
            {
                // Deserialization failed
                parsedToken.IsValid = false;
                return default;
            }

            if (parsedToken.Expires < DateTime.Now)
            {
                // The token has expired
                parsedToken.IsValid = false;
                return parsedToken;
            }

            return parsedToken;
        }

        /// <summary>
        /// Generate a JWT that includes the provided claims
        /// </summary>
        public string GenerateJWT(IEnumerable<Claim> claims)
        {
            var key = new SymmetricSecurityKey(Convert.FromBase64String(_config.SecretKey));

            var token = new JwtSecurityToken(
                claims: claims,
                issuer: _config.JWTIssuer,
                audience: _config.JWTAudience,
                notBefore: DateTime.Now,
                expires: DateTime.Now.AddHours(Convert.ToDouble(_config.JWTExpires)),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            ); ;

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public class Token<T>
        {
            public T Value { get; set; }
            public DateTime Expires { get; set; } = DateTime.Now;

            public bool IsValid { get; set; } = true;
            public bool Expired => Expires < DateTime.Now;
            
        }
    }
}
