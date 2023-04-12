using System;
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
            _eventRepo      = iEventRepo;
            _ticketRepo     = iTicketRepo;
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
        /// <param name="id"></param>
        /// </summary>
        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(typeof(Event), StatusCodes.Status200OK)]
        public async Task<ActionResult> GetEvent(int id)
        {
            var _event = await _eventRepo.Get(id);

            if (_event == null) 
            {
                return NotFound();
            }

            return Ok(_event);
        }

        /// <summary>
        /// Get all tickets for an event
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}/tickets")]
        [Authorize(Roles = nameof(Role.Administrator))]
        [ProducesResponseType(typeof(IEnumerable<NamedTicketDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult> GetTicketsForEvent(int id)
        {
            var _event = await _eventRepo.Get(id);
            if (_event == null) {
                return NotFound();
            }

            var tickets = await _ticketRepo.GetAllForEvent(_event.Id.Value);
            var namedTickets = new List<NamedTicketDto>();
            foreach (var ticket in tickets){
                var user = await _userRepository.Get(ticket.UserId);
                var dto = new NamedTicketDto
                {
                    ticket        = ticket,
                    userFirstName = user.FirstName,
                    userLastName  = user.LastName
                };
                namedTickets.Add(dto);
            }
            return Ok(namedTickets);
        }
        /// <summary>
        /// Update information for an Event
        /// </summary>
        /// <param name="id">The event id</param>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("{id}")]
        [Authorize(Roles = nameof(Role.Administrator))]
        [ProducesResponseType(typeof(Event), StatusCodes.Status200OK)]
        public async Task<ActionResult> UpdateEvent(int id, AddEventDto dto)
        {

            var _event = await _eventRepo.Get(id);

            if(!string.IsNullOrEmpty(dto.Name)){
                _event.Name = dto.Name; 
            }
            if(!string.IsNullOrEmpty(dto.Description)){
                _event.Description = dto.Description; 
            }
            if(!string.IsNullOrEmpty(dto.Date)){
                _event.Date = dto.Date; 
            }
            if(!string.IsNullOrEmpty(dto.Start)){
                _event.Start = dto.Start; 
            }
            if(!string.IsNullOrEmpty(dto.End)){
                _event.End = dto.End; 
            }
            if(!string.IsNullOrEmpty(dto.Location)){
                _event.Location = dto.Location; 
            }
            if(!string.IsNullOrEmpty(dto.Host)){
                _event.Host = dto.Host; 
            }
            if(!string.IsNullOrEmpty(dto.Language)){
                _event.Language = dto.Language; 
            }
            if(dto.Capacity != 0){
                _event.Capacity = dto.Capacity; 
            }
            await _eventRepo.Update(_event);

            return Ok(_event);
        }

        /// <summary>
        /// Add a new event
        /// </summary>
        /// <param name="dto"></param>
        [HttpPost]
        [Authorize(Roles = nameof(Role.Administrator))]
        [ProducesResponseType(typeof(Event), StatusCodes.Status200OK)]
        public async Task<ActionResult> AddNewEvent(AddEventDto dto)
        {
            DateTime date;
            if(DateTime.TryParse(dto.Date, out date) && DateTime.TryParse(dto.Start, out date) && DateTime.TryParse(dto.End, out date))
            {
                var _event = new Event
                {
                    Name        = dto.Name,
                    Description = dto.Description,
                    Date        = dto.Date,
                    Start       = dto.Start,
                    End         = dto.End,
                    Location    = dto.Location,
                    Host        = dto.Host,
                    Language    = dto.Language,
                    Capacity    = dto.Capacity
                };
                await _eventRepo.Add(_event);

                return Ok(_event);
            }
            
            return BadRequest();
        }
    }


}

