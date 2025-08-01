using EST_Work_Dashboard.Data;
using EST_Work_Dashboard.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EST_Work_Dashboard.Pages.TrainingPage
{
    public class IndexModel : PageModel
    {
        private readonly TrainingService _service;

        // �����ڿ��� Service ����
        public IndexModel(TrainingService service)
        {
            _service = service;
        }

        // Razor ���������� ����� ������ ���
        public List<TrainingModel> TrainingList { get; set; }

        // �������� ó�� �ε�� �� ����
        public async Task OnGetAsync(DateTime? start, DateTime? end, string? cp, string? manager, 
            string? site, string? plant, string? process, string? manufacturer, string? trainer)
        {
            var all = await _service.GetAllAsync();

            TrainingList = all.Where(x =>
                (!start.HasValue || x.StartDate >= start.Value) &&
                (!end.HasValue || x.EndDate <= end.Value) &&
                (string.IsNullOrEmpty(cp) || (x.CP ?? "").Contains(cp)) &&
                (string.IsNullOrEmpty(manager) || (x.Manager ?? "").Contains(manager)) &&
                (string.IsNullOrEmpty(site) || (x.Site ?? "").Contains(site)) &&                
                (string.IsNullOrEmpty(plant) || (x.Plant ?? "").Contains(plant)) &&                
                (string.IsNullOrEmpty(process) || (x.Process ?? "").Contains(process)) &&
                (string.IsNullOrEmpty(manufacturer) || (x.Eq_Manufacturer ?? "").Contains(manufacturer)) &&
                (string.IsNullOrEmpty(trainer) || (x.Trainer ?? "").Contains(trainer))
            ).ToList();
        }

        // ���� �ڵ鷯 �߰�
        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            await _service.DeleteAsync(id);

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDownloadExcelAsync()
        {
            var dataList = await _service.GetAllAsync(); // ������ �ҷ�����

            using (var workbook = new ClosedXML.Excel.XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Training");
                var currentRow = 1;

                // ��� �ۼ�                
                worksheet.Cell(currentRow, 1).Value = "Start Date";
                worksheet.Cell(currentRow, 2).Value = "End Date";
                worksheet.Cell(currentRow, 3).Value = "CP";
                worksheet.Cell(currentRow, 4).Value = "�����";
                worksheet.Cell(currentRow, 5).Value = "Site";
                worksheet.Cell(currentRow, 6).Value = "Plant";
                worksheet.Cell(currentRow, 7).Value = "����";
                worksheet.Cell(currentRow, 8).Value = "����";
                worksheet.Cell(currentRow, 9).Value = "�𵨸�";
                worksheet.Cell(currentRow, 10).Value = "���� ����";
                worksheet.Cell(currentRow, 11).Value = "���� ������";
                worksheet.Cell(currentRow, 12).Value = "���� �����";
                worksheet.Cell(currentRow, 13).Value = "Remark";

                // ������ �ۼ�
                foreach (var item in dataList)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = item.StartDate?.ToString("yyyy-MM-dd") ?? "";
                    worksheet.Cell(currentRow, 2).Value = item.EndDate?.ToString("yyyy-MM-dd") ?? "";
                    worksheet.Cell(currentRow, 3).Value = item.CP;
                    worksheet.Cell(currentRow, 4).Value = item.Manager;
                    worksheet.Cell(currentRow, 5).Value = item.Site;
                    worksheet.Cell(currentRow, 6).Value = item.Plant;
                    worksheet.Cell(currentRow, 7).Value = item.Process;
                    worksheet.Cell(currentRow, 8).Value = item.Eq_Manufacturer;
                    worksheet.Cell(currentRow, 9).Value = item.Model_Name;
                    worksheet.Cell(currentRow, 10).Value = item.Training_Content;
                    worksheet.Cell(currentRow, 11).Value = item.Trainer;
                    worksheet.Cell(currentRow, 12).Value = item.Trainee;
                    worksheet.Cell(currentRow, 13).Value = item.Remark;
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    stream.Position = 0;

                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Training.xlsx");
                }
            }
        }
    }
}
