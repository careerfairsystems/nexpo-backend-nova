using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nexpo.DTO;
using Nexpo.Models;
using Nexpo.Repositories;
using Nexpo.Services;

namespace Nexpo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {

        private readonly NotifyService _notifyService;

        public NotificationController(NotifyService notifyService)
        {
            _notifyService = notifyService;
        }

        /// <summary>
        /// The api that the admin can use to send notifications to all users
        /// </summary>
        [HttpPut]
        [Route("{id}")]
        [Authorize(Roles = nameof(Role.Administrator))]
        [ProducesResponseType(typeof(Contact), StatusCodes.Status200OK)]
        public async Task<IActionResult> NotifyAll(NotificationDTO dto){
            _notifyService.NotifyAll(dto);
            return Ok();

        }
        
    }
    
}

