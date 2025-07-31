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
        public DailyRawData Input { get; set; } // ����ڰ� �Է��� �� �����               

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

        // �� �׸� �ۼ� �� ����
        public async Task<IActionResult> OnPostAsync()
        {         
            if (!ModelState.IsValid)
                return Page();

            // tkww �ڵ� ���
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
    }
}
