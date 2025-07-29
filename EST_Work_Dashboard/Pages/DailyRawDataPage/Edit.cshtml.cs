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
        public DailyRawData Input { get; set; } // ����ڰ� �Է��� �� �����
        
        public void OnGet()
        {
            // �� �� ǥ�ÿ� (�������� ��쿡�� ���⼭ ���� ������ �ε�)
        }
        
        public async Task<IActionResult> OnPostAsync()
        {         
            if (!ModelState.IsValid)
                return Page();

            await _dataService.InsertAsync(Input); // �� ������ ����

            return RedirectToPage("Index"); // ������ �Ϸ�Ǹ� Index�� �̵�. GetAllAsync()�� ȣ���� ��� ���̺� ��� �ݿ�
        }
    }
}
