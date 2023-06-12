using Microsoft.Extensions.Configuration;
using System;
namespace Nexpo.AWS
{
    public class S3Config : IS3Configuration
    {
        // Keep the following details in appsettings.config file or DB or Enivironment variable
        // Get those values from it and assign to the below varibales. Based on the approach , modify the below code.
        public S3Config(IConfiguration configuration)
        {
            BucketName = "cvfiler";
            Region = "eu-north-1";

            // These are stored as user secrets in the project. 
            // Should currently be available via bitwarden
            Console.WriteLine("...");
            Console.WriteLine("AWS Access Key: " + configuration["AwsAccessKey"]);
            Console.WriteLine("AWS Secret Access Key: " + configuration["AwsSecretAccessKey"]);

            AwsAccessKey = configuration["AWS:AwsAccessKey"];
            AwsSecretAccessKey = configuration["AWS:AwsSecretAccessKey"];

        }


        public string BucketName { get; set; }
        public string Region { get; set; }
        public string AwsAccessKey { get; set; }
        public string AwsSecretAccessKey { get; set; }
        public string AwsSessionToken { get; set; }
    }
}