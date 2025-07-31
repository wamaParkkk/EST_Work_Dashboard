using EST_Work_Dashboard.Data;
using EST_Work_Dashboard.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EST_Work_Dashboard.Pages.EqSupportOverview
{
    public class IndexModel : PageModel
    {
        private readonly EqSupportOverviewService _service;

        // 생성자에서 Service 주입
        public IndexModel(EqSupportOverviewService service)
        {
            _service = service;
        }

        // Razor 페이지에서 사용할 데이터 목록
        public List<EqSupportOverviewModel> OverviewList { get; set; }

        public async Task OnGetAsync(string? classification, string? cp, string? manager, string? process, string? equipment, string? equipmentNo, DateTime? down, DateTime? recovery)
        {
            var all = await _service.GetAllAsync();

            OverviewList = all.Where(x =>
                (string.IsNullOrEmpty(classification) || (x.Classification ?? "").Contains(classification)) &&
                (string.IsNullOrEmpty(cp) || (x.CP ?? "").Contains(cp)) &&
                (string.IsNullOrEmpty(manager) || (x.Manager ?? "").Contains(manager)) &&
                (string.IsNullOrEmpty(process) || (x.Process ?? "").Contains(process)) &&
                (string.IsNullOrEmpty(equipment) || (x.Equipment ?? "").Contains(equipment)) &&
                (string.IsNullOrEmpty(equipmentNo) || (x.EquipmentNo ?? "").Contains(equipmentNo)) &&
                (!down.HasValue || x.Down_Date >= down.Value) &&
                (!recovery.HasValue || x.Recovery_Date <= recovery.Value)
            ).ToList();
        }

        // 삭제 핸들러 추가
        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            await _service.DeleteAsync(id);

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDownloadExcelAsync()
        {
            var dataList = await _service.GetAllAsync(); // 데이터 불러오기

            using (var workbook = new ClosedXML.Excel.XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Equipment Support Overview");
                var currentRow = 1;

                // 헤더 작성
                worksheet.Cell(currentRow, 1).Value = "분류";
                worksheet.Cell(currentRow, 2).Value = "CP";
                worksheet.Cell(currentRow, 3).Value = "담당자";
                worksheet.Cell(currentRow, 4).Value = "공정";
                worksheet.Cell(currentRow, 5).Value = "장비";
                worksheet.Cell(currentRow, 6).Value = "장비번호";
                worksheet.Cell(currentRow, 7).Value = "Down 사유";
                worksheet.Cell(currentRow, 8).Value = "조치 내용";
                worksheet.Cell(currentRow, 9).Value = "현황";
                worksheet.Cell(currentRow, 10).Value = "Down 날짜";
                worksheet.Cell(currentRow, 11).Value = "복구 날짜";
                worksheet.Cell(currentRow, 12).Value = "Down 시간 (Hr)";
                worksheet.Cell(currentRow, 13).Value = "조치 인원";                

                // 데이터 작성
                foreach (var item in dataList)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = item.Classification;                    
                    worksheet.Cell(currentRow, 2).Value = item.CP;
                    worksheet.Cell(currentRow, 3).Value = item.Manager;
                    worksheet.Cell(currentRow, 4).Value = item.Process;
                    worksheet.Cell(currentRow, 5).Value = item.Equipment;
                    worksheet.Cell(currentRow, 6).Value = item.EquipmentNo;
                    worksheet.Cell(currentRow, 7).Value = item.Down_Reason;
                    worksheet.Cell(currentRow, 8).Value = item.Actions;
                    worksheet.Cell(currentRow, 9).Value = item.Status;
                    worksheet.Cell(currentRow, 10).Value = item.Down_Date?.ToString("yyyy-MM-dd") ?? ""; ;
                    worksheet.Cell(currentRow, 11).Value = item.Recovery_Date?.ToString("yyyy-MM-dd") ?? "";
                    worksheet.Cell(currentRow, 12).Value = item.Down_Time;
                    worksheet.Cell(currentRow, 13).Value = item.Technician;                    
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    stream.Position = 0;

                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "EquipmentSupportOverview.xlsx");
                }
            }
        }
    }
}
