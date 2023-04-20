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
        /// <param name="id">The event id</param>
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
        /// <param name="id">The event id</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}/tickets")]
        [Authorize(Roles = nameof(Role.Administrator))]
        [ProducesResponseType(typeof(IEnumerable<TicketInfoDTO>), StatusCodes.Status200OK)]
        public async Task<ActionResult> GetTicketsForEvent(int id)
        {
            var _event = await _eventRepo.Get(id);
            if (_event == null) {
                return NotFound();
            }

            var tickets = await _ticketRepo.GetAllForEvent(_event.Id.Value);
            var namedTickets = new List<TicketInfoDTO>();
            foreach (var ticket in tickets){
                var user = await _userRepository.Get(ticket.UserId);
                var DTO = new TicketInfoDTO
                {
                    ticket        = ticket,
                    userFirstName = user.FirstName,
                    userLastName  = user.LastName
                };
                namedTickets.Add(DTO);
            }
            return Ok(namedTickets);
        }
        /// <summary>
        /// Update information for an Event
        /// </summary>
        /// <param name="id">The event id</param>
        /// <param name="DTO"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("{id}")]
        [Authorize(Roles = nameof(Role.Administrator))]
        [ProducesResponseType(typeof(Event), StatusCodes.Status200OK)]
        public async Task<ActionResult> UpdateEvent(int id, AddEventDTO DTO)
        {

            var _event = await _eventRepo.Get(id);

            if(!string.IsNullOrEmpty(DTO.Name)){
                _event.Name = DTO.Name; 
            }
            if(!string.IsNullOrEmpty(DTO.Description)){
                _event.Description = DTO.Description; 
            }
            if(!string.IsNullOrEmpty(DTO.Date)){
                _event.Date = DTO.Date; 
            }
            if(!string.IsNullOrEmpty(DTO.Start)){
                _event.Start = DTO.Start; 
            }
            if(!string.IsNullOrEmpty(DTO.End)){
                _event.End = DTO.End; 
            }
            if(!string.IsNullOrEmpty(DTO.Location)){
                _event.Location = DTO.Location; 
            }
            if(!string.IsNullOrEmpty(DTO.Host)){
                _event.Host = DTO.Host; 
            }
            if(!string.IsNullOrEmpty(DTO.Language)){
                _event.Language = DTO.Language; 
            }
            if(DTO.Capacity != 0){
                _event.Capacity = DTO.Capacity; 
            }
            await _eventRepo.Update(_event);

            return Ok(_event);
        }

        /// <summary>
        /// Add a new event
        /// </summary>
        [HttpPost]
        [Authorize(Roles = nameof(Role.Administrator))]
        [ProducesResponseType(typeof(Event), StatusCodes.Status200OK)]
        public async Task<ActionResult> AddNewEvent(AddEventDTO DTO)
        {
            DateTime date;
            if(DateTime.TryParse(DTO.Date, out date) && DateTime.TryParse(DTO.Start, out date) && DateTime.TryParse(DTO.End, out date))
            {
                var _event = new Event
                {
                    Name        = DTO.Name,
                    Description = DTO.Description,
                    Date        = DTO.Date,
                    Start       = DTO.Start,
                    End         = DTO.End,
                    Location    = DTO.Location,
                    Host        = DTO.Host,
                    Language    = DTO.Language,
                    Capacity    = DTO.Capacity
                };
                await _eventRepo.Add(_event);

                return Ok(_event);
            }
            
            return BadRequest();
        }
    }


}

