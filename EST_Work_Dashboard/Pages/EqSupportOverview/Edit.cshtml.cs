using EST_Work_Dashboard.Data;
using EST_Work_Dashboard.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EST_Work_Dashboard.Pages.EqSupportOverview
{
    public class EditModel : PageModel
    {
        private readonly EqSupportOverviewService _service;

        public EditModel(EqSupportOverviewService service)
        {
            _service = service;
        }

        [BindProperty]
        public EqSupportOverviewModel Input { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id.HasValue)
            {
                Input = await _service.GetByIdAsync(id.Value);
                if (Input == null)
                {
                    return NotFound();
                }
            }
            else
            {
                Input = new EqSupportOverviewModel(); // ���� �ۼ� ��
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            if (Input.Id > 0)
            {
                // Id�� �����ϸ� ����
                await _service.UpdateAsync(Input);
            }
            else
            {
                // Id�� ������ ���� ����
                await _service.InsertAsync(Input);
            }

            return RedirectToPage("Index"); // ������ �Ϸ�Ǹ� Index�� �̵�. GetAllAsync()�� ȣ���� ��� ���̺� ��� �ݿ�
        }
    }
}