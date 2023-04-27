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
    public class FAQController : ControllerBase
    {
        private readonly IFAQRepository _questionsRepo;

        public FAQController(IFAQRepository iquestions)
        {
            _questionsRepo = iquestions;
        }

        /// <summary>
        /// Get a list of all contacts
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<FrequentAskedQuestion>), StatusCodes.Status200OK)]
        [Authorize(Roles = nameof(Role.Administrator) + "," + nameof(Role.Volunteer))]
        public async Task<ActionResult> GetAll()
        {
            var questions = await _questionsRepo.GetAll();
            return Ok(questions);
        }

        /// <summary>
        /// Get information about a single contact
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Roles = nameof(Role.Administrator) + "," + nameof(Role.Volunteer))]
        [ProducesResponseType(typeof(FrequentAskedQuestion), StatusCodes.Status200OK)]
        public async Task<ActionResult> GetFAQ(int id)
        {
            var question = await _questionsRepo.Get(id);

            if (question == null)
            {
                return NotFound();
            }

            return Ok(question);
        }


        /// <summary>
        /// Update a contats's information
        /// </summary>
        [HttpPut]
        [Route("{id}")]
        [Authorize(Roles = nameof(Role.Administrator))]
        [ProducesResponseType(typeof(FrequentAskedQuestion), StatusCodes.Status200OK)]
        public async Task<IActionResult> PutFAQ(int id, UpdateFAQDTO dto)
        {
            var question = await _questionsRepo.Get(id);

            if (question == null)
            {
                return NotFound();
            }

            if (dto.Id == null)
            {
                question.Id = dto.Id;
            }

            if (dto.Question == null)
            {
                question.Question = dto.Question;
            }

            await _questionsRepo.Update(question);

            return Ok(question);
        }

        /// <summary>
        /// Create a new contact
        /// </summary>
        [HttpPost]
        [Route("add")]
        [Authorize(Roles = nameof(Role.Administrator))]
        [ProducesResponseType(typeof(FrequentAskedQuestion), StatusCodes.Status201Created)]
        public async Task<ActionResult> PostFAQ(CreateFAQDTO dto)
        {
            var frequentAskedQuestion = new FrequentAskedQuestion
            {
                Id = dto.Id,
                Question = dto.Question
            };



            await _questionsRepo.Add(frequentAskedQuestion);

            return Ok(frequentAskedQuestion);
        }

        /// <summary>
        /// Delete a contact
        /// </summary>
        [HttpDelete]
        [Route("{id}")]
        [Authorize(Roles = nameof(Role.Administrator))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteFAQ(int id)
        {
            var question = await _questionsRepo.Get(id);

            if (question == null)
            {
                return NotFound();
            }

            await _questionsRepo.Remove(question);

            return NoContent();
        }
    }
}

