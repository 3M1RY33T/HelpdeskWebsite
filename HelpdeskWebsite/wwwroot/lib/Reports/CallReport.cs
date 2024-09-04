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
    public class CallReport
    {
        public void GenerateCallReport(string rootpath)
        {
            CallViewModel viewmodel = new();
            List<Call> calls = new List<Call>();
            try
            {
                calls = viewmodel.GetAll().Result.Select(t => new Call
                {
                    Id = t.Id,
                    DateOpened = t.DateOpened,
                    Employee = new Employee {
                        LastName = (t.EmployeeName ?? string.Empty).Split(' ').LastOrDefault()
                    },
                    Tech = new Employee {
                        LastName = (t.TechName ?? string.Empty).Split(' ').LastOrDefault()
                    },
                    Problem = new Problem {
                        Description = t.ProblemDescription
                    },
                    OpenStatus = t.OpenStatus,
                    DateClosed = t.DateClosed
                }).ToList();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " ");
            }

            PageSize pg = PageSize.A4.Rotate();
            var helvetica = PdfFontFactory.CreateFont(StandardFontFamilies.HELVETICA);
            PdfWriter writer = new(rootpath + "/pdfs/callreport.pdf",
            new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0));
            PdfDocument pdf = new(writer);
            Document document = new(pdf, pg); 

            document.Add(new Image(ImageDataFactory.Create(rootpath + "/img/img1.png"))
                .ScaleAbsolute(100, 100)
                .SetFixedPosition((pg.GetWidth() / 2 - 50), 480));

            document.Add(new Paragraph("\n"));
            document.Add(new Paragraph("\n"));
            document.Add(new Paragraph("\n"));
            document.Add(new Paragraph("Current Calls")
                .SetFont(helvetica)
                .SetFontSize(24)
                .SetBold()
                .SetTextAlignment(TextAlignment.CENTER));
            document.Add(new Paragraph(""));
            document.Add(new Paragraph(""));
            Table table = new(6);
            table
                .SetWidth(650) // roughly 50%
                .SetTextAlignment(TextAlignment.CENTER)
                .SetRelativePosition(0, 0, 0, 0)
                .SetHorizontalAlignment(HorizontalAlignment.CENTER);
            table.AddCell(new Cell().Add(new Paragraph("Opened")
                .SetFontSize(16)
                .SetBold()
                .SetPaddingLeft(18)
                .SetTextAlignment(TextAlignment.CENTER))
                .SetBorder(Border.NO_BORDER));
            table.AddCell(new Cell().Add(new Paragraph("Last Name")
                .SetFontSize(16)
                .SetBold()
                .SetPaddingLeft(16)
                .SetTextAlignment(TextAlignment.CENTER))
                .SetBorder(Border.NO_BORDER));
            table.AddCell(new Cell().Add(new Paragraph("Tech")
                .SetFontSize(16)
                .SetBold()
                .SetPaddingLeft(16)
                .SetTextAlignment(TextAlignment.CENTER))
                .SetBorder(Border.NO_BORDER));
            table.AddCell(new Cell().Add(new Paragraph("Problem")
                .SetBold()
                .SetFontSize(16)
                .SetPaddingLeft(16)
                .SetTextAlignment(TextAlignment.CENTER))
                .SetBorder(Border.NO_BORDER));
            table.AddCell(new Cell().Add(new Paragraph("Status")
                .SetBold()
                .SetFontSize(16)
                .SetPaddingLeft(16)
                .SetTextAlignment(TextAlignment.CENTER))
                .SetBorder(Border.NO_BORDER));
            table.AddCell(new Cell().Add(new Paragraph("Closed")
                .SetBold()
                .SetFontSize(16)
                .SetPaddingLeft(16)
                .SetTextAlignment(TextAlignment.CENTER))
                .SetBorder(Border.NO_BORDER));

            // Print the list of students
            foreach (var call in calls)
            {
                table.AddCell(new Cell().Add(new Paragraph(call.DateOpened.ToShortDateString())
                .SetFontSize(14)
               .SetPaddingLeft(24)
               .SetTextAlignment(TextAlignment.CENTER))
               .SetBorder(Border.NO_BORDER)); ; ;
                table.AddCell(new Cell().Add(new Paragraph(call.Employee.LastName)
                .SetFontSize(14)
               .SetPaddingLeft(24)
               .SetTextAlignment(TextAlignment.CENTER))
               .SetBorder(Border.NO_BORDER));
                table.AddCell(new Cell().Add(new Paragraph(call.Tech.LastName)
                .SetFontSize(14)
               .SetPaddingLeft(24)
               .SetTextAlignment(TextAlignment.CENTER))
               .SetBorder(Border.NO_BORDER));
                table.AddCell(new Cell().Add(new Paragraph(call.Problem.Description)
                .SetFontSize(14)
               .SetPaddingLeft(24)
               .SetTextAlignment(TextAlignment.CENTER))
               .SetBorder(Border.NO_BORDER));
                table.AddCell(new Cell().Add(new Paragraph(call.OpenStatus? "Closed": "Open")
                .SetFontSize(14)
               .SetPaddingLeft(24)
               .SetTextAlignment(TextAlignment.CENTER))
               .SetBorder(Border.NO_BORDER));
                table.AddCell(new Cell().Add(new Paragraph(call.DateClosed == null? "-" : call.DateClosed.Value.ToShortDateString())
                .SetFontSize(14)
               .SetPaddingLeft(24)
               .SetTextAlignment(TextAlignment.CENTER))
               .SetBorder(Border.NO_BORDER));
            }

            document.Add(table);

            document.Add(new Paragraph("Call report written on - " + DateTime.Now)
                .SetFontSize(6)
                .SetTextAlignment(TextAlignment.CENTER));

            document.Close();
        }

    }
}
