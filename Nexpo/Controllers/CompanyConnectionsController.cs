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
    public class CompanyConnectionsController : ControllerBase
    {
        private readonly ICompanyConnectionRepository _connectionRepo;

        public CompanyConnectionsController(ICompanyConnectionRepository iCompanyConnectionRepo)
        {
            _connectionRepo = iCompanyConnectionRepo;
        }
        
        /// <summary>
        /// Create a connection, initated by company
        /// </summary>
        [HttpPost]
        [Authorize(Roles = nameof(Role.CompanyRepresentative))]
        [ProducesResponseType(typeof(CompanyCompanyConnectionDto), StatusCodes.Status200OK)]
        public async Task<ActionResult> PutConnection(CreateCompanyConnectionDto dto)
        {
            var companyId = HttpContext.User.GetCompanyId().Value;

            CompanyConnection connection;

            if (await _connectionRepo.ConnectionExists(dto.StudentId, companyId))
            {
                connection = await _connectionRepo.GetForStudentAndCompany(dto.StudentId, companyId);
                connection.Comment = dto.Comment;
                connection.Rating = dto.Rating;
                await _connectionRepo.Update(connection);
            }
            else
            {
                connection = new CompanyConnection
                {
                    Rating = dto.Rating,
                    Comment = dto.Comment,
                    StudentId = dto.StudentId,
                    CompanyId = companyId
                };
                await _connectionRepo.Add(connection);
            }

            var companyConnection = new CompanyCompanyConnectionDto
            {
                Id = connection.Id.Value,
                Rating = connection.Rating,
                Comment = connection.Comment,
                StudentId = connection.StudentId
            };

            return Ok(companyConnection);
        }

        /// <summary>
        /// Get all connections made as student
        /// </summary>
        [HttpGet]
        [Route("student")]
        [Authorize(Roles = nameof(Role.Student))]
        [ProducesResponseType(typeof(IEnumerable<StudentCompanyConnectionDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult> GetStudentConnections()
        {
            var studentId = HttpContext.User.GetStudentId().Value;
            var connections = await _connectionRepo.GetAllForStudent(studentId);
            var studentConnections = connections.Select(c => new StudentCompanyConnectionDto
            {
                Id = c.Id.Value,
                CompanyId = c.CompanyId
            });
            return Ok(studentConnections);
        }

        /// <summary>
        /// Get all connections made as a company
        /// </summary>
        [HttpGet]
        [Route("company")]
        [Authorize(Roles = nameof(Role.CompanyRepresentative))]
        [ProducesResponseType(typeof(IEnumerable<CompanyCompanyConnectionDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult> GetCompanyConnections()
        {
            var companyId = HttpContext.User.GetCompanyId().Value;
            var connections = await _connectionRepo.GetAllForCompany(companyId);
            var companyConnections = connections.Select(c => new CompanyCompanyConnectionDto
            {
                Id = c.Id.Value,
                Rating = c.Rating,
                Comment = c.Comment,
                StudentId = c.StudentId
            });
            return Ok(companyConnections);
        }
        
        /// <summary>
        /// Modify a connection made, eg change comment or rating
        /// </summary>
        [HttpPut]
        [Route("{id}")]
        [Authorize(Roles = nameof(Role.CompanyRepresentative))]
        [ProducesResponseType(typeof(CompanyCompanyConnectionDto), StatusCodes.Status200OK)]
        public async Task<ActionResult> PutConnection(int id, UpdateCompanyConnectionDto dto)
        {
            var companyId = HttpContext.User.GetCompanyId().Value;
            var companyConnection = await _connectionRepo.Get(id);

            if (companyConnection == null)
            {
                return NotFound();
            }

            if (companyConnection.CompanyId != companyId)
            {
                return Forbid();
            }

            // Update the allowed fields
            companyConnection.Rating = dto.Rating;
            companyConnection.Comment = dto.Comment;

            await _connectionRepo.Update(companyConnection);
            return Ok(dto);
        }

        /// <summary>
        /// Delete a connection, can be done by both company and student
        /// </summary>
        [HttpDelete]
        [Route("{id}")]
        [Authorize(Roles = nameof(Role.Student) + "," + nameof(Role.CompanyRepresentative))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> DeleteConnection(int id)
        {
            var connection = await _connectionRepo.Get(id);
            if (connection == null)
            {
                return NotFound();
            }

            var userRole = HttpContext.User.GetRole();
            if (userRole == Role.Student)
            {
                var studentId = HttpContext.User.GetStudentId().Value;
                if (connection.StudentId != studentId)
                {
                    return Forbid();
                }
            }
            if (userRole == Role.CompanyRepresentative)
            {
                var companyId = HttpContext.User.GetCompanyId().Value;
                if (connection.CompanyId != companyId)
                {
                    return Forbid();
                }
            }

            await _connectionRepo.Remove(connection);
            return NoContent();
        }
    }
}

