using iText.IO.Font.Constants;
using iText.IO.Image;
using System.Diagnostics;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using HelpdeskViewModels;
using HelpdeskDAL;

namespace HelpdeskWebsite.Reports
{
    public class EmployeeReport
    {
        public void GenerateEmployeeReport(string rootpath)
        {
            EmployeeViewModel viewmodel = new();
            List<Employee> employees = new List<Employee>();
            try
            {
                employees = viewmodel.GetAll().Result.Select(t => new Employee
                {
                    Id = t.Id.GetValueOrDefault(0),
                    FirstName = t.Firstname,
                    LastName = t.Lastname,
                    Title = t.Title
                }).ToList();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " ");
            }

            PageSize pg = PageSize.A4;
            var helvetica = PdfFontFactory.CreateFont(StandardFontFamilies.HELVETICA);
            PdfWriter writer = new(rootpath + "/pdfs/employeereport.pdf",
            new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0));
            PdfDocument pdf = new(writer);
            Document document = new(pdf); // PageSize(595, 842)

            document.Add(new Image(ImageDataFactory.Create(rootpath + "/img/img1.png"))
                .ScaleAbsolute(100, 100)
                .SetFixedPosition((pg.GetWidth()/ 2 - 50), 730));

            document.Add(new Paragraph("\n"));
            document.Add(new Paragraph("\n"));
            document.Add(new Paragraph("\n"));
            document.Add(new Paragraph("Current Employees")
                .SetFont(helvetica)
                .SetFontSize(24)
                .SetBold()
                .SetTextAlignment(TextAlignment.CENTER));
            document.Add(new Paragraph(""));
            document.Add(new Paragraph(""));
            Table table = new(3);
            table
                .SetWidth(298) // roughly 50%
                .SetTextAlignment(TextAlignment.CENTER)
                .SetRelativePosition(0, 0, 0, 0)
                .SetHorizontalAlignment(HorizontalAlignment.CENTER);
            table.AddCell(new Cell().Add(new Paragraph("Title")
                .SetFontSize(16)
                .SetBold()
                .SetPaddingLeft(18)
                .SetTextAlignment(TextAlignment.CENTER))
                .SetBorder(Border.NO_BORDER));
            table.AddCell(new Cell().Add(new Paragraph("First Name")
                .SetFontSize(16)
                .SetBold()
                .SetPaddingLeft(16)
                .SetTextAlignment(TextAlignment.CENTER))
                .SetBorder(Border.NO_BORDER));
            table.AddCell(new Cell().Add(new Paragraph("Last Name")
                .SetBold()
                .SetFontSize(16)
                .SetPaddingLeft(16)
                .SetTextAlignment(TextAlignment.CENTER))
                .SetBorder(Border.NO_BORDER));

            // Print the list of students
            foreach (var employee in employees)
            {
                table.AddCell(new Cell().Add(new Paragraph(employee.Title)
                .SetFontSize(14)
               .SetPaddingLeft(24)
               .SetTextAlignment(TextAlignment.CENTER))
               .SetBorder(Border.NO_BORDER));
                table.AddCell(new Cell().Add(new Paragraph(employee.FirstName)
                .SetFontSize(14)
               .SetPaddingLeft(24)
               .SetTextAlignment(TextAlignment.CENTER))
               .SetBorder(Border.NO_BORDER));
                table.AddCell(new Cell().Add(new Paragraph(employee.LastName)
                .SetFontSize(14)
               .SetPaddingLeft(24)
               .SetTextAlignment(TextAlignment.CENTER))
               .SetBorder(Border.NO_BORDER));
            }

            document.Add(table);

            document.Add(new Paragraph("Employee report written on - " + DateTime.Now)
                .SetFontSize(6)
                .SetTextAlignment(TextAlignment.CENTER));

            document.Close();
        }

    }
}
