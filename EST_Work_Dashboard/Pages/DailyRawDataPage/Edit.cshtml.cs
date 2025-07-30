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
        
        // �� �׸� �ۼ� �� ����
        public async Task<IActionResult> OnPostAsync()
        {         
            if (!ModelState.IsValid)
                return Page();

            if (Input.Id > 0)
            {
                // Id�� �����ϸ� ����
                await _dataService.UpdateAsync(Input);
            }
            else
            {
                // Id�� ������ ���� ����
                await _dataService.InsertAsync(Input);
            }            

            return RedirectToPage("Index"); // ������ �Ϸ�Ǹ� Index�� �̵�. GetAllAsync()�� ȣ���� ��� ���̺� ��� �ݿ�
        }

        // �׸� ���� �� ������ �ε�
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
                Input = new DailyRawData(); // ���� �ۼ� ��
            }

            return Page();
        }
    }
}
