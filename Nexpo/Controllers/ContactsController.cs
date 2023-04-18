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
        [ProducesResponseType(typeof(IEnumerable<Contact>), StatusCodes.Status200OK)]
        [Authorize(Roles = nameof(Role.Administrator) + "," + nameof(Role.Volunteer))]
        public async Task<ActionResult<IEnumerable<User>>> GetContacts()
        {
            return Ok(await _contactRepo.GetAll());
        }


        /// <summary>
        /// Update a contats's information
        /// </summary>
        [HttpPut]
        [Route("{id}")]
        [Authorize(Roles = nameof(Role.Administrator))]
        [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
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

