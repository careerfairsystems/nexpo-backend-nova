using Microsoft.AspNetCore.Mvc;
using System;
using Microsoft.AspNetCore.Http;
using System.Net;
using Nexpo.AWS;

namespace Nexpo.Controllers
{
    ///<summary>
    /// Controller for AWS S3 Bucket
    /// AWS S3 Bucket is used to store documents
    /// (A document is a file that is uploaded to the S3 Bucket)
    ///</summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AwsS3Controller : ControllerBase
    {
        private IS3Configuration _appConfiguration;
        private IAws3Services _aws3Services;

        public AwsS3Controller(IS3Configuration appConfiguration, IAws3Services aws3Services)
        {
            _appConfiguration = appConfiguration;
            _aws3Services     = aws3Services;
        }

        /// <summary>
        /// Get a document from S3 using the document name
        /// </summary>
        [HttpGet("{documentName}")]
        public IActionResult GetDocumentFromS3(string documentName)
        {
            try
            {
                if (string.IsNullOrEmpty(documentName))
                {
                    return StatusCode( (int) HttpStatusCode.BadRequest, "the document name is required");
                }

                _aws3Services = new Aws3Services(
                    _appConfiguration.AwsAccessKey,
                    _appConfiguration.AwsSecretAccessKey,
                    _appConfiguration.Region, 
                    _appConfiguration.BucketName
                );

                var document = _aws3Services.DownloadFileAsync(documentName).Result;

                return File(document, "application/octet-stream", documentName);
            }
            catch (Exception ex)
            {
                return StatusCode( (int) HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Upload a document to S3
        /// </summary>
        /// <param name="file">The file to upload</param>
        /// <param name="name">The name of the file</param>
        [HttpPost]
        [Route("{name}")]
        public IActionResult UploadDocumentToS3(IFormFile file, string name)
        {
            try
            {
                if (file is null || file.Length <= 0)
                {
                    return StatusCode( (int) HttpStatusCode.InternalServerError, "file is required to upload");
                }

                _aws3Services = new Aws3Services(
                    _appConfiguration.AwsAccessKey,
                    _appConfiguration.AwsSecretAccessKey,
                    _appConfiguration.Region,
                    _appConfiguration.BucketName
                );

                var result = _aws3Services.UploadFileAsync(file,name);

                return Ok((int)HttpStatusCode.Created);
            }
            catch (Exception ex)
            {
                return StatusCode( (int) HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Delete a document from S3 using its name
        /// </summary>
        [HttpDelete("{documentName}")]
        public IActionResult DeletetDocumentFromS3(string documentName)
        {
            try
            {
                if (string.IsNullOrEmpty(documentName))
                {
                    return StatusCode( (int)HttpStatusCode.BadRequest, "The 'documentName' parameter is required");
                }
                
                _aws3Services = new Aws3Services(
                    _appConfiguration.AwsAccessKey,
                    _appConfiguration.AwsSecretAccessKey,
                    _appConfiguration.Region,
                    _appConfiguration.BucketName
                );

                _aws3Services.DeleteFileAsync(documentName);

                return StatusCode((int)HttpStatusCode.OK, documentName);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }

    }

    



}