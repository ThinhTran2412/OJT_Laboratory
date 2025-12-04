using Laboratory_Service.Application.Interface;
using Laboratory_Service.Domain.Entity;
using MediatR;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using OfficeOpenXml.Table;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Laboratory_Service.Application.TestOrders.Queries
{
    /// <summary>
    /// Handler for exporting selected test orders or all orders from current month.
    /// Implements business rules:
    /// - If TestOrderIds is null/empty, export all test orders from CURRENT MONTH
    /// - If TestOrderIds has values, export ONLY those specific orders
    /// - Run By and Run On columns are empty if Status is not "Completed"
    /// - File naming: "Test Orders-{PatientName}-{ExportDate}.xlsx" or custom name
    /// </summary>
    public class ExportSelectedTestOrdersQueryHandler : IRequestHandler<ExportSelectedTestOrdersQuery, byte[]>
    {
        private readonly ITestOrderRepository _testOrderRepository;

        public ExportSelectedTestOrdersQueryHandler(ITestOrderRepository testOrderRepository)
        {
            _testOrderRepository = testOrderRepository;
        }

        public async Task<byte[]> Handle(ExportSelectedTestOrdersQuery request, CancellationToken cancellationToken)
        {
            // Set EPPlus license context
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            // 1. Get all test orders for this patient first
            var allOrders = await _testOrderRepository.GetAllByPatientIdAsync(request.PatientId, cancellationToken);

            List<TestOrder> orders;

            // 2. CRITICAL: Filter based on selection (per policy)
            if (request.TestOrderIds != null && request.TestOrderIds.Any())
            {
                // Export ONLY selected orders
                orders = allOrders
                    .Where(o => request.TestOrderIds.Contains(o.TestOrderId))
                    .OrderByDescending(o => o.CreatedAt)
                    .ToList();

                Console.WriteLine($"[Export] Exporting {orders.Count} selected test orders out of {request.TestOrderIds.Count} requested");
            }
            else
            {
                // Export all orders from CURRENT MONTH (per policy)
                var now = DateTime.UtcNow;
                var firstDayOfMonth = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
                var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1).AddHours(23).AddMinutes(59).AddSeconds(59);

                orders = allOrders
                    .Where(o => o.CreatedAt >= firstDayOfMonth && o.CreatedAt <= lastDayOfMonth)
                    .OrderByDescending(o => o.CreatedAt)
                    .ToList();

                Console.WriteLine($"[Export] No selection - Exporting all test orders from {now:yyyy-MM} ({orders.Count} orders)");
            }

            // Get patient name for filename
            var patientName = orders.FirstOrDefault()?.MedicalRecord?.Patient?.FullName ?? "Patient";

            // 3. Create Excel package
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Test Orders");
            worksheet.View.ShowGridLines = false;
            worksheet.Cells.Style.Font.Name = "Segoe UI";
            worksheet.Cells.Style.Font.Size = 10;
            worksheet.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            var headers = new[]
            {
                "Test Order Id",
                "Patient Name",
                "Gender",
                "Date of Birth",
                "Phone Number",
                "Status",
                "Created By",
                "Created On",
                "Run By",
                "Run On"
            };

            var columnCount = headers.Length;

            // Title row
            var currentRow = 1;
            var titleRange = worksheet.Cells[currentRow, 1, currentRow, columnCount];
            titleRange.Merge = true;
            titleRange.Value = request.TestOrderIds != null && request.TestOrderIds.Any()
                ? $"Selected Patient Test Orders ({orders.Count} orders)"
                : $"Patient Test Orders - {DateTime.UtcNow:MMMM yyyy} ({orders.Count} orders)";
            titleRange.Style.Font.Bold = true;
            titleRange.Style.Font.Size = 16;
            titleRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            titleRange.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            titleRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
            titleRange.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(48, 84, 150));
            titleRange.Style.Font.Color.SetColor(Color.White);
            titleRange.Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.FromArgb(48, 84, 150));

            // Metadata row
            currentRow++;
            worksheet.Row(currentRow).Height = 20;
            worksheet.Cells[currentRow, 1].Value = "Patient Name:";
            worksheet.Cells[currentRow, 1].Style.Font.Bold = true;
            worksheet.Cells[currentRow, 2].Value = patientName;
            worksheet.Cells[currentRow, 3].Value = "Exported On:";
            worksheet.Cells[currentRow, 3].Style.Font.Bold = true;
            worksheet.Cells[currentRow, 4].Value = DateTime.UtcNow;
            worksheet.Cells[currentRow, 4].Style.Numberformat.Format = "yyyy-mm-dd HH:mm";

            if (request.TestOrderIds != null && request.TestOrderIds.Any())
            {
                worksheet.Cells[currentRow, 6].Value = "Export Type:";
                worksheet.Cells[currentRow, 6].Style.Font.Bold = true;
                worksheet.Cells[currentRow, 7].Value = $"Selected ({orders.Count} orders)";
            }

            currentRow += 2; // Blank spacer row before table

            // 4. Set headers with styling
            var headerRow = currentRow;
            for (int i = 0; i < headers.Length; i++)
            {
                var cell = worksheet.Cells[headerRow, i + 1];
                cell.Value = headers[i];
                cell.Style.Font.Bold = true;
                cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                cell.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(79, 129, 189));
                cell.Style.Font.Color.SetColor(Color.White);
                cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                cell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            }

            worksheet.Row(headerRow).Height = 28;

            // Column widths and formats
            worksheet.Column(1).Width = 38; // GUID is longer
            worksheet.Column(2).Width = 26;
            worksheet.Column(3).Width = 12;
            worksheet.Column(4).Width = 16;
            worksheet.Column(4).Style.Numberformat.Format = "yyyy-mm-dd";
            worksheet.Column(5).Width = 18;
            worksheet.Column(6).Width = 14;
            worksheet.Column(7).Width = 20;
            worksheet.Column(8).Width = 20;
            worksheet.Column(8).Style.Numberformat.Format = "yyyy-mm-dd HH:mm";
            worksheet.Column(9).Width = 16;
            worksheet.Column(10).Width = 20;
            worksheet.Column(10).Style.Numberformat.Format = "yyyy-mm-dd HH:mm";

            // 5. Populate data rows
            int row = headerRow + 1;
            foreach (var order in orders)
            {
                var patient = order.MedicalRecord?.Patient;
                var isCompleted = order.Status?.Equals("Completed", StringComparison.OrdinalIgnoreCase) == true;

                worksheet.Cells[row, 1].Value = order.TestOrderId.ToString();
                worksheet.Cells[row, 2].Value = patient?.FullName ?? string.Empty;
                worksheet.Cells[row, 3].Value = patient?.Gender ?? string.Empty;
                
                if (patient?.DateOfBirth != null)
                {
                    worksheet.Cells[row, 4].Value = patient.DateOfBirth;
                }
                else
                {
                    worksheet.Cells[row, 4].Value = string.Empty;
                }
                
                worksheet.Cells[row, 5].Value = patient?.PhoneNumber ?? string.Empty;
                worksheet.Cells[row, 6].Value = order.Status ?? string.Empty;
                worksheet.Cells[row, 7].Value = order.CreatedBy ?? string.Empty;
                worksheet.Cells[row, 8].Value = order.CreatedAt;

                // Business Rule: Run By and Run On are empty if Status is not "Completed"
                if (isCompleted)
                {
                    worksheet.Cells[row, 9].Value = order.RunBy?.ToString() ?? string.Empty;
                    if (order.RunDate != default && order.RunDate != DateTime.MinValue)
                    {
                        worksheet.Cells[row, 10].Value = order.RunDate;
                    }
                    else
                    {
                        worksheet.Cells[row, 10].Value = string.Empty;
                    }
                }
                else
                {
                    worksheet.Cells[row, 9].Value = string.Empty;
                    worksheet.Cells[row, 10].Value = string.Empty;
                }

                // Zebra striping
                var dataRowRange = worksheet.Cells[row, 1, row, headers.Length];
                if ((row - (headerRow + 1)) % 2 == 0)
                {
                    dataRowRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    dataRowRange.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(235, 241, 222));
                }

                row++;
            }

            // Add table formatting
            var lastRow = Math.Max(row - 1, headerRow);
            var tableRange = worksheet.Cells[headerRow, 1, lastRow, headers.Length];
            var table = worksheet.Tables.Add(tableRange, $"TestOrdersTable_{Guid.NewGuid():N}");
            table.TableStyle = TableStyles.Medium9;

            // Table borders
            var tableBorder = tableRange.Style.Border;
            tableBorder.Top.Style = ExcelBorderStyle.Thin;
            tableBorder.Bottom.Style = ExcelBorderStyle.Thin;
            tableBorder.Left.Style = ExcelBorderStyle.Thin;
            tableBorder.Right.Style = ExcelBorderStyle.Thin;
            tableBorder.Top.Color.SetColor(Color.FromArgb(155, 194, 230));
            tableBorder.Bottom.Color.SetColor(Color.FromArgb(155, 194, 230));
            tableBorder.Left.Color.SetColor(Color.FromArgb(155, 194, 230));
            tableBorder.Right.Color.SetColor(Color.FromArgb(155, 194, 230));

            // Auto-fit columns
            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

            // Freeze header row
            worksheet.View.FreezePanes(headerRow + 1, 1);

            // Return Excel file as byte array
            return package.GetAsByteArray();
        }
    }
}