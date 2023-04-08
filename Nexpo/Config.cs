using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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

        // ** ADDED for SSO feature **
        public string SPEntityId { get; set; }
        public string IDPEntityId { get; set; }
        public string SPCertificatePath { get; set; }
        public string SPCertificatePassword { get; set; }
        public string SPPrivateKeyPath { get; set; }
        public string SPCallbackUrl { get; set; }
        public string SPLogoutUrl { get; set; }
        public string SPACSUrl { get; set; }


    }

    public class Config : IConfig
    {

    
        public Config(IConfiguration config)
        {
            BaseUrl = config["BaseUrl"];
            SendGridApiKey = config["SendGridApiKey"];
            SecretKey = config["SecretKey"];
            JWTIssuer = config["JWTIssuer"];
            JWTAudience = config["JWTAudience"];
            JWTExpires = config["JWTExpires"];
            ConnectionString = config["ConnectionString"];

            SPEntityId = config["SAML:SP:SPEntityId"];
            SPCallbackUrl = config["SAML:SP:SPCallbackUrl"];
            SPLogoutUrl = config["SAML:SP:SPLogoutUrl"];
            SPACSUrl = config["SAML:SP:SPACSUrl"];
            SPCertificatePath = config["SAML:SP:SPCertificatePath"];
            SPCertificatePassword = config["SAML:SP:SPCertificatePassword"];
            SPPrivateKeyPath = config["SAML:SP:SPPrivateKeyPath"];

            IDPEntityId = config["SAML:IDP:IDPEntityId"];
            // CertificatePath = config["SAML:IDP:IDPCertificatePath"];
            // CertificatePassword = config["SAML:IDP:IDPCertificatePassword"];
            // IDPSSOUrl = config["SAML:IDP:IDPSSOUrl"];
            // IDPLogoutUrl = config["SAML:IDP:IDPLogoutUrl"];


        }

        public string BaseUrl { get; set; }
        public string SendGridApiKey { get; set; }
        public string SecretKey { get; set; }
        public string JWTIssuer { get; set; }
        public string JWTAudience { get; set; }
        public string JWTExpires { get; set; }
        public string ConnectionString { get; set; }
        public string SPEntityId { get; set; }
        public string IDPEntityId { get; set; }
        public string SPCertificatePath { get; set; }
        public string SPCertificatePassword { get; set; }
        public string SPPrivateKeyPath { get; set; }
        public string SPCallbackUrl { get; set; }
        public string SPLogoutUrl { get; set; }
        public string SPACSUrl { get; set; }






    }
}
