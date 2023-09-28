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
        public string AwsAccessKey { get; set; }
        public string AwsSecretAccessKey { get; set; }
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
            AwsAccessKey = config["AwsAccessKey"];
            AwsSecretAccessKey = config["AwsSecretAccessKey"];
        }

        public string BaseUrl { get; set; }
        public string SendGridApiKey { get; set; }
        public string SecretKey { get; set; }
        public string JWTIssuer { get; set; }
        public string JWTAudience { get; set; }
        public string JWTExpires { get; set; }
        public string ConnectionString { get; set; }
        public string AwsAccessKey { get; set; }
        public string AwsSecretAccessKey { get; set; }

    }
}
