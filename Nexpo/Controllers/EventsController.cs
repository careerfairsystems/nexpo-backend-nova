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
    public class EventsController : ControllerBase
    {
        private readonly IEventRepository _eventRepo;
        private readonly ITicketRepository _ticketRepo;

        public EventsController(IEventRepository iEventRepo, ITicketRepository iTicketRepo)
        {
            _eventRepo = iEventRepo;
            _ticketRepo = iTicketRepo;
        }

        /// <summary>
        /// Get a list of all events
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Event>), StatusCodes.Status200OK)]
        public async Task<ActionResult> GetAll()
        {
            return Ok(await _eventRepo.GetAll());
        }

        /// <summary>
        /// Get a specific event
        /// </summary>
        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(typeof(Event), StatusCodes.Status200OK)]
        public async Task<ActionResult> GetEvent(int id)
        {
            var e = await _eventRepo.Get(id);
            if (e == null) 
            {
                return NotFound();
            }

            return Ok(e);
        }

        /// <summary>
        /// Get all tickets for 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}/tickets")]
        [Authorize(Roles = nameof(Role.Administrator))]
        [ProducesResponseType(typeof(IEnumerable<NamedTicketDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult> GetTicketsForEvent(int id)
        {
            var e = await _eventRepo.Get(id);
            if (e == null) {
                return NotFound();
            }

            var tickets = await _ticketRepo.GetAllForEvent(e.Id.Value);
            IEnumerable<NamedTicketDto> namedTickets = tickets.Select(t => new NamedTicketDto
            {
                ticket = t,
                userFirstName = t.User.FirstName,
                userLastName = t.User.LastName
            });
            return Ok(namedTickets);
        }

    }
}

