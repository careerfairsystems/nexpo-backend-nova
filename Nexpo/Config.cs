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
	public string AWSKey { get; set; }
	public string AWSSecret { get; set; }
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
	    AWSKey = config["AWSKey"];
	    AWSSecret = config["AWSSecret"];
        }
	
	public string AWSKey { get; set; }
	public string AWSSecret { get; set; }
        public string BaseUrl { get; set; }
        public string SendGridApiKey { get; set; }
        public string SecretKey { get; set; }
        public string JWTIssuer { get; set; }
        public string JWTAudience { get; set; }
        public string JWTExpires { get; set; }
        public string ConnectionString { get; set; }

    }
}
