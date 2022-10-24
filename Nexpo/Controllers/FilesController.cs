using IOFile = System.IO.File;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nexpo.Helpers;
using Nexpo.Models;
using Nexpo.Repositories;
using Nexpo.Services;
using Microsoft.AspNetCore.StaticFiles;
using System.Collections.Generic;
using Nexpo.DTO;

namespace Nexpo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly IUserRepository _userRepo;
        private readonly IStudentRepository _studentRepo;
        private readonly ICompanyRepository _companyRepo;
        private readonly FileService _fileService;

        public FilesController(IUserRepository iUserRepo, IStudentRepository iStudentRepo, ICompanyRepository iCompanyRepo, FileService fileService)
        {
            _userRepo = iUserRepo;
            _studentRepo = iStudentRepo;
            _companyRepo = iCompanyRepo;
            _fileService = fileService;
        }

        [HttpGet]
        [Route("{filename}")]
        [ProducesResponseType(typeof(IOFile), StatusCodes.Status200OK)]
        public ActionResult GetFile(string filename)
        {
            var localFilename = _fileService.GetLocalFileName(filename);
            if (!IOFile.Exists(localFilename)) {
                return NotFound();
            }

            var mimeProvider = new FileExtensionContentTypeProvider();
            string mime;
            if (!mimeProvider.TryGetContentType(filename, out mime))
            {
                mime = "application/octet-stream";
            }
            
            return File(IOFile.OpenRead(localFilename), mime);
        }

    //    [HttpPost]
    //    [Route("profile_picture")]
    //    [Consumes("multipart/form-data")]
    //    [Authorize]
    //    [ProducesResponseType(typeof(FileCreatedDto), StatusCodes.Status200OK)]
    //    public async Task<ActionResult> PostProfilePicture(IFormFile file)
    //    {
    //        var maxAllowedFileSize = 2 * 1024 * 1024; // 2 MiB
    //        if (file == null || file.Length > maxAllowedFileSize)
    //        {
    //            return BadRequest();
    //        }
//
    //        var allowedMimes = new List<string> { "image/png", "image/jpeg" };
    //        if (!allowedMimes.Contains(file.ContentType))
    //        {
    //            return BadRequest();
    //        }
//
    //        var userId = HttpContext.User.GetId();
    //        var user = await _userRepo.Get(userId);
    //        if (!string.IsNullOrEmpty(user.ProfilePictureUrl))
    //        {
    //            _fileService.RemoveFile(user.ProfilePictureUrl);
    //        }
//
    //        var fileUrl = await _fileService.SaveFile(file);
    //        user.ProfilePictureUrl = fileUrl;
    //        await _userRepo.Update(user);
//
    //        var response = new FileCreatedDto { Url = fileUrl };
//
    //        return Ok(response);
    //    }
//
    //    [HttpDelete]
    //    [Route("profile_picture")]
    //    [Authorize]
    //    [ProducesResponseType(StatusCodes.Status204NoContent)]
    //    public async Task<ActionResult> DeleteProfilePicture()
    //    {
    //        var userId = HttpContext.User.GetId();
    //        var user = await _userRepo.Get(userId);
    //        if (!string.IsNullOrEmpty(user.ProfilePictureUrl))
    //        {
    //            _fileService.RemoveFile(user.ProfilePictureUrl);
    //            user.ProfilePictureUrl = null;
    //            await _userRepo.Update(user);
    //        }
//
    //        return NoContent();
    //    }
//
        [HttpPost]
        [Route("company_logo")]
        [Consumes("multipart/form-data")]
        [Authorize(Roles = nameof(Role.CompanyRepresentative))]
        [ProducesResponseType(typeof(FileCreatedDto), StatusCodes.Status200OK)]
        public async Task<ActionResult> PostCompanyLogo(IFormFile file)
        {
            var maxAllowedFileSize = 2 * 1024 * 1024; // 2 MiB
            if (file == null || file.Length > maxAllowedFileSize)
            {
                return BadRequest();
            }

            var allowedMimes = new List<string> { "image/png", "image/jpeg" };
            if (!allowedMimes.Contains(file.ContentType))
            {
                return BadRequest();
            }

            var companyId = HttpContext.User.GetCompanyId().Value;
            var company = await _companyRepo.Get(companyId);
            if (!string.IsNullOrEmpty(company.LogoUrl))
            {
                _fileService.RemoveFile(company.LogoUrl);
            }

            var fileUrl = await _fileService.SaveFile(file);
            company.LogoUrl = fileUrl;
            await _companyRepo.Update(company);

            var response = new FileCreatedDto { Url = fileUrl };

            return Ok(response);
        }
        
        [HttpDelete]
        [Route("company_logo")]
        [Authorize(Roles = nameof(Role.CompanyRepresentative))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> DeleteCompanyLogo()
        {
            var companyId = HttpContext.User.GetCompanyId().Value;
            var company = await _companyRepo.Get(companyId);
            if (!string.IsNullOrEmpty(company.LogoUrl))
            {
                _fileService.RemoveFile(company.LogoUrl);
                company.LogoUrl = null;
                await _companyRepo.Update(company);
            }

            return NoContent();
        }
        
        [HttpPost]
        [Route("resume_english")]
        [Consumes("multipart/form-data")]
        [Authorize(Roles = nameof(Role.Student))]
        [ProducesResponseType(typeof(FileCreatedDto), StatusCodes.Status200OK)]
        public async Task<ActionResult> PostResumeEnglish(IFormFile file)
        {
            var maxAllowedFileSize = 4 * 1024 * 1024; // 4 MiB
            if (file == null || file.Length > maxAllowedFileSize)
            {
                return BadRequest();
            }
            
            var allowedMimes = new List<string> { "application/pdf" };
            if (!allowedMimes.Contains(file.ContentType))
            {
                return BadRequest();
            }

            var studentId = HttpContext.User.GetStudentId().Value;
            var student = await _studentRepo.Get(studentId);
            if (!string.IsNullOrEmpty(student.ResumeEnUrl))
            {
                _fileService.RemoveFile(student.ResumeEnUrl);
            }

            var fileUrl = await _fileService.SaveFile(file);
            student.ResumeEnUrl = fileUrl;
            await _studentRepo.Update(student);

            var response = new FileCreatedDto { Url = fileUrl };

            return Ok(response);
        }
        
        [HttpDelete]
        [Route("resume_english")]
        [Authorize(Roles = nameof(Role.Student))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> DeleteResumeEnglish()
        {
            var studentId = HttpContext.User.GetStudentId().Value;
            var student = await _studentRepo.Get(studentId);
            if (!string.IsNullOrEmpty(student.ResumeEnUrl))
            {
                _fileService.RemoveFile(student.ResumeEnUrl);
                student.ResumeEnUrl = null;
                await _studentRepo.Update(student);
            }

            return NoContent();
        }

        [HttpPost]
        [Route("resume_swedish")]
        [Consumes("multipart/form-data")]
        [Authorize(Roles = nameof(Role.Student))]
        [ProducesResponseType(typeof(FileCreatedDto), StatusCodes.Status200OK)]
        public async Task<ActionResult> PostResumeSwedish(IFormFile file)
        {
            var maxAllowedFileSize = 4 * 1024 * 1024; // 4 MiB
            if (file == null || file.Length > maxAllowedFileSize)
            {
                return BadRequest();
            }

            
            var allowedMimes = new List<string> { "application/pdf" };
            if (!allowedMimes.Contains(file.ContentType))
            {
                return BadRequest();
            }

            var studentId = HttpContext.User.GetStudentId().Value;
            var student = await _studentRepo.Get(studentId);
            if (!string.IsNullOrEmpty(student.ResumeSvUrl))
            {
                _fileService.RemoveFile(student.ResumeSvUrl);
            }

            var fileUrl = await _fileService.SaveFile(file);
            student.ResumeSvUrl = fileUrl;
            await _studentRepo.Update(student);

            var response = new FileCreatedDto { Url = fileUrl };

            return Ok(response);
        }

        [HttpDelete]
        [Route("resume_swedish")]
        [Authorize(Roles = nameof(Role.Student))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> DeleteResumeSwedish()
        {
            var studentId = HttpContext.User.GetStudentId().Value;
            var student = await _studentRepo.Get(studentId);
            if (!string.IsNullOrEmpty(student.ResumeSvUrl))
            {
                _fileService.RemoveFile(student.ResumeSvUrl);
                student.ResumeSvUrl = null;
                await _studentRepo.Update(student);
            }

            return NoContent();
        }
    }
}

