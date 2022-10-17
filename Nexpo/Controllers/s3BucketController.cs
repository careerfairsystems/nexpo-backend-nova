using Microsoft.AspNetCore.Mvc;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Net;
using Nexpo.AWS;
using System.IO;
using Microsoft.EntityFrameworkCore.Query;

namespace Nexpo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //api/[controller]
    public class AwsS3Controller : ControllerBase
    {
        private IAppConfiguration _appConfiguration;
        private IAws3Services _aws3Services;

        public AwsS3Controller(IAppConfiguration appConfiguration, IAws3Services aws3Services)
        {
            _appConfiguration = appConfiguration;
            _aws3Services = aws3Services;
        }

        [HttpGet("{documentName}")]
        public IActionResult GetDocumentFromS3(string documentName)
        {
            try
            {
                if (string.IsNullOrEmpty(documentName))
                    return StatusCode((int)HttpStatusCode.BadRequest, "the document name is required");

                _aws3Services = new Aws3Services();

                var document = _aws3Services.DownloadFileAsync(documentName).Result;

                return File(document, "application/octet-stream", documentName);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        [Route("post")]
        public IActionResult UploadDocumentToS3(IFormFile file)
        //public IActionResult UploadDocumentToS3()
        {
            //File file = File.Create("C:\\Users\\Hampus\\Documents\\Arkad\\cv\\test.pdf");
            try
            {
                if (file is null || file.Length <= 0)
                    //return ReturnMessage("file is required to upload", (int)HttpStatusCode.BadRequest);
                    return StatusCode((int)HttpStatusCode.InternalServerError, "file is required to upload");

                //_aws3Services = new Aws3Services(_appConfiguration.AwsAccessKey, _appConfiguration.AwsSecretAccessKey, _appConfiguration.AwsSessionToken, _appConfiguration.Region, _appConfiguration.BucketName);
                _aws3Services = new Aws3Services();

                var result = _aws3Services.UploadFileAsync(file);

                return Ok((int)HttpStatusCode.Created);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }


        [HttpDelete("{documentName}")]
        public IActionResult DeletetDocumentFromS3(string documentName)
        {
            try
            {
                if (string.IsNullOrEmpty(documentName))
                    return StatusCode( (int)HttpStatusCode.BadRequest, "The 'documentName' parameter is required");

                _aws3Services = new Aws3Services();

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