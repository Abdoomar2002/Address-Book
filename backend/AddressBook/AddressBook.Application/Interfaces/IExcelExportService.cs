using AddressBook.Application.DTOs.Contacts;
using System;
using System.Collections.Generic;
using System.Text;

namespace AddressBook.Application.Interfaces
{
    public interface IExcelExportService
    {
        /// <summary>
        /// Renders contacts as an .xlsx workbook.
        /// </summary>
        /// <returns>The workbook bytes, ready to hand to a FileResult.</returns>
        byte[] ExportContacts(IEnumerable<ContactDto> contacts);
    }
}
