using Amazon.S3;
using Amazon.S3.Transfer;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;
using System;
using Amazon;

namespace Nexpo.AWS
{
    public class Aws3Services : IAws3Services
    {
        private readonly string _bucketName;
        private readonly IAmazonS3 _awsS3Client;

        //public Aws3Services(string awsAccessKeyId, string awsSecretAccessKey, string awsSessionToken, string region, string bucketName)
        public Aws3Services()
        {
            _bucketName = "cvfiler";

            _awsS3Client = new AmazonS3Client("AKIAX3BYI22ZD733TJZ3", "Zz6i8UUK3FH003JjnvzqtQTjb7SMg9qxV2CSCfBK", RegionEndpoint.GetBySystemName("eu-north-1"));
            //_awsS3Client = new AmazonS3Client(awsAccessKeyId, awsSecretAccessKey, RegionEndpoint.GetBySystemName(region));
            
            //Session token needed?
            //_awsS3Client = new AmazonS3Client(awsAccessKeyId, awsSecretAccessKey, awsSessionToken, RegionEndpoint.GetBySystemName(region));
        }

        public async Task<bool> UploadFileAsync(IFormFile file)
        {
            try
            {
                using (var newMemoryStream = new MemoryStream())
                {
                    file.CopyTo(newMemoryStream);

                    var uploadRequest = new TransferUtilityUploadRequest
                    {
                        InputStream = newMemoryStream,
                        Key = file.FileName,
                        BucketName = _bucketName,
                        ContentType = file.ContentType
                    };

                    var fileTransferUtility = new TransferUtility(_awsS3Client);

                    await fileTransferUtility.UploadAsync(uploadRequest);

                    return true;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}