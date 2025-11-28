using Microsoft.Extensions.Configuration;
using System;
namespace Nexpo.AWS
{
    public class S3Config : IS3Configuration
    {
        // Keep the following details in appsettings.config file or DB or Enivironment variable
        // Get those values from it and assign to the below varibales. Based on the approach , modify the below code.
        public S3Config(IConfig configuration)
        {
            BucketName = "cvfiler";
            Region = "eu-north-1";

            // These keys are read from user secrets. The "secrets.json" file is not included in the repository.
            // It has to be added manually to the project (in the same level as startup.cs, ergo in the Nexpo folder). 
            // It is currently available via bitwarden.
            AwsAccessKey = configuration.AwsAccessKey;
            AwsSecretAccessKey = configuration.AwsSecretAccessKey;

        }


        public string BucketName { get; set; }
        public string Region { get; set; }
        public string AwsAccessKey { get; set; }
        public string AwsSecretAccessKey { get; set; }
        public string AwsSessionToken { get; set; }
    }
}