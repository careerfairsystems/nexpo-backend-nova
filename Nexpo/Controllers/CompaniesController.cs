using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nexpo.DTO;
using Nexpo.Helpers;
using Nexpo.Models;
using Nexpo.Repositories;

namespace Nexpo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompaniesController : ControllerBase
    {
        private readonly ICompanyRepository _companyRepo;

        public CompaniesController(ICompanyRepository iCompanyRepo)
        {
            _companyRepo = iCompanyRepo;
        }

        /// <summary>
        /// Get a list of all companies
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<PublicCompanyDTO>), StatusCodes.Status200OK)]
        public async Task<ActionResult> GetCompanies()
        {
            var companies = await _companyRepo.GetAll();
            var publicCompanies = companies.Select(company => new PublicCompanyDTO
            {
                Id                       = company.Id.Value,
                Name                     = company.Name,
                Description              = company.Description,
                DidYouKnow               = company.DidYouKnow,
                Website                  = company.Website,
                LogoUrl                  = company.LogoUrl,
                DaysAtArkad              = company.DaysAtArkad,
                DesiredDegrees           = company.DesiredDegrees,
                DesiredProgramme         = company.DesiredProgramme,
                Positions                = company.Positions,
                Industries               = company.Industries,
                StudentSessionMotivation = company.StudentSessionMotivation
            });

            return Ok(publicCompanies);
        }

        /// <summary>
        /// Get a single comapny
        /// </summary>
        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(typeof(PublicCompanyDTO), StatusCodes.Status200OK)]
        public async Task<ActionResult> GetCompany(int id)
        {
            var company = await _companyRepo.Get(id);
            if (company == null) 
            {
                return NotFound();
            }

            var publicCompany = new PublicCompanyDTO
            {
                Id                       = company.Id.Value,
                Name                     = company.Name,
                Description              = company.Description,
                DidYouKnow               = company.DidYouKnow,
                Website                  = company.Website,
                LogoUrl                  = company.LogoUrl,
                DaysAtArkad              = company.DaysAtArkad,
                DesiredDegrees           = company.DesiredDegrees,
                DesiredProgramme         = company.DesiredProgramme,
                Positions                = company.Positions,
                Industries               = company.Industries,
                StudentSessionMotivation = company.StudentSessionMotivation

            };
            return Ok(publicCompany);
        }
        
        /// <summary>
        /// Update a company's information
        /// </summary>
        [HttpPut]
        [Route("{id}")]
        [Authorize(Roles = nameof(Role.Administrator))]
        [ProducesResponseType(typeof(Company), StatusCodes.Status200OK)]
        public async Task<ActionResult> PutCompany(int id, UpdateCompanyAdminDTO DTO)
        {
            var company = await _companyRepo.Get(id);
            if (company == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(DTO.Description))
            {
                company.Description = DTO.Description;
            }
            if (!string.IsNullOrEmpty(DTO.DidYouKnow))
            {
                company.DidYouKnow = DTO.DidYouKnow;
            }
            if (!string.IsNullOrEmpty(DTO.Website))
            {
                company.Website = DTO.Website;
            }
            if (!string.IsNullOrEmpty(DTO.HostEmail))
            {
                company.HostEmail = DTO.HostEmail;
            }
            if (!string.IsNullOrEmpty(DTO.HostName))
            {
                company.HostName = DTO.HostName;
            }
            if (!string.IsNullOrEmpty(DTO.HostPhone))
            {
                company.HostPhone = DTO.HostPhone;
            }
            if (!string.IsNullOrEmpty(DTO.StudentSessionMotivation))
            {
                company.StudentSessionMotivation = DTO.StudentSessionMotivation;
            }
            
            if (DTO.DaysAtArkad != null)
            {
                company.DaysAtArkad = new List<DateTime>(DTO.DaysAtArkad);
            }

            await _companyRepo.Update(company);

            return Ok(company);
        }
        
        /// <summary>
        /// Get the company currently signed in
        /// </summary>
        [HttpGet]
        [Route("me")]
        [Authorize(Roles = nameof(Role.CompanyRepresentative))]
        [ProducesResponseType(typeof(Company), StatusCodes.Status200OK)]
        public async Task<ActionResult> GetMe()
        {
            var companyId = HttpContext.User.GetCompanyId();
            var company = await _companyRepo.GetWithChildren(companyId.Value);
            return Ok(company);
        }

        /// <summary>
        /// Update the signed in company's information
        /// </summary>
        [HttpPut]
        [Route("me")]
        [Authorize(Roles = nameof(Role.CompanyRepresentative))]
        [ProducesResponseType(typeof(Company), StatusCodes.Status200OK)]
        public async Task<ActionResult> PutMe(UpdateCompanySelfDTO DTO)
        {
            var companyId = HttpContext.User.GetCompanyId().Value;
            var company = await _companyRepo.Get(companyId);

            if (!string.IsNullOrEmpty(DTO.Description))
            {
                company.Description = DTO.Description;
            }
            if (!string.IsNullOrEmpty(DTO.DidYouKnow))
            {
                company.DidYouKnow = DTO.DidYouKnow;
            }
            if (!string.IsNullOrEmpty(DTO.Website))
            {
                company.Website = DTO.Website;
            }

            if (DTO.DaysAtArkad != null)
            {
                company.DaysAtArkad = new List<DateTime>(DTO.DaysAtArkad);
            }

            await _companyRepo.Update(company);

            return Ok(company);
        }

        /// <summary>
        /// Add new company
        /// </summary>
        [HttpPost]
        [Authorize(Roles = nameof(Role.Administrator))]
        [ProducesResponseType(typeof(Company), StatusCodes.Status200OK)]
        public async Task<ActionResult> AddNewCompany(AddCompanyDTO DTO)
        {
            
            var company = new Company {
                Name                     = DTO.Name,
                Description              = DTO.Description,
                DidYouKnow               = DTO.DidYouKnow,
                LogoUrl                  = DTO.LogoUrl,
                Website                  = DTO.Website,
                DaysAtArkad              = DTO.DaysAtArkad,
                DesiredDegrees           = DTO.DesiredDegrees,
                DesiredProgramme         = DTO.DesiredProgramme,
                Positions                = DTO.Positions,
                Industries               = DTO.Industries,
                HostName                 = DTO.HostName,
                HostEmail                = DTO.HostEmail,
                HostPhone                = DTO.HostPhone,
                StudentSessionMotivation = DTO.StudentSessionMotivation
            };

            await _companyRepo.Add(company);

            return Ok(company);
        }
        /// <summary>
        /// Delete company by name
        /// </summary>
        /// <param name="name">name of company</param>
        [HttpDelete]
        [Route("{name}")]
        [Authorize(Roles = nameof(Role.Administrator))]
        [ProducesResponseType(typeof(Company), StatusCodes.Status200OK)]
        public async Task<ActionResult> DeleteCompany(string name)
        {
            Company company = await _companyRepo.FindByName(name);
            if(company == null)
            {
                return NotFound();
            }
            await _companyRepo.Remove(company);
            return Ok();
        }

    }
}

