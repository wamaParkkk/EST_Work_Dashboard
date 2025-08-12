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
        
        // ����/������
        public string? DownloadedPath { get; set; }
        public List<(string AssetNo, string Model)> Assets { get; set; } = new();
        
        // �ڻ� ����
        [BindProperty] public string? PickedAsset { get; set; }
        public List<LotSummary> LotSummaries { get; set; } = new();
        
        public async Task OnGetAsync()
        {
            (EquipTypes, LineCodes) = await _service.LoadDropdownsAsync();
        }

        // 1) CSV �ٿ�ε� & �ڻ� ��� �Ľ�
        public async Task<IActionResult> OnPostFetchAsync()
        {
            try
            {
                (EquipTypes, LineCodes) = await _service.LoadDropdownsAsync();
                
                if (string.IsNullOrWhiteSpace(SelectedEquip))
                {
                    ModelState.AddModelError("", "��� Ÿ���� �����ϼ���");
                    return Page();
                }

                DownloadedPath = await _service.DownloadCsvAsync(SelectedDate, SelectedEquip);
                Assets = await _service.EnumerateAssetsAsync(DownloadedPath);
                
                if (Assets.Count == 0)
                {
                    var headers = await _service.ProbeHeadersAsync(DownloadedPath);
                    ModelState.AddModelError("", "���: " + string.Join(" | ", headers));
                    ModelState.AddModelError("", "csv ������ �޾����� '�ڻ��ȣ'�� �� �ǵ� �Ľ̵��� �ʾҽ��ϴ�. (���/���ڵ�/������ Ȯ��)");                                        
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Fetch �� ����: {ex.Message}");
            }

            return Page();
        }

        // 2) �ڻ� ���� �� ��Ʈ ���
        public async Task<IActionResult> OnPostSummarizeAsync()
        {
            (EquipTypes, LineCodes) = await _service.LoadDropdownsAsync();
            
            var folder = _service.GetLocalFolder(SelectedDate);
            var file = $"{SelectedEquip}_{SelectedDate:yyyyMMdd}.csv";
            var csvPath = Path.Combine(folder, file);
            
            if (!System.IO.File.Exists(csvPath))
            {
                ModelState.AddModelError("", "���� CSV ������ �ٿ�ε� ���ּ���");
                return Page();
            }
            
            // �ڻ� ����� �ٽ� ǥ��
            Assets = await _service.EnumerateAssetsAsync(csvPath);
            
            if (string.IsNullOrWhiteSpace(PickedAsset))
            {
                ModelState.AddModelError(nameof(PickedAsset), "�ڻ��ȣ�� �����ϼ���");
                return Page();
            }
            
            try
            {
                LotSummaries = await _service.SummarizeByLotForAssetAsync(csvPath, PickedAsset);
                return Page();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"��� �� ����: {ex.Message}");
            }
            
            return Page();
        }
    }
}
