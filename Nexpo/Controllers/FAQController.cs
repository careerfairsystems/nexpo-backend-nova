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

        public FAQController(IFAQRepository iQuestions)
        {
            _questionsRepo = iQuestions;
        }

        /// <summary>
        /// Get a list of all FAQ
        /// </summary>
        [Authorize(Roles = nameof(Role.Administrator) + "," + nameof(Role.Volunteer))]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<FrequentAskedQuestion>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var faq = await _questionsRepo.GetAll();
            return Ok(faq);
        }

        /// <summary>
        /// Get information about a FAQ
        /// </summary>
        [Authorize(Roles = nameof(Role.Administrator) + "," + nameof(Role.Volunteer))]
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(FrequentAskedQuestion), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetFAQ(int id)
        {
            var faq = await _questionsRepo.Get(id);

            if (faq == null)
            {
                return NotFound();
            }

            return Ok(faq);
        }


        /// <summary>
        /// Update FAQ
        /// </summary>
        [HttpPut]
        [Route("{id}")]
        [Authorize(Roles = nameof(Role.Administrator))]
        [ProducesResponseType(typeof(FrequentAskedQuestion), StatusCodes.Status200OK)]
        public async Task<IActionResult> PutFAQ(int id, UpdateFAQDTO dto)
        {
            var faq = await _questionsRepo.Get(id);

            if (faq == null)
            {
                return NotFound();
            }

            if (dto.Question != null)
            {
                faq.Question = dto.Question;
            }

            if (dto.Answer != null)
            {
                faq.Answer = dto.Answer;
            }

            await _questionsRepo.Update(faq);

            return Ok(faq);
        }

        /// <summary>
        /// Create a FAQ
        /// </summary>
        [HttpPost]
        [Route("add")]
        [Authorize(Roles = nameof(Role.Administrator))]
        [ProducesResponseType(typeof(FrequentAskedQuestion), StatusCodes.Status201Created)]
        public async Task<IActionResult> PostFAQ(CreateFAQDTO dto)
        {
            if(await _questionsRepo.Get(dto.Id) != null)
            {
                return BadRequest();
            }

            var faq = new FrequentAskedQuestion
            {
                Id = dto.Id,
                Question = dto.Question,
                Answer = dto.Answer
            };

            await _questionsRepo.Add(faq);

            return CreatedAtAction(nameof(GetFAQ), new { id = faq.Id }, faq);
        }

        /// <summary>
        /// Delete a FAQ
        /// </summary>
        [HttpDelete]
        [Route("{id}")]
        [Authorize(Roles = nameof(Role.Administrator))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteFAQ(int id)
        {
            var faq = await _questionsRepo.Get(id);

            if (faq == null)
            {
                return NotFound();
            }

            await _questionsRepo.Remove(faq);

            return NoContent();
        }
    }
}

