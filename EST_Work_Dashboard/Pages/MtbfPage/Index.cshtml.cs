using EST_Work_Dashboard.Data;
using EST_Work_Dashboard.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EST_Work_Dashboard.Pages.MtbfPage
{
    public class IndexModel : PageModel
    {
        private readonly MtbfLogService _service;
        public IndexModel(MtbfLogService service) => _service = service;
        
        // Dropdown
        [BindProperty(SupportsGet = true)] public DateTime SelectedDate { get; set; } = DateTime.Today;
        [BindProperty(SupportsGet = true)] public string SelectedEquip { get; set; } = "";
        [BindProperty(SupportsGet = true)] public string SelectedLine { get; set; } = "";
        
        public List<string> EquipTypes { get; set; } = new();
        public List<string> LineCodes { get; set; } = new();
        
        // 파일/데이터
        public string? DownloadedPath { get; set; }
        public List<(string AssetNo, string Model)> Assets { get; set; } = new();
        
        // 자산 선택
        [BindProperty] public string? PickedAsset { get; set; }
        public List<LotSummary> LotSummaries { get; set; } = new();
        
        public async Task OnGetAsync()
        {
            (EquipTypes, LineCodes) = await _service.LoadDropdownsAsync();
        }

        // 1) CSV 다운로드 & 자산 목록 파싱
        public async Task<IActionResult> OnPostFetchAsync()
        {
            try
            {
                (EquipTypes, LineCodes) = await _service.LoadDropdownsAsync();
                
                if (string.IsNullOrWhiteSpace(SelectedEquip))
                {
                    ModelState.AddModelError("", "장비 타입을 선택하세요");
                    return Page();
                }

                DownloadedPath = await _service.DownloadCsvAsync(SelectedDate, SelectedEquip);
                Assets = await _service.EnumerateAssetsAsync(DownloadedPath);
                
                if (Assets.Count == 0)
                {
                    var headers = await _service.ProbeHeadersAsync(DownloadedPath);
                    ModelState.AddModelError("", "헤더: " + string.Join(" | ", headers));
                    ModelState.AddModelError("", "csv 파일은 받았지만 '자산번호'가 한 건도 파싱되지 않았습니다. (헤더/인코딩/구분자 확인)");                                        
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Fetch 중 오류: {ex.Message}");
            }

            return Page();
        }

        // 2) 자산 선택 후 라트 요약
        public async Task<IActionResult> OnPostSummarizeAsync()
        {
            (EquipTypes, LineCodes) = await _service.LoadDropdownsAsync();
            
            var folder = _service.GetLocalFolder(SelectedDate);
            var file = $"{SelectedEquip}_{SelectedDate:yyyyMMdd}.csv";
            var csvPath = Path.Combine(folder, file);
            
            if (!System.IO.File.Exists(csvPath))
            {
                ModelState.AddModelError("", "먼저 CSV 파일을 다운로드 해주세요");
                return Page();
            }
            
            // 자산 목록은 다시 표시
            Assets = await _service.EnumerateAssetsAsync(csvPath);
            
            if (string.IsNullOrWhiteSpace(PickedAsset))
            {
                ModelState.AddModelError(nameof(PickedAsset), "자산번호를 선택하세요");
                return Page();
            }
            
            try
            {
                LotSummaries = await _service.SummarizeByLotForAssetAsync(csvPath, PickedAsset);
                return Page();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"요약 중 오류: {ex.Message}");
            }
            
            return Page();
        }
    }
}
