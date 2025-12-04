using Laboratory_Service.Application.Interface;
using Laboratory_Service.Domain.Entity;
using MediatR;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Linq;
using Unit = QuestPDF.Infrastructure.Unit;
using CommentEntity = Laboratory_Service.Domain.Entity.Comment;

namespace Laboratory_Service.Application.Test_Result.Queries
{
    /// <summary>
    /// Handler for exporting test results to PDF for printing with Watermark.
    /// Creates a professional PDF with beautiful design:
    /// 1. Elegant header with gradient
    /// 2. Diagonal watermark across the page
    /// 3. Test Order Information in styled card
    /// 4. Test Results table with professional styling
    /// 5. Comments section at the end
    /// </summary>
    public class ExportTestResultsToPdfQueryHandler : IRequestHandler<ExportTestResultsToPdfQuery, byte[]>
    {
        private readonly ITestOrderRepository _testOrderRepository;
        private readonly ICommentRepository _commentRepository;
        
        // Cấu hình watermark - có thể config từ appsettings
        private const string WATERMARK_TEXT = "LABORATORY"; // Thay bằng tên công ty của bạn
        private const int WATERMARK_FONT_SIZE = 70; // Kích thước chữ nhỏ hơn cho nhiều dòng
        private const int WATERMARK_LINE_COUNT = 1; // Số dòng watermark
        private const int WATERMARK_LINE_SPACING = 60; // Khoảng cách giữa các dòng

        public ExportTestResultsToPdfQueryHandler(ITestOrderRepository testOrderRepository, ICommentRepository commentRepository)
        {
            _testOrderRepository = testOrderRepository;
            _commentRepository = commentRepository;
        }

        public async Task<byte[]> Handle(ExportTestResultsToPdfQuery request, CancellationToken cancellationToken)
        {
            QuestPDF.Settings.License = LicenseType.Community;
            QuestPDF.Settings.EnableDebugging = true;

            var testOrder = await _testOrderRepository.GetByIdAsync(request.TestOrderId, cancellationToken);
            if (testOrder == null)
                throw new InvalidOperationException($"Test order with ID {request.TestOrderId} not found.");

            if (testOrder.IsDeleted)
                throw new InvalidOperationException($"Test order with ID {request.TestOrderId} has been deleted.");

            var validStatuses = new[] { "Completed", "Đã hoàn thành", "completed", "Hoàn thành" };
            if (!validStatuses.Contains(testOrder.Status))
                throw new InvalidOperationException($"Cannot print test results. Status must be 'Completed'. Current: {testOrder.Status}");

            var testResults = testOrder.TestResults?.ToList() ?? new List<TestResult>();
            var patient = testOrder.MedicalRecord?.Patient;
            if (patient == null)
                throw new InvalidOperationException("Patient information not found.");

            // Lấy comments cho TestOrder
            var testOrderComments = await _commentRepository.GetByTestOrderIdAsync(request.TestOrderId);

            // Lấy comments cho các TestResults
            var testResultIds = testResults.Select(tr => tr.TestResultId).ToList();
            var testResultComments = new List<CommentEntity>();
            if (testResultIds.Any())
            {
                testResultComments = await _commentRepository.GetByTestResultIdAsync(testResultIds);
            }

            var pdfBytes = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(1, Unit.Centimetre);
                    page.DefaultTextStyle(TextStyle.Default.FontFamily("Calibri").FontSize(10));

                    page.Header()
                        .Height(3, Unit.Centimetre)
                        .Background(Colors.Blue.Darken3)
                        .Padding(15)
                        .Column(column =>
                        {
                            column.Item()
                                .Text("LABORATORY TEST RESULT")
                                .FontSize(24)
                                .Bold()
                                .FontColor(Colors.White)
                                .AlignCenter();

                            column.Item()
                                .PaddingTop(3)
                                .Text("LABORATORY SERVICE REPORT")
                                .FontSize(10)
                                .FontColor(Colors.Grey.Lighten1)
                                .AlignCenter();
                        });

                    page.Content()
                        .Layers(layers =>
                        {
                            // Lớp chính chứa nội dung
                            layers.PrimaryLayer()
                                .Column(column =>
                                {
                                    column.Spacing(10);

                                    column.Item()
                                        .Element(TestOrderCard(testOrder, patient));

                                    if (testResults.Any())
                                    {
                                        column.Item()
                                            .Element(TestResultsTable(testResults));
                                    }
                                    else
                                    {
                                        column.Item()
                                            .Element(NoResultsMessage());
                                    }

                                    // Thêm phần Comments
                                    if (testOrderComments.Any() || testResultComments.Any())
                                    {
                                        column.Item()
                                            .Element(CommentsSection(testOrderComments, testResultComments, testResults));
                                    }
                                });

                            // ===== THÊM WATERMARK LAYER (CHỮ CHÌM CHÉO) =====
                            // Vẽ watermark ở layer trên cùng, phủ lên nội dung
                            layers.Layer()
                                .ExtendHorizontal()
                                .ExtendVertical()
                                .Element(DiagonalWatermark);
                        });

                    page.Footer()
                        .Height(1.5f, Unit.Centimetre)
                        .Background(Colors.Grey.Lighten4)
                        .PaddingHorizontal(15)
                        .PaddingVertical(8)
                        .Row(row =>
                        {
                            row.RelativeItem()
                                .DefaultTextStyle(TextStyle.Default.FontSize(9))
                                .Text(text =>
                                {
                                    text.Span("Printed on: ").FontColor(Colors.Grey.Darken2);
                                    text.Span(DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm")).FontColor(Colors.Grey.Darken1).Bold();
                                });

                            row.RelativeItem()
                                .AlignRight()
                                .DefaultTextStyle(TextStyle.Default.FontSize(9))
                                .Text(text =>
                                {
                                    text.Span("Page ").FontColor(Colors.Grey.Darken2);
                                    text.CurrentPageNumber().FontColor(Colors.Grey.Darken1).Bold();
                                    text.Span(" / ").FontColor(Colors.Grey.Darken2);
                                    text.TotalPages().FontColor(Colors.Grey.Darken1).Bold();
                                });
                        });
                });
            }).GeneratePdf();

            return pdfBytes;
        }

        private static Action<IContainer> TestOrderCard(TestOrder testOrder, Patient patient)
        {
            return container =>
            {
                container
                    .Background(Colors.White)
                    .Border(1)
                    .BorderColor(Colors.Blue.Lighten1)
                    .Padding(15)
                    .Column(column =>
                    {
                        column.Item()
                            .PaddingBottom(8)
                            .Text("TEST ORDER INFORMATION")
                            .FontSize(14)
                            .Bold()
                            .FontColor(Colors.Blue.Darken3);

                        column.Item()
                            .Height(1.5f)
                            .Background(Colors.Blue.Lighten2);

                        column.Item()
                            .PaddingTop(8)
                            .Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.ConstantColumn(140);
                                    columns.RelativeColumn();
                                    columns.ConstantColumn(140);
                                    columns.RelativeColumn();
                                });

                                void AddInfoRow(string label1, string value1, string label2, string value2)
                                {
                                    table.Cell().Element(InfoLabelStyle).Text(label1);
                                    table.Cell().Element(InfoValueStyle).Text(value1 ?? "N/A");
                                    table.Cell().Element(InfoLabelStyle).Text(label2);
                                    table.Cell().Element(InfoValueStyle).Text(value2 ?? "N/A");
                                }

                                table.Cell().ColumnSpan(2)
                                    .Element(InfoLabelStyle)
                                    .Text("Test Order ID:");

                                table.Cell().ColumnSpan(2)
                                    .Element(InfoValueStyle)
                                    .Text(testOrder.TestOrderId.ToString())
                                    .FontColor(Colors.Blue.Darken2)
                                    .Bold();

                                AddInfoRow("Patient Name:", patient.FullName,
                                    "Gender:", patient.Gender);

                                AddInfoRow("Date of Birth:", patient.DateOfBirth.ToString("dd/MM/yyyy"),
                                    "Phone Number:", patient.PhoneNumber);

                                AddInfoRow("Status:", testOrder.Status,
                                    "Test Type:", testOrder.TestType ?? "N/A");

                                AddInfoRow("Created By:", testOrder.CreatedBy ?? "N/A",
                                    "__", testOrder.RunBy?.ToString() ?? "__");

                                AddInfoRow("Created At:", testOrder.CreatedAt.ToString("dd/MM/yyyy HH:mm"),
                                    "__", testOrder.RunDate != default
                                        ? testOrder.RunDate.ToString("dd/MM/yyyy HH:mm")
                                        : "__");

                                if (!string.IsNullOrWhiteSpace(testOrder.Note))
                                {
                                    table.Cell().ColumnSpan(2)
                                        .Element(InfoLabelStyle)
                                        .Text("Note:");

                                    table.Cell().ColumnSpan(2)
                                        .Element(InfoValueStyle)
                                        .Text(testOrder.Note)
                                        .Italic()
                                        .FontColor(Colors.Grey.Darken1);
                                }
                            });
                    });
            };
        }

        private static Action<IContainer> TestResultsTable(List<TestResult> testResults)
        {
            return container =>
            {
                container
                    .Column(column =>
                    {
                        column.Item()
                            .PaddingBottom(8)
                            .Text("DETAILED TEST RESULTS")
                            .FontSize(14)
                            .Bold()
                            .FontColor(Colors.Blue.Darken3);

                        column.Item()
                            .Height(1.5f)
                            .Background(Colors.Blue.Lighten2);

                        column.Item()
                            .PaddingTop(8)
                            .Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.ConstantColumn(30);
                                    columns.RelativeColumn(2.5f);
                                    columns.RelativeColumn(1.8f);
                                    columns.RelativeColumn(1.2f);
                                    columns.RelativeColumn(2f);
                                    columns.RelativeColumn(1.5f);
                                });

                                table.Header(header =>
                                {
                                    header.Cell().Element(TableHeaderStyle).Text("No.").Bold().AlignCenter();
                                    header.Cell().Element(TableHeaderStyle).Text("Parameter").Bold().AlignCenter();
                                    header.Cell().Element(TableHeaderStyle).Text("Value").Bold().AlignCenter();
                                    header.Cell().Element(TableHeaderStyle).Text("Unit").Bold().AlignCenter();
                                    header.Cell().Element(TableHeaderStyle).Text("Reference Range").Bold().AlignCenter();
                                    header.Cell().Element(TableHeaderStyle).Text("Status").Bold().AlignCenter();
                                });

                                int index = 1;
                                bool isEven = false;
                                foreach (var result in testResults.OrderBy(r => r.Parameter))
                                {
                                    Func<IContainer, IContainer> rowStyle = isEven ? TableRowEvenStyle : TableRowOddStyle;

                                    table.Cell().Element(rowStyle).Text(index.ToString()).AlignCenter();
                                    table.Cell().Element(rowStyle).Text(result.Parameter ?? "N/A");

                                    var valueText = result.ValueText ??
                                                   (result.ValueNumeric.HasValue
                                                       ? result.ValueNumeric.Value.ToString("F2")
                                                       : "N/A");
                                    table.Cell().Element(rowStyle).Text(valueText).AlignCenter().Bold();

                                    table.Cell().Element(rowStyle).Text(result.Unit ?? "N/A").AlignCenter();
                                    table.Cell().Element(rowStyle).Text(result.ReferenceRange ?? "N/A").AlignCenter();

                                    var statusColor = result.ResultStatus?.ToLower() == "completed"
                                        ? Colors.Green.Darken1
                                        : Colors.Grey.Darken1;

                                    table.Cell().Element(rowStyle)
                                        .Text(result.ResultStatus ?? "N/A")
                                        .FontColor(statusColor)
                                        .AlignCenter();

                                    index++;
                                    isEven = !isEven;
                                }
                            });

                        var resultsWithComments = testResults
                            .Where(r => !string.IsNullOrWhiteSpace(r.TestCode) ||
                                       !string.IsNullOrWhiteSpace(r.Instrument) ||
                                       r.PerformedDate != default)
                            .ToList();

                        if (resultsWithComments.Any())
                        {
                            column.Item()
                                .PaddingTop(15)
                                .Element(AdditionalNotesSection(resultsWithComments));
                        }
                    });
            };
        }

        private static Action<IContainer> AdditionalNotesSection(List<TestResult> resultsWithComments)
        {
            return container =>
            {
                container
                    .Background(Colors.Grey.Lighten5)
                    .Border(1)
                    .BorderColor(Colors.Grey.Lighten2)
                    .Padding(15)
                    .Column(column =>
                    {
                        column.Item()
                            .PaddingBottom(8)
                            .Text("Additional Notes")
                            .FontSize(12)
                            .Bold()
                            .FontColor(Colors.Blue.Darken2);

                        foreach (var result in resultsWithComments)
                        {
                            var comments = new List<string>();
                            if (!string.IsNullOrWhiteSpace(result.Parameter))
                                comments.Add($"Parameter: {result.Parameter}");
                            if (!string.IsNullOrWhiteSpace(result.TestCode))
                                comments.Add($"Code: {result.TestCode}");
                            if (!string.IsNullOrWhiteSpace(result.Instrument))
                                comments.Add($"Instrument: {result.Instrument}");
                            if (result.PerformedDate != default)
                                comments.Add($"Performed: {result.PerformedDate:dd/MM/yyyy HH:mm}");

                            if (comments.Any())
                            {
                                column.Item()
                                    .PaddingBottom(5)
                                    .Text(string.Join(" • ", comments))
                                    .FontSize(9)
                                    .FontColor(Colors.Grey.Darken2);
                            }
                        }
                    });
            };
        }

        private static Action<IContainer> CommentsSection(List<CommentEntity> testOrderComments, List<CommentEntity> testResultComments, List<TestResult> testResults)
        {
            return container =>
            {
                container
                    .Background(Colors.White)
                    .Border(1)
                    .BorderColor(Colors.Orange.Lighten1)
                    .Padding(15)
                    .Column(column =>
                    {
                        column.Item()
                            .PaddingBottom(8)
                            .Text("COMMENTS & NOTES")
                            .FontSize(14)
                            .Bold()
                            .FontColor(Colors.Orange.Darken2);

                        column.Item()
                            .Height(1.5f)
                            .Background(Colors.Orange.Lighten2);

                        column.Item().PaddingTop(10);

                        // Test Order Comments
                        if (testOrderComments.Any())
                        {
                            column.Item()
                                .PaddingBottom(10)
                                .Column(subColumn =>
                                {
                                    subColumn.Item()
                                        .PaddingBottom(5)
                                        .Text("General Comments:")
                                        .FontSize(11)
                                        .Bold()
                                        .FontColor(Colors.Orange.Darken1);

                                    foreach (var comment in testOrderComments)
                                    {
                                        subColumn.Item()
                                            .PaddingBottom(8)
                                            .Background(Colors.Orange.Lighten5)
                                            .Border(1)
                                            .BorderColor(Colors.Orange.Lighten3)
                                            .Padding(10)
                                            .Column(commentColumn =>
                                            {
                                                commentColumn.Item()
                                                    .Text(comment.Message)
                                                    .FontSize(9)
                                                    .FontColor(Colors.Black);

                                                commentColumn.Item()
                                                    .PaddingTop(5)
                                                    .Row(row =>
                                                    {
                                                        row.RelativeItem()
                                                            .Text($"By: {comment.CreatedBy}")
                                                            .FontSize(8)
                                                            .Italic()
                                                            .FontColor(Colors.Grey.Darken1);

                                                        row.RelativeItem()
                                                            .AlignRight()
                                                            .Text($"{comment.CreatedDate:dd/MM/yyyy HH:mm}")
                                                            .FontSize(8)
                                                            .Italic()
                                                            .FontColor(Colors.Grey.Darken1);
                                                    });
                                            });
                                    }
                                });
                        }

                        // Test Result Comments
                        if (testResultComments.Any())
                        {
                            column.Item()
                                .Column(subColumn =>
                                {
                                    subColumn.Item()
                                        .PaddingBottom(5)
                                        .Text("Test Result Comments:")
                                        .FontSize(11)
                                        .Bold()
                                        .FontColor(Colors.Orange.Darken1);

                                    // Group comments by TestResultId
                                    var groupedComments = testResultComments.GroupBy(c => c.TestResultId);

                                    foreach (var group in groupedComments)
                                    {
                                        var testResult = testResults.FirstOrDefault(tr => tr.TestResultId == group.Key);
                                        var parameterName = testResult?.Parameter ?? "Unknown Parameter";

                                        subColumn.Item()
                                            .PaddingBottom(10)
                                            .Column(paramColumn =>
                                            {
                                                paramColumn.Item()
                                                    .PaddingBottom(3)
                                                    .Text($"• {parameterName}")
                                                    .FontSize(10)
                                                    .Bold()
                                                    .FontColor(Colors.Blue.Darken2);

                                                foreach (var comment in group.OrderByDescending(c => c.CreatedDate))
                                                {
                                                    paramColumn.Item()
                                                        .PaddingBottom(6)
                                                        .PaddingLeft(15)
                                                        .Background(Colors.Orange.Lighten5)
                                                        .Border(1)
                                                        .BorderColor(Colors.Orange.Lighten3)
                                                        .Padding(10)
                                                        .Column(commentColumn =>
                                                        {
                                                            commentColumn.Item()
                                                                .Text(comment.Message)
                                                                .FontSize(9)
                                                                .FontColor(Colors.Black);

                                                            commentColumn.Item()
                                                                .PaddingTop(5)
                                                                .Row(row =>
                                                                {
                                                                    row.RelativeItem()
                                                                        .Text($"By: {comment.CreatedBy}")
                                                                        .FontSize(8)
                                                                        .Italic()
                                                                        .FontColor(Colors.Grey.Darken1);

                                                                    row.RelativeItem()
                                                                        .AlignRight()
                                                                        .Text($"{comment.CreatedDate:dd/MM/yyyy HH:mm}")
                                                                        .FontSize(8)
                                                                        .Italic()
                                                                        .FontColor(Colors.Grey.Darken1);
                                                                });
                                                        });
                                                }
                                            });
                                    }
                                });
                        }
                    });
            };
        }

        private static Action<IContainer> NoResultsMessage()
        {
            return container =>
            {
                container
                    .Height(100)
                    .Background(Colors.Grey.Lighten5)
                    .Border(1)
                    .BorderColor(Colors.Grey.Lighten2)
                    .Padding(30)
                    .AlignCenter()
                    .AlignMiddle()
                    .Text("No test results available")
                    .FontSize(14)
                    .Italic()
                    .FontColor(Colors.Grey.Medium);
            };
        }

        private static IContainer InfoLabelStyle(IContainer container)
        {
            return container
                .PaddingVertical(6)
                .PaddingHorizontal(5)
                .DefaultTextStyle(TextStyle.Default.FontColor(Colors.Grey.Darken2).FontSize(9));
        }

        private static IContainer InfoValueStyle(IContainer container)
        {
            return container
                .PaddingVertical(6)
                .PaddingHorizontal(5)
                .DefaultTextStyle(TextStyle.Default.FontColor(Colors.Black).FontSize(9));
        }

        private static IContainer TableHeaderStyle(IContainer container)
        {
            return container
                .Background(Colors.Blue.Darken3)
                .Border(1)
                .BorderColor(Colors.Blue.Darken4)
                .PaddingVertical(8)
                .PaddingHorizontal(6)
                .DefaultTextStyle(TextStyle.Default.FontColor(Colors.White).FontSize(9));
        }

        private static IContainer TableRowOddStyle(IContainer container)
        {
            return container
                .Background(Colors.White)
                .Border(1)
                .BorderColor(Colors.Grey.Lighten2)
                .PaddingVertical(6)
                .PaddingHorizontal(6)
                .DefaultTextStyle(TextStyle.Default.FontColor(Colors.Black).FontSize(8));
        }

        private static IContainer TableRowEvenStyle(IContainer container)
        {
            return container
                .Background(Colors.Grey.Lighten5)
                .Border(1)
                .BorderColor(Colors.Grey.Lighten2)
                .PaddingVertical(6)
                .PaddingHorizontal(6)
                .DefaultTextStyle(TextStyle.Default.FontColor(Colors.Black).FontSize(8));
        }

        /// <summary>
        /// Tạo watermark chữ chìm chéo từ trên trái xuống dưới phải với nhiều dòng
        /// </summary>
        private static Action<IContainer> DiagonalWatermark => container =>
        {
            container
                .ExtendHorizontal() // Chiếm toàn bộ chiều rộng
                .ExtendVertical() // Chiếm toàn bộ chiều cao
                .AlignCenter() // Căn giữa theo chiều ngang
                .AlignMiddle() // Căn giữa theo chiều dọc
                .Element(centerContainer =>
                {
                    // Container trung gian để đảm bảo căn giữa tốt hơn
                    centerContainer
                        .AlignCenter()
                        .AlignMiddle()
                        .Element(rotateContainer =>
                        {
                            rotateContainer
                                .Rotate(-45) // Xoay -45 độ để tạo watermark chéo từ trên trái xuống dưới phải
                                .Column(column =>
                                {
                                    column.Spacing(WATERMARK_LINE_SPACING);
                                    
                                    // Tạo nhiều dòng watermark lặp lại
                                    for (int i = 0; i < WATERMARK_LINE_COUNT; i++)
                                    {
                                        column.Item()
                                            .AlignCenter()
                                            .Text(WATERMARK_TEXT)
                                            .FontSize(WATERMARK_FONT_SIZE)
                                            .Bold()
                                           .FontColor("#3Ff6b26b");
                                    }
                                });
                        });
                });
        };
    }
}