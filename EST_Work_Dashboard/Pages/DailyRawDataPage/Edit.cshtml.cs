using EST_Work_Dashboard.Data;
using EST_Work_Dashboard.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

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
        
        public void OnGet()
        {
            // 빈 폼 표시용 (수정용일 경우에는 여기서 기존 데이터 로드)
        }
        
        public async Task<IActionResult> OnPostAsync()
        {         
            if (!ModelState.IsValid)
                return Page();

            await _dataService.InsertAsync(Input); // 새 데이터 저장

            return RedirectToPage("Index"); // 저장이 완료되면 Index로 이동. GetAllAsync()를 호출해 목록 테이블에 즉시 반영
        }
    }
}
