using ITfoxtec.Identity.Saml2;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nexpo
{
    public interface IConfig
    {
        public string BaseUrl { get; set; }
        public string SendGridApiKey { get; set; }
        public string SecretKey { get; set; }
        public string JWTIssuer { get; set; }
        public string JWTAudience { get; set; }
        public string JWTExpires { get; set; }
        public string ConnectionString { get; set; }
        public Saml2Configuration Saml2 { get; set; }
        public string IdPMetadata { get; set; }
    }

    public class Config : IConfig
    {

        public Config(IConfiguration config)
        {
            BaseUrl = config["BaseUrl"];
            SendGridApiKey = config["SendGridApiKey"];
            SecretKey = config["SecretKey"];
            JWTIssuer = config["JWT:Issuer"];
            JWTAudience = config["JWT:Audience"];
            JWTExpires = config["JWT:Expires"];
            ConnectionString = config["ConnectionString"];
            IdPMetadata = config["Saml2:IdPMetadata"];
            Saml2 = new Saml2Configuration
            {
                
                Issuer = config["Saml2:Issuer"],
                SignatureAlgorithm = config["Saml2:SignatureAlgorithm"],
                //SigningCertificate = new System.Security.Cryptography.X509Certificates.X509Certificate2(config["Saml2:SigningCertificate"]),
            };}

        public string BaseUrl { get; set; }
        public string SendGridApiKey { get; set; }
        public string SecretKey { get; set; }
        public string JWTIssuer { get; set; }
        public string JWTAudience { get; set; }
        public string JWTExpires { get; set; }
        public string ConnectionString { get; set; }
        public Saml2Configuration Saml2 { get; set; }
        public string IdPMetadata { get; set; }

    }
}
