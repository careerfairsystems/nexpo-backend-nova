﻿using System;
using System.Collections.Generic;
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
    public class TicketsController : ControllerBase
    {
        private readonly ITicketRepository _ticketRepo;
        private readonly IEventRepository _eventRepo;
        private readonly IUserRepository _userRepo;

        public TicketsController(ITicketRepository iTicketRepo, IEventRepository iEventRepo, IUserRepository iUserRepo)
        {
            _ticketRepo = iTicketRepo;
            _eventRepo  = iEventRepo;
            _userRepo   = iUserRepo;
        }

        /// <summary>
        /// Get all tickets for the signed in user
        /// </summary>
        [HttpGet]
        [Authorize]
        [ProducesResponseType(typeof(IEnumerable<Ticket>), StatusCodes.Status200OK)]
        public async Task<ActionResult> GetTickets()
        {
            var userId = HttpContext.User.GetId();
            var tickets = await _ticketRepo.GetAllForUser(userId);
            return Ok(tickets);
        }

        /// <summary>
        /// Create a new ticket, for the user, to an event
        /// </summary>
        [HttpPost]
        [Authorize(Roles = nameof(Role.Student) + "," + nameof(Role.CompanyRepresentative))]
        [ProducesResponseType(typeof(Ticket), StatusCodes.Status201Created)]
        public async Task<ActionResult> PostTicket(CreateTicketDTO DTO)
        {
            var _event = await _eventRepo.Get(DTO.EventId);
            if (_event == null)
            {
                return NotFound();
            }

            if ((DateTime.Parse(_event.Date) - DateTime.Today).TotalDays < 2) 
            {
                return BadRequest();
            }

            // Only allow a user to register once
            var userId = HttpContext.User.GetId();
            if (await _ticketRepo.TicketExists(DTO.EventId, userId))
            {
                return Conflict();
            }

            var ticket = new Ticket
            {
                PhotoOk = DTO.PhotoOk,
                EventId = DTO.EventId,
                UserId  = userId,
            };

            if(!await _ticketRepo.Add(ticket))
            {
                return Conflict();
            }

            return CreatedAtAction(nameof(GetTicket), new { id = ticket.Id }, ticket);
        }

        /// <summary>
        /// Get the type of ticket
        /// </summary>
        [HttpGet]
        [Route("{id}/type")]
        [Authorize]
        [ProducesResponseType(typeof(EventType), StatusCodes.Status200OK)]
        public async Task<ActionResult> GetTicketType(int id)
        {
            var ticket = await _ticketRepo.Get(id);
            
            if (ticket == null)
            {
                return NotFound();
            }

            var userId = HttpContext.User.GetId();

            if(ticket.UserId != userId)
            {
                return Forbid();
            }

            return Ok(await _ticketRepo.GetEventType(id));

            
        }

        /// <summary>
        /// Create new ticket as admin to an event. Ignores capacity and startTime
        /// </summary>
        [HttpPost]
        [Route("add")]
        [Authorize(Roles = nameof(Role.Administrator))]
        [ProducesResponseType(typeof(Ticket), StatusCodes.Status201Created)]
        public async Task<ActionResult> PostTicketAdmin(CreateTicketAdminDTO DTO)
        {
            var _event = await _eventRepo.Get(DTO.EventId);
            if (_event == null)
            {
                return NotFound();
            }

            // Only allow a user to register once
            if (await _ticketRepo.TicketExists(DTO.EventId, DTO.UserId))
            {
                return Conflict();
            }

            var ticket = new Ticket
            {
                PhotoOk = DTO.PhotoOk,
                EventId = DTO.EventId,
                UserId  = DTO.UserId,
            };

            await _ticketRepo.AddAdmin(ticket);

            return CreatedAtAction(nameof(GetTicket), new { id = ticket.Id }, ticket);
        } 

        /// <summary>
        /// Update isConsumed on a ticket
        /// <param name="id">Id of the ticket</param>
        /// </summary>
        [HttpPut]
        [Route("{id}")]
        [Authorize(Roles = nameof(Role.Administrator))]
        [ProducesResponseType(typeof(Ticket), StatusCodes.Status200OK)]
        public async Task<ActionResult> PutTicket(int id, UpdateTicketDTO DTO)
        {
            var ticket = await _ticketRepo.Get(id);

            ticket.isConsumed = DTO.isConsumed;

            if (DTO.TakeAway != null)
            {
                ticket.TakeAway = DTO.TakeAway;
            }

            if (DTO.TakeAwayTime != null)
            {
                ticket.TakeAwayTime = DTO.TakeAwayTime;
            }


            await _ticketRepo.Update(ticket);

            return Ok(ticket);
        }

        /// <summary>
        /// Get a specific ticket by Guid (Global Unique Identifier)
        /// </summary>
        /// <param name="id">Globally Unique ID (GUID) of the ticket</param>
        [HttpGet]
        [Route("{id}")]
        [Authorize(Roles = nameof(Role.Administrator))]
        [ProducesResponseType(typeof(Ticket), StatusCodes.Status200OK)]
        public async Task<ActionResult<Ticket>> GetTicket(Guid id)
        {
            var ticket = await _ticketRepo.GetByCode(id);

            if (ticket != null)
            {
                return Ok(ticket);
            }
            else
            {
                return NotFound();
            }
        }

        /// <summary>
        /// Get a specific ticket by id 
        /// </summary>
        /// <param name="id">Id of the ticket</param>
        [HttpGet]
        [Route("id/{id}")]
        [Authorize]
        [ProducesResponseType(typeof(Ticket), StatusCodes.Status200OK)]
        public async Task<ActionResult<Ticket>> GetTicket(int id)
        {
            var ticket = await _ticketRepo.Get(id);
            var userId = HttpContext.User.GetId();
            var userRole = HttpContext.User.GetRole();

            if (ticket != null && (ticket.UserId == userId || userRole == Role.Administrator))
            {
                return Ok(ticket);
            }
            else
            {
                return NotFound();
            }
        }


        /// <summary>
        /// Delete/Unregister a ticket
        /// </summary>
        /// <param name="id">Id of the ticket</param>
        [HttpDelete]
        [Route("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> DeleteTicket(int id)
        {
            var ticket = await _ticketRepo.Get(id);
            if (ticket == null)
            {
                return NotFound();
            }

            var userId = HttpContext.User.GetId();
            var userRole = HttpContext.User.GetRole();

            if(userRole != Role.Administrator)
            {
                var _event = await _eventRepo.Get(ticket.EventId);
                if ((DateTime.Parse(_event.Date) - DateTime.Today).TotalDays < 2)
                {
                    return BadRequest();
                }

                if (ticket.UserId != userId || ticket.isConsumed)
                {
                    return Forbid();
                }
            }
            
            await _ticketRepo.Remove(ticket);
            return NoContent();
        }
    }
}

