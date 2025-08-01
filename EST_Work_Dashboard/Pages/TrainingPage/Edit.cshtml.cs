using EST_Work_Dashboard.Data;
using EST_Work_Dashboard.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EST_Work_Dashboard.Pages.TrainingPage
{
    public class EditModel : PageModel
    {
        private readonly TrainingService _service;

        public EditModel(TrainingService service)
        {
            _service = service;
        }

        [BindProperty]
        public TrainingModel Input { get; set; } // ����ڰ� �Է��� �� �����               

        // �׸� ���� �� ������ �ε�
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
                Input = new TrainingModel(); // ���� �ۼ� ��
            }

            return Page();
        }

        // �� �׸� �ۼ� �� ����
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
