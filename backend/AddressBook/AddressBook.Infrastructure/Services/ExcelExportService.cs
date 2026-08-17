using AddressBook.Application.DTOs.Contacts;
using AddressBook.Application.Interfaces;
using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AddressBook.Infrastructure.Services
{
    public class ExcelExportService : IExcelExportService
    {
        private const string WorksheetName = "Contacts";
        private const string DateFormat = "dd/MM/yyyy";

        private static readonly string[] Headers =
        {
            "Full Name",
            "Job Title",
            "Department",
            "Mobile",
            "Email",
            "Date of Birth",
            "Age",
            "Address"
        };

        public byte[] ExportContacts(IEnumerable<ContactDto> contacts)
        {
            contacts ??= Enumerable.Empty<ContactDto>();

            using var workbook = new XLWorkbook();
            var sheet = workbook.Worksheets.Add(WorksheetName);

            for (var column = 0; column < Headers.Length; column++)
            {
                sheet.Cell(1, column + 1).Value = Headers[column];
            }

            var headerRow = sheet.Row(1);
            headerRow.Style.Font.Bold = true;

            // Keep the header visible while scrolling the data.
            sheet.SheetView.FreezeRows(1);

            var row = 2;
            foreach (var contact in contacts)
            {
                sheet.Cell(row, 1).Value = contact.FullName;
                sheet.Cell(row, 2).Value = contact.JobTitleName;
                sheet.Cell(row, 3).Value = contact.DepartmentName;

                // Written as text: mobile numbers keep their leading zero and are not
                // reinterpreted as a number by Excel.
                sheet.Cell(row, 4).SetValue(contact.MobileNumber).Style.NumberFormat.Format = "@";

                sheet.Cell(row, 5).Value = contact.Email;
                sheet.Cell(row, 6).Value = contact.DateOfBirth;
                sheet.Cell(row, 7).Value = contact.Age;
                sheet.Cell(row, 8).Value = contact.Address;

                row++;
            }

            if (row > 2)
            {
                sheet.Range(2, 6, row - 1, 6).Style.DateFormat.Format = DateFormat;
            }

            sheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);

            return stream.ToArray();
        }
    }
}
