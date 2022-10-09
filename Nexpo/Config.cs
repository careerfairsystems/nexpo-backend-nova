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

        // ** ADDED for SSO feature **
        public string SPEntityId { get; set; }
        public string IDPEntityId { get; set; }
        public string CertificatePath { get; set; }
        public string CertificatePassword { get; set; }

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
            SPEntityId = config["SAML:SPEntityId"];
            IDPEntityId = config["SAML:IDPEntityId"];
            CertificatePath = config["SAML:CertificatePath"];
            CertificatePassword = config["SAML:CertificatePassword"];


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
        public string CertificatePath { get; set; }
        public string CertificatePassword { get; set; }


    }
}
