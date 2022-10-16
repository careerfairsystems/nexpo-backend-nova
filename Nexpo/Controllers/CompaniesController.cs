using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        [ProducesResponseType(typeof(IEnumerable<PublicCompanyDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult> GetCompanies()
        {
            var companies = await _companyRepo.GetAll();
            var publicCompanies = companies.Select(c => new PublicCompanyDto
            {
                Id = c.Id.Value,
                Name = c.Name,
                Description = c.Description,
                DidYouKnow = c.DidYouKnow,
                Website = c.Website,
                LogoUrl = c.LogoUrl,
                DesiredDegrees = c.DesiredDegrees,
                DesiredGuilds = c.DesiredGuilds,
                Positions = c.Positions,
                Industries = c.Industries
            });

            return Ok(publicCompanies);
        }

        /// <summary>
        /// Get a single comapny
        /// </summary>
        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(typeof(PublicCompanyDto), StatusCodes.Status200OK)]
        public async Task<ActionResult> GetCompany(int id)
        {
            var company = await _companyRepo.Get(id);
            if (company == null) 
            {
                return NotFound();
            }

            var publicCompany = new PublicCompanyDto
            {
                Id = company.Id.Value,
                Name = company.Name,
                Description = company.Description,
                DidYouKnow = company.DidYouKnow,
                Website = company.Website,
                LogoUrl = company.LogoUrl,
                DesiredDegrees = company.DesiredDegrees,
                DesiredGuilds = company.DesiredGuilds,
                Positions = company.Positions,
                Industries = company.Industries

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
        public async Task<ActionResult> PutCompany(int id, UpdateCompanyDto dto)
        {
            var company = await _companyRepo.Get(id);

            if (!string.IsNullOrEmpty(dto.Description))
            {
                company.Description = dto.Description;
            }
            if (!string.IsNullOrEmpty(dto.DidYouKnow))
            {
                company.DidYouKnow = dto.DidYouKnow;
            }
            if (!string.IsNullOrEmpty(dto.Website))
            {
                company.Website = dto.Website;
            }
            if (!string.IsNullOrEmpty(dto.HostEmail))
            {
                company.HostEmail = dto.HostEmail;
            }
            if (!string.IsNullOrEmpty(dto.HostName))
            {
                company.HostName = dto.HostName;
            }
            if (!string.IsNullOrEmpty(dto.HostPhone))
            {
                company.HostPhone = dto.HostPhone;
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
        public async Task<ActionResult> PutMe(UpdateCompanySelfDto dto)
        {
            var companyId = HttpContext.User.GetCompanyId().Value;
            var company = await _companyRepo.Get(companyId);

            if (!string.IsNullOrEmpty(dto.Description))
            {
                company.Description = dto.Description;
            }
            if (!string.IsNullOrEmpty(dto.DidYouKnow))
            {
                company.DidYouKnow = dto.DidYouKnow;
            }
            if (!string.IsNullOrEmpty(dto.Website))
            {
                company.Website = dto.Website;
            }
            await _companyRepo.Update(company);

            return Ok(company);
        }

         /// <summary>
        /// Add new company
        /// </summary>
        [HttpPut]
        [Route("add")]
        [Authorize(Roles = nameof(Role.Administrator))]
        [ProducesResponseType(typeof(Company), StatusCodes.Status200OK)]
        public async Task<ActionResult> AddNewCompany(AddCompanyDto dto)
        {
            
            var company = new Company {
                Name = dto.Name,
                Description = dto.Description,
                DidYouKnow = dto.DidYouKnow,
                LogoUrl = dto.LogoUrl,
                Website = dto.Website
            };
            await _companyRepo.Add(company);

            return Ok(company);
        }
    }
}

