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
        private readonly IUserRepository _userRepository;

        public EventsController(
            IEventRepository iEventRepo,
            ITicketRepository iTicketRepo,
            IUserRepository iUserRepository
            )
        {
            _eventRepo = iEventRepo;
            _ticketRepo = iTicketRepo;
            _userRepository = iUserRepository;
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
            var namedTickets = new List<NamedTicketDto>();
            foreach (var t in tickets){
                var user = await _userRepository.Get(t.UserId);
                var dto = new NamedTicketDto
                {
                    ticket = t,
                    userFirstName = user.FirstName,
                    userLastName = user.LastName
                };
                namedTickets.Add(dto);
            }
            return Ok(namedTickets);
        }


        [HttpPost]
        //[Authorize(Roles = nameof(Role.Administrator))]
        [ProducesResponseType(typeof(Event), StatusCodes.Status200OK)]
        public async Task<ActionResult> AddNewEvent(AddEventDto dto)
        {

            var even = new Event
            {
                Name = dto.Name,
                Description = dto.Description,
                Date = dto.Date,
                Start = dto.Start,
                End = dto.End,
                Location = dto.Location,
                Host = dto.Host,
                Language = dto.Language,
                Capacity = dto.Capacity
            };
            await _eventRepo.Add(even);

            return Ok(even);
        }
    }


}

