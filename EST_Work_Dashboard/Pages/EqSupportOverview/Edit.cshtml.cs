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
                Input = new EqSupportOverviewModel(); // 새로 작성 시
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            if (Input.Id > 0)
            {
                // Id가 존재하면 수정
                await _service.UpdateAsync(Input);
            }
            else
            {
                // Id가 없으면 새로 저장
                await _service.InsertAsync(Input);
            }

            return RedirectToPage("Index"); // 저장이 완료되면 Index로 이동. GetAllAsync()를 호출해 목록 테이블에 즉시 반영
        }
    }
}