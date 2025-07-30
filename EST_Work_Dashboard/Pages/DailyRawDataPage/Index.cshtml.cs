using EST_Work_Dashboard.Data;
using EST_Work_Dashboard.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EST_Work_Dashboard.Pages.DailyRawDataPage
{
    public class IndexModel : PageModel
    {
        private readonly DailyRawDataService _dataService;

        // 생성자에서 Service 주입
        public IndexModel(DailyRawDataService dataService)
        {
            _dataService = dataService;
        }

        // Razor 페이지에서 사용할 데이터 목록
        public List<DailyRawData> RawDataList { get; set; }
        
        // 페이지가 처음 로드될 때 실행
        public async Task OnGetAsync()
        {
            RawDataList = await _dataService.GetAllAsync();
        }

        // 삭제 핸들러 추가
        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            await _dataService.DeleteAsync(id);

            return RedirectToPage();
        }
    }
}
