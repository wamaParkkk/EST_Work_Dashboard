using EST_Work_Dashboard.Data;
using EST_Work_Dashboard.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EST_Work_Dashboard.Pages.DailyRawDataPage
{
    public class IndexModel : PageModel
    {
        private readonly DailyRawDataService _dataService;

        // �����ڿ��� Service ����
        public IndexModel(DailyRawDataService dataService)
        {
            _dataService = dataService;
        }

        // Razor ���������� ����� ������ ���
        public List<DailyRawData> RawDataList { get; set; }

        // �������� ó�� �ε�� �� ����
        public async Task OnGetAsync(string? ww, DateTime? start, DateTime? end, string? cp, string? manager, string? classification, string? line, string? process, string? model, string? mc)
        {
            var all = await _dataService.GetAllAsync();

            RawDataList = all.Where(x =>                
                (string.IsNullOrEmpty(ww) || x.ww == ww) &&
                (!start.HasValue || x.StartDate >= start.Value) &&
                (!end.HasValue || x.EndDate <= end.Value) &&
                (string.IsNullOrEmpty(cp) || x.CP.Contains(cp)) &&
                (string.IsNullOrEmpty(manager) || x.Manager.Contains(manager)) &&
                (string.IsNullOrEmpty(classification) || x.Classification.Contains(classification)) &&
                (string.IsNullOrEmpty(line) || x.Line.Contains(line)) &&
                (string.IsNullOrEmpty(process) || x.Process.Contains(process)) &&
                (string.IsNullOrEmpty(model) || x.Model_Name.Contains(model)) &&
                (string.IsNullOrEmpty(mc) || x.MC.Contains(mc))
            ).ToList();
        }

        // ���� �ڵ鷯 �߰�
        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            await _dataService.DeleteAsync(id);

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDownloadExcelAsync()
        {
            var dataList = await _dataService.GetAllAsync(); // ������ �ҷ�����

            using (var workbook = new ClosedXML.Excel.XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Daily Raw Data");
                var currentRow = 1;

                // ��� �ۼ�
                worksheet.Cell(currentRow, 1).Value = "ww";
                worksheet.Cell(currentRow, 2).Value = "Start Date";
                worksheet.Cell(currentRow, 3).Value = "End Date";
                worksheet.Cell(currentRow, 4).Value = "CP";
                worksheet.Cell(currentRow, 5).Value = "�����";
                worksheet.Cell(currentRow, 6).Value = "�з�";
                worksheet.Cell(currentRow, 7).Value = "Site";
                worksheet.Cell(currentRow, 8).Value = "Plant";
                worksheet.Cell(currentRow, 9).Value = "Line";
                worksheet.Cell(currentRow, 10).Value = "����";
                worksheet.Cell(currentRow, 11).Value = "����";
                worksheet.Cell(currentRow, 12).Value = "�𵨸�";
                worksheet.Cell(currentRow, 13).Value = "����ȣ";
                worksheet.Cell(currentRow, 14).Value = "�˶�/���� ����";
                worksheet.Cell(currentRow, 15).Value = "��ġ ����";
                worksheet.Cell(currentRow, 16).Value = "���� ����";
                worksheet.Cell(currentRow, 17).Value = "Status";
                worksheet.Cell(currentRow, 18).Value = "TTL Q'ty";
                worksheet.Cell(currentRow, 19).Value = "Done Q'ty";
                worksheet.Cell(currentRow, 20).Value = "Remark";
                worksheet.Cell(currentRow, 21).Value = "Q'ty";
                worksheet.Cell(currentRow, 22).Value = "Outsourced Cost ($K)/Set";
                worksheet.Cell(currentRow, 23).Value = "In-house Cost ($K)/Set";
                worksheet.Cell(currentRow, 24).Value = "Cost Save ($K)/Set";
                worksheet.Cell(currentRow, 25).Value = "TTL Cost Save ($K)";                

                // ������ �ۼ�
                foreach (var item in dataList)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = item.ww;
                    worksheet.Cell(currentRow, 2).Value = item.StartDate?.ToString("yyyy-MM-dd") ?? "";
                    worksheet.Cell(currentRow, 3).Value = item.EndDate?.ToString("yyyy-MM-dd") ?? "";
                    worksheet.Cell(currentRow, 4).Value = item.CP;
                    worksheet.Cell(currentRow, 5).Value = item.Manager;
                    worksheet.Cell(currentRow, 6).Value = item.Classification;
                    worksheet.Cell(currentRow, 7).Value = item.Site;
                    worksheet.Cell(currentRow, 8).Value = item.Plant;
                    worksheet.Cell(currentRow, 9).Value = item.Line;
                    worksheet.Cell(currentRow, 10).Value = item.Process;
                    worksheet.Cell(currentRow, 11).Value = item.Eq_Manufacturer;
                    worksheet.Cell(currentRow, 12).Value = item.Model_Name;
                    worksheet.Cell(currentRow, 13).Value = item.MC;
                    worksheet.Cell(currentRow, 14).Value = item.Problem_Description;
                    worksheet.Cell(currentRow, 15).Value = item.Actions;
                    worksheet.Cell(currentRow, 16).Value = item.Cause;
                    worksheet.Cell(currentRow, 17).Value = item.Status;
                    worksheet.Cell(currentRow, 18).Value = item.TTL_Qty;
                    worksheet.Cell(currentRow, 19).Value = item.Done_Qty;
                    worksheet.Cell(currentRow, 20).Value = item.Remark;
                    worksheet.Cell(currentRow, 21).Value = item.Qty;
                    worksheet.Cell(currentRow, 22).Value = item.Outsourced_Cost;
                    worksheet.Cell(currentRow, 23).Value = item.Inhouse_Cost;
                    worksheet.Cell(currentRow, 24).Value = item.Cost_Save;
                    worksheet.Cell(currentRow, 25).Value = item.TTL_Cost_Save;
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    stream.Position = 0;

                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "DailyRawData.xlsx");
                }
            }
        }
    }
}
