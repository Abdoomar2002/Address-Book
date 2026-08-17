using AddressBook.Application.DTOs.Lookups;
using AddressBook.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AddressBook.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/departments")]
    public class DepartmentsController : ControllerBase
    {
        private readonly IDepartmentService _service;

        public DepartmentsController(IDepartmentService service)
        {
            _service = service;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyList<DepartmentDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<DepartmentDto>>> GetAll(CancellationToken cancellationToken)
        {
            return Ok(await _service.GetAllAsync(cancellationToken));
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(DepartmentDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DepartmentDto>> GetById(Guid id, CancellationToken cancellationToken)
        {
            return Ok(await _service.GetByIdAsync(id, cancellationToken));
        }

        [HttpPost]
        [ProducesResponseType(typeof(DepartmentDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<DepartmentDto>> Create(DepartmentSaveDto dto, CancellationToken cancellationToken)
        {
            var created = await _service.CreateAsync(dto, cancellationToken);

            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(DepartmentDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<DepartmentDto>> Update(Guid id, DepartmentSaveDto dto, CancellationToken cancellationToken)
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
