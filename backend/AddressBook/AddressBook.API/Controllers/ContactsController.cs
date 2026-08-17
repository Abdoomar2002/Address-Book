using AddressBook.Application.DTOs.Contacts;
using AddressBook.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AddressBook.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/contacts")]
    public class ContactsController : ControllerBase
    {
        private const string ExcelContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

        private readonly IContactService _service;
        private readonly IExcelExportService _excelExportService;

        public ContactsController(IContactService service, IExcelExportService excelExportService)
        {
            _service = service;
            _excelExportService = excelExportService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyList<ContactDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<ContactDto>>> GetAll(
            [FromQuery] ContactFilterDto filter,
            CancellationToken cancellationToken)
        {
            return Ok(await _service.GetAllAsync(filter, cancellationToken));
        }

        /// <summary>
        /// Exports the same result set as <see cref="GetAll"/> - identical filter, identical
        /// service call - as an .xlsx workbook.
        /// </summary>
        [HttpGet("export")]
        // Without Produces, the document advertises text/plain and Swagger UI renders the
        // workbook as text instead of offering it as a download.
        [Produces(ExcelContentType)]
        [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> Export(
            [FromQuery] ContactFilterDto filter,
            CancellationToken cancellationToken)
        {
            var contacts = await _service.GetAllAsync(filter, cancellationToken);
            var bytes = _excelExportService.ExportContacts(contacts);

            return File(bytes, ExcelContentType, "AddressBook.xlsx");
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(ContactDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ContactDto>> GetById(Guid id, CancellationToken cancellationToken)
        {
            return Ok(await _service.GetByIdAsync(id, cancellationToken));
        }

        [HttpPost]
        [ProducesResponseType(typeof(ContactDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ContactDto>> Create(ContactSaveDto dto, CancellationToken cancellationToken)
        {
            var created = await _service.CreateAsync(dto, cancellationToken);

            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(ContactDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ContactDto>> Update(Guid id, ContactSaveDto dto, CancellationToken cancellationToken)
        {
            return Ok(await _service.UpdateAsync(id, dto, cancellationToken));
        }

        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            await _service.DeleteAsync(id, cancellationToken);

            return NoContent();
        }
    }
}
