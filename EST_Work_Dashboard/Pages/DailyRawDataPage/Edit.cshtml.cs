using EST_Work_Dashboard.Data;
using EST_Work_Dashboard.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Globalization;

namespace EST_Work_Dashboard.Pages.DailyRawDataPage
{
    public class EditModel : PageModel
    {
        private readonly DailyRawDataService _dataService;
        
        public EditModel(DailyRawDataService dataService)
        {
            _dataService = dataService;
        }
        
        [BindProperty]
        public DailyRawData Input { get; set; } // 사용자가 입력한 값 저장용               

        // 항목 수정 시 데이터 로드
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id.HasValue)
            {
                Input = await _dataService.GetByIdAsync(id.Value);
                if (Input == null)
                {
                    return NotFound();
                }
            }
            else
            {
                Input = new DailyRawData(); // 새로 작성 시
            }

            return Page();
        }

        // 새 항목 작성 및 수정
        public async Task<IActionResult> OnPostAsync()
        {         
            if (!ModelState.IsValid)
                return Page();

            // tkww 자동 계산
            if (Input.StartDate.HasValue)
            {
                var startDate = Input.StartDate.Value;
                CultureInfo ci = CultureInfo.InvariantCulture;
                Calendar cal = ci.Calendar;
                int weekNum = cal.GetWeekOfYear(startDate, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
                Input.ww = $"ww{weekNum:D2}";
            }
            else
            {
                Input.ww = "";
            }

            if (Input.Id > 0)
            {
                // Id가 존재하면 수정
                await _dataService.UpdateAsync(Input);
            }
            else
            {
                // Id가 없으면 새로 저장
                await _dataService.InsertAsync(Input);
            }            

            return RedirectToPage("Index"); // 저장이 완료되면 Index로 이동. GetAllAsync()를 호출해 목록 테이블에 즉시 반영
        }        
    }
}
