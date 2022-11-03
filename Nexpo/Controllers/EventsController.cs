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

        /// <summary>
        /// Returns all food preferences of event. 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}/foodprefs")]
        [Authorize(Roles = nameof(Role.CompanyRepresentative))]
        [ProducesResponseType(typeof(IEnumerable<EventFoodDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult> GetFoodPrefForEvent(int id)
        {
            var e = await _eventRepo.Get(id);
            if (e == null)
            {
                return NotFound();
            }

            var tickets = await _ticketRepo.GetAllForEvent(e.Id.Value);

            var foodPreferences = (from t in tickets
                                  select _userRepository.Get(t.UserId).Result.FoodPreferences)
                                  .ToArray();

            var uniqueWithCount = foodPreferences.GroupBy(z => z)
                                                    .Select(g => new EventFoodDto { Preference = (g.Key != null) ? g.Key : "Inget", Count = g.Count() });

            return Ok(uniqueWithCount);
        }
        
        /// <summary>
        /// Update information for an Event
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("{id}")]
        [Authorize(Roles = nameof(Role.Administrator))]
        [ProducesResponseType(typeof(Event), StatusCodes.Status200OK)]
        public async Task<ActionResult> UpdateEvent(int id, AddEventDto dto)
        {

            var even = await _eventRepo.Get(id);

            if(!string.IsNullOrEmpty(dto.Name)){
                even.Name = dto.Name; 
            }
            if(!string.IsNullOrEmpty(dto.Description)){
                even.Description = dto.Description; 
            }
            if(!string.IsNullOrEmpty(dto.Date)){
                even.Date = dto.Date; 
            }
            if(!string.IsNullOrEmpty(dto.Start)){
                even.Start = dto.Start; 
            }
            if(!string.IsNullOrEmpty(dto.End)){
                even.End = dto.End; 
            }
            if(!string.IsNullOrEmpty(dto.Location)){
                even.Location = dto.Location; 
            }
            if(!string.IsNullOrEmpty(dto.Host)){
                even.Host = dto.Host; 
            }
            if(!string.IsNullOrEmpty(dto.Language)){
                even.Language = dto.Language; 
            }
            if(dto.Capacity != 0){
                even.Capacity = dto.Capacity; 
            }
            await _eventRepo.Update(even);

            return Ok(even);
        }

        [HttpPost]
        [Authorize(Roles = nameof(Role.Administrator))]
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

