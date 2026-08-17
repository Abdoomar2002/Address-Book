using AddressBook.Application.DTOs.Lookups;
using AddressBook.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AddressBook.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/jobtitles")]
    public class JobTitlesController : ControllerBase
    {
        private readonly IJobTitleService _service;

        public JobTitlesController(IJobTitleService service)
        {
            _service = service;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyList<JobTitleDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<JobTitleDto>>> GetAll(CancellationToken cancellationToken)
        {
            return Ok(await _service.GetAllAsync(cancellationToken));
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(JobTitleDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<JobTitleDto>> GetById(Guid id, CancellationToken cancellationToken)
        {
            return Ok(await _service.GetByIdAsync(id, cancellationToken));
        }

        [HttpPost]
        [ProducesResponseType(typeof(JobTitleDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<JobTitleDto>> Create(JobTitleSaveDto dto, CancellationToken cancellationToken)
        {
            var created = await _service.CreateAsync(dto, cancellationToken);

            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(JobTitleDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<JobTitleDto>> Update(Guid id, JobTitleSaveDto dto, CancellationToken cancellationToken)
        {
            return Ok(await _service.UpdateAsync(id, dto, cancellationToken));
        }

        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            await _service.DeleteAsync(id, cancellationToken);

            return NoContent();
        }
    }
}
