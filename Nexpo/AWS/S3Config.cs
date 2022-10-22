namespace Nexpo.AWS
{
    public class S3Config : IS3Configuration
    {
        // Keep the following details in appsettings.config file or DB or Enivironment variable
        // Get those values from it and assign to the below varibales. Based on the approach , modify the below code.
        public S3Config()
        {
            BucketName = "cvfiler";
            Region = "eu-north-1";
            AwsAccessKey = "AKIAX3BYI22ZD733TJZ3";
            AwsSecretAccessKey = "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";
        }

        public string BucketName { get; set; }
        public string Region { get; set; }
        public string AwsAccessKey { get; set; }
        public string AwsSecretAccessKey { get; set; }
        public string AwsSessionToken { get; set; }
    }
}