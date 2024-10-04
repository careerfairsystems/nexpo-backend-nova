using Microsoft.AspNetCore.Mvc;
using System;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Nexpo.AWS;
using Nexpo.Models;

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
                ); // Att skapa en ny instans i varje metod är väl redundant ? 
                
                

                var document = _aws3Services.DownloadFileAsync(documentName).Result;

                return File(document, "application/octet-stream", documentName);
            }
            catch (Exception ex)
            {
                return StatusCode( (int) HttpStatusCode.InternalServerError, ex.Message);
            }
        }


        /// <summary>
        /// Downloads a resume from S3
        /// </summary>
        [HttpGet]
        [Route("download_resume")]
        [Authorize(Roles = nameof(Role.Student) + "," + nameof(Role.Volunteer))]
        public async Task<IActionResult> DownloadResumeFromS3()
        {
            try
            {
                // Tries to find uuid, and handles the case when it cannot be found.
                var uuidClaim = HttpContext.User.FindFirst(UserClaims.Uuid);
                if (uuidClaim == null)
                {
                    return NotFound("Uuid could not be found.");
                }

                var uuid = uuidClaim.Value;

                var resumeName = $"{uuid}.pdf";

                var resume = await _aws3Services.DownloadFileAsync(resumeName);

                // If document was not found
                if (resume == null)
                {
                    return NotFound("The resume could not be found");
                }

                return File(resume, "application/octet-stream", "cv.pdf");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An unexpected error occurred"); // 500 Internal Server Error
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
        /// Uploads a resume to S3
        /// </summary>
        /// <param name="resume">The resume to upload</param>
        [HttpPost]
        [Route("upload_resume")]
        [Authorize(Roles = nameof(Role.Student) + "," + nameof(Role.Volunteer))]
        public async Task<IActionResult> UploadResumeToS3(IFormFile resume)
        {
            try
            {
                // In case no file is given.
                if (resume == null || resume.Length == 0) 
                {
                    return BadRequest("File is required to upload."); 
                }
                
                // If file is not a pdf.
                if (resume.ContentType != "application/pdf")
                {
                    return BadRequest("File is required to be a PDF");
                }
                
                // Tries to find uuid, and handles the case when it cannot be found.
                var uuidClaim = HttpContext.User.FindFirst(UserClaims.Uuid);
                if (uuidClaim == null)
                {
                    return NotFound("Uuid could not be found."); 
                }
                
                // Tries to upload resume
                bool result = await _aws3Services.UploadResume(resume, uuidClaim.Value);
                if (result)
                {
                    return Ok("Resume uploaded successfully."); 
                }

                return StatusCode(500, "Resume could not be uploaded."); // 500 Internal Server Error
            }
            catch (Exception e)
            {
                return StatusCode(500, "An unexpected error occurred"); // 500 Internal Server Error
            }
        }
        

        /// <summary>
        /// Delete a document from S3 using its name
        /// </summary>
        [HttpDelete("{documentName}")]
        public IActionResult DeleteDocumentFromS3(string documentName)
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