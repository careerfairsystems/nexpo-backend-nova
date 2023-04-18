using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nexpo.DTO;
using Nexpo.Models;
using Nexpo.Repositories;

namespace Nexpo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactsController : ControllerBase
    {
        private readonly IContactReposity _contactRepo;

        public ContactsController(
            IContactReposity iContactRepo
            )
        {
            _contactRepo = iContactRepo;
        }

        /// <summary>
        /// Get a list of all contacts
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(List<Contact>), StatusCodes.Status200OK)]
        [Authorize(Roles = nameof(Role.Administrator) + "," + nameof(Role.Volunteer))]
        public async Task<ActionResult<List<Contact>>> GetAll()
        {
            var contacts = await _contactRepo.GetAll();
            return Ok(contacts);
        }

        /// <summary>
        /// Get information about a single contact
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Roles = nameof(Role.Administrator) + "," + nameof(Role.Volunteer))]
        [ProducesResponseType(typeof(Contact), StatusCodes.Status200OK)]
        public async Task<ActionResult<Contact>> GetContact(int id)
        {
            var contact = await _contactRepo.Get(id);

            if (contact == null)
            {
                return NotFound();
            }

            return Ok(contact);
        }


        /// <summary>
        /// Update a contats's information
        /// </summary>
        [HttpPut]
        [Route("{id}")]
        [Authorize(Roles = nameof(Role.Administrator))]
        [ProducesResponseType(typeof(Contact), StatusCodes.Status200OK)]
        public async Task<IActionResult> PutContact(int id, UpdateContactDTO dto)
        {
            var contact = await _contactRepo.Get(id);

            if (contact == null)
            {
                return NotFound();
            }

            if(dto.FirstName != null)
            {
                contact.FirstName = dto.FirstName;
            }
            if(dto.LastName != null)
            {
                contact.LastName = dto.LastName;
            }
            if(dto.Email != null)
            {
                contact.Email = dto.Email;
            }

            await _contactRepo.Update(contact);

            return Ok(contact);
        }

        /// <summary>
        /// Create a new contact
        /// </summary>
        [HttpPost]
        [Authorize(Roles = nameof(Role.Administrator))]
        [ProducesResponseType(typeof(Contact), StatusCodes.Status201Created)]
        public async Task<ActionResult<Contact>> PostContact(CreateContactDTO dto)
        {
            var contact = new Contact
            {
                FirstName   = dto.FirstName,
                LastName    = dto.LastName,
                Email       = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                RoleInArkad = dto.RoleInArkad
            };

            await _contactRepo.Add(contact);

            return Ok(contact);
        }

        /// <summary>
        /// Delete a contact
        /// </summary>
        [HttpDelete]
        [Route("{id}")]
        [Authorize(Roles = nameof(Role.Administrator))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteContact(int id)
        {
            var contact = await _contactRepo.Get(id);

            if (contact == null)
            {
                return NotFound();
            }

            await _contactRepo.Remove(contact);

            return NoContent();
        }
    }
}

