using Amazon.S3;
using Amazon.S3.Transfer;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;
using System;
using Amazon;
using Amazon.S3.Model;
using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace Nexpo.AWS
{
    public class Aws3Services : IAws3Services
    {
        private readonly string _bucketName;
        private readonly IAmazonS3 _awsS3Client;

        public Aws3Services(string awsAccessKeyId, string awsSecretAccessKey, string region, string bucketName)
        {
            _bucketName = bucketName;
            _awsS3Client = new AmazonS3Client(awsAccessKeyId, awsSecretAccessKey, RegionEndpoint.GetBySystemName(region));
        }

        /// <summary>
        /// Uploads a file to AWS S3
        /// </summary>
        /// <param name="file">The file to upload</param>
        /// <param name="name">The name of the file</param>
        public async Task<bool> UploadFileAsync(IFormFile file, string name)
        {
            using (var newMemoryStream = new MemoryStream())
            {
                file.CopyTo(newMemoryStream);
                var uploadRequest = new TransferUtilityUploadRequest
                {
                    InputStream = newMemoryStream,
                    Key         = name,
                    BucketName  = _bucketName,
                    ContentType = file.ContentType
                };

                var fileTransferUtility = new TransferUtility(_awsS3Client);
                await fileTransferUtility.UploadAsync(uploadRequest);
                return true;
            }
        }


        /// <summary>
        /// Downloads a file from AWS S3
        /// </summary>
        /// <param name="file">The name of the file to download</param>
        public async Task<byte[]> DownloadFileAsync(string file)
        {
        MemoryStream ms = null;

            GetObjectRequest getObjectRequest = new GetObjectRequest
            {
                BucketName = _bucketName,
                Key = file
            };

            using (var response = await _awsS3Client.GetObjectAsync(getObjectRequest))
            {
                if (response.HttpStatusCode == HttpStatusCode.OK)
                {
                    using (ms = new MemoryStream())
                    {
                        await response.ResponseStream.CopyToAsync(ms);
                    }
                }
            }

            if (ms is null || ms.ToArray().Length < 1)
                throw new FileNotFoundException(string.Format("The document '{0}' is not found", file));
            return ms.ToArray();
        }

        /// <summary>
        /// Deletes a file from AWS S3
        /// </summary>
        /// <param name="fileName">The name of the file to delete</param>
        [HttpDelete("{documentName}")]
        public async Task<bool> DeleteFileAsync(string fileName)
        {
            DeleteObjectRequest request = new DeleteObjectRequest
            {
                BucketName = _bucketName,
                Key = fileName
            };
            await _awsS3Client.DeleteObjectAsync(request);
            return true;
        }

        public bool IfFileExists(string fileName)
        {
            try
            {
                GetObjectMetadataRequest request = new GetObjectMetadataRequest()
                {
                    BucketName = _bucketName,
                    Key = fileName
                };

                var response = _awsS3Client.GetObjectMetadataAsync(request).Result;
                return true;
            }
            catch (Exception ex)
            {
                return ex.InnerException != null && ex.InnerException is AmazonS3Exception awsEx && (string.Equals(awsEx.ErrorCode, "NoSuchBucket") || string.Equals(awsEx.ErrorCode, "NotFound"));

            }
        }
    }
}