namespace Nexpo.AWS
{
    public class AppConfiguration : IAppConfiguration
    {
        // Keep the following details in appsettings.config file or DB or Enivironment variable
        // Get those values from it and assign to the below varibales. Based on the approach , modify the below code.
        public AppConfiguration()
        {
            BucketName = "";
            Region = "eu-north-1";
            AwsAccessKey = "";
            AwsSecretAccessKey = "";
            AwsSessionToken = "";
        }

        public string BucketName { get; set; }
        public string Region { get; set; }
        public string AwsAccessKey { get; set; }
        public string AwsSecretAccessKey { get; set; }
        public string AwsSessionToken { get; set; }
    }
}