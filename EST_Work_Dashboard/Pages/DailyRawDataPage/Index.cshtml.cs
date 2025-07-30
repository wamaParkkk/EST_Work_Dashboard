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
        public async Task OnGetAsync()
        {
            RawDataList = await _dataService.GetAllAsync();
        }

        // ���� �ڵ鷯 �߰�
        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            await _dataService.DeleteAsync(id);

            return RedirectToPage();
        }
    }
}
