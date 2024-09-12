using Amazon.S3;
using Amazon.S3.Transfer;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;
using System;
using Amazon;
using Amazon.S3.Model;
using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nexpo.Controllers;
using Nexpo.Models;

namespace Nexpo.AWS
{
    public class Aws3Services : IAws3Services
    {
        private readonly string _bucketName;
        private readonly IAmazonS3 _awsS3Client;
        private readonly IHttpContextAccessor _httpContextAccessor;

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
            try
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
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Uploads a user's resume to AWS S3
        /// </summary>
        /// <param name="resume"> The resume file </param>
        
        [Authorize(Roles = nameof(Role.Student) + "," + nameof(Role.Volunteer))]
        public async Task<bool> UploadResume(IFormFile resume, string uuid)
        {
            try
            {
                using (var newMemoryStream = new MemoryStream())
                {
                    await resume.CopyToAsync(newMemoryStream);
                    var uploadRequest = new TransferUtilityUploadRequest
                    {
                        InputStream = newMemoryStream,
                        Key = $"{uuid}.pdf",
                        BucketName = _bucketName,
                        ContentType = resume.ContentType
                    };

                    var fileTransferUtility = new TransferUtility(_awsS3Client);
                    await fileTransferUtility.UploadAsync(uploadRequest);
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        
        /// <summary>
        /// Downloads a file from AWS S3
        /// </summary>
        /// <param name="file">The name of the file to download</param>
        public async Task<byte[]> DownloadFileAsync(string file)
        {
            MemoryStream ms = null;
            try
            {
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
            catch (Exception)
            {
                throw;
            }
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
                if (ex.InnerException != null && ex.InnerException is AmazonS3Exception awsEx)
                {
                    if (string.Equals(awsEx.ErrorCode, "NoSuchBucket"))
                        return false;

                    else if (string.Equals(awsEx.ErrorCode, "NotFound"))
                        return false;
                }
                return false;
                throw;
            }
        }
    }
}