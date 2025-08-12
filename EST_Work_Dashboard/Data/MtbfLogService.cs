using CsvHelper;
using CsvHelper.Configuration;
using EST_Work_Dashboard.Models;
using System.Globalization;
using System.Net;
using System.Text;
using System.Text.Json;

namespace EST_Work_Dashboard.Data
{
    public class MtbfLogService
    {
        private readonly IWebHostEnvironment _env;
        public MtbfLogService(IWebHostEnvironment env) => _env = env;
        
        private string AppDataPath => Path.Combine(_env.ContentRootPath, "App_Data");
        
        // ─────────────────────────────────────────────────────────────────────────
        // JSON 읽기
        // ─────────────────────────────────────────────────────────────────────────
        public async Task<(List<string> EquipTypes, List<string> LineCodes)> LoadDropdownsAsync()
        {
            var equipJson = await File.ReadAllTextAsync(Path.Combine(AppDataPath, "mtbf_equipment.json"));
            var lineJson = await File.ReadAllTextAsync(Path.Combine(AppDataPath, "mtbf_lines.json"));
            
            var equip = JsonSerializer.Deserialize<EquipmentListConfig>(equipJson) ?? new();
            var line = JsonSerializer.Deserialize<LineListConfig>(lineJson) ?? new();
            
            return (equip.equipmentTypes, line.lineCodes);
        }

        public async Task<FtpConfig> LoadFtpConfigAsync()
        {
            var ftpJson = await File.ReadAllTextAsync(Path.Combine(AppDataPath, "mtbf_ftp.json"));
            return JsonSerializer.Deserialize<FtpConfig>(ftpJson) ?? new FtpConfig();
        }
        
        // ─────────────────────────────────────────────────────────────────────────
        // 경로 & FTP 다운로드
        // ─────────────────────────────────────────────────────────────────────────
        public string GetLocalFolder(DateTime date) =>
            Path.Combine(@"C:\Amkor\MTBF Log", date.ToString("yyyy"), date.ToString("MM"), date.ToString("dd"));
        
        public async Task<string> DownloadCsvAsync(DateTime date, string equipType)
        {
            var ftp = await LoadFtpConfigAsync();
            
            var y = date.ToString("yyyy");
            var m = date.ToString("MM");
            var d = date.ToString("dd");
            var fileName = $"{equipType}_{y}{m}{d}.csv";
            
            // FTP 경로 (공백 등 안전하게 인코딩)
            var remoteUri = $"{ftp.host.TrimEnd('/')}/{Uri.EscapeDataString(ftp.rootFolder)}/{y}/{m}/{d}/{fileName}";
            
            // 로컬 폴더 준비
            var localFolder = GetLocalFolder(date);
            Directory.CreateDirectory(localFolder);
            var localPath = Path.Combine(localFolder, fileName);
            
            // 이미 있으면 재사용
            if (File.Exists(localPath))
                return localPath;
            
            // 다운로드
            var request = (FtpWebRequest)WebRequest.Create(new Uri(remoteUri));
            request.Method = WebRequestMethods.Ftp.DownloadFile;
            request.Credentials = new NetworkCredential(ftp.username, ftp.password);
            request.UseBinary = true;
            request.EnableSsl = false; // FTPS 사용 시 true, 서버 설정과 일치 필요
            
            using var response = (FtpWebResponse)await request.GetResponseAsync();
            await using var respStream = response.GetResponseStream();
            await using var outStream = File.Create(localPath);
            await respStream.CopyToAsync(outStream);
            
            return localPath;
        }

        // ─────────────────────────────────────────────────────────────────────────
        // CSV Helper
        // ─────────────────────────────────────────────────────────────────────────
        private StreamReader OpenReader(string path)
        {
            // 파일 공유(다른 프로세스가 쓰는 중일 수도)
            var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            
            // 1) BOM 체크
            if (fs.Length >= 3)
            {
                byte[] bom = new byte[3];
                fs.Read(bom, 0, 3);
                fs.Position = 0;
                
                // UTF-8 BOM
                if (bom[0] == 0xEF && bom[1] == 0xBB && bom[2] == 0xBF)
                    return new StreamReader(fs, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false), detectEncodingFromByteOrderMarks: true);
            }

            // 2) 샘플로 UTF-8 vs CP949 중 더 "한글이 많은" 쪽 선택
            int sampleLen = 4096;
            byte[] buf = new byte[Math.Min(sampleLen, Math.Max((int)fs.Length, 1))];
            int read = fs.Read(buf, 0, buf.Length);
            fs.Position = 0;
            
            string asUtf8 = Encoding.UTF8.GetString(buf, 0, read);
            string as949 = Encoding.GetEncoding(949).GetString(buf, 0, read);
            
            int scoreUtf8 = CountHangul(asUtf8);
            int score949 = CountHangul(as949);
            
            var enc = (score949 > scoreUtf8) ? Encoding.GetEncoding(949) : Encoding.UTF8;
            return new StreamReader(fs, enc, detectEncodingFromByteOrderMarks: false);
        }

        private static int CountHangul(string s)
        {
            int c = 0;
            foreach (var ch in s)
            {
                // 한글 유니코드 범위(가~힣)
                if (ch >= 0xAC00 && ch <= 0xD7A3) 
                    c++;
            }

            return c;
        }

        private CsvReader CreateCsvReader(StreamReader sr)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                DetectDelimiter = true,        // 구분자 false:고정 (속도 상승)
                //Delimiter = ",",
                TrimOptions = TrimOptions.Trim,
                BadDataFound = null,            // 깨진 데이터 무시
                MissingFieldFound = null,       // 누락 필드 무시
                HeaderValidated = null          // 헤더 검증 생략
            };

            return new CsvReader(sr, config);
        }

        private static string? GetVal(IDictionary<string, object> d, params string[] keys)
        {
            foreach (var k in keys)
                if (d.ContainsKey(k) && d[k] != null) return d[k]!.ToString();

            return null;
        }        

        // ─────────────────────────────────────────────────────────────────────────
        // Step1: 자산 목록만 스트리밍 수집 (AssetNo - Model)
        // ─────────────────────────────────────────────────────────────────────────
        public async Task<List<(string AssetNo, string Model)>> EnumerateAssetsAsync(string csvPath)
        {
            var set = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
            
            using var sr = OpenReader(csvPath);
            using var csv = CreateCsvReader(sr);
            
            await foreach (var rec in csv.GetRecordsAsync<dynamic>())
            {
                var d = (IDictionary<string, object>)rec;
                
                var asset = GetVal(d,
                    "자산번호", "자산 번호",
                    "AssetNo", "Asset No", "Asset"                    
                );

                if (string.IsNullOrWhiteSpace(asset)) 
                    continue;

                if (!set.ContainsKey(asset))
                {
                    var model = GetVal(d, "모델", "모델명", "Model", "Model Name", "Model_Name", "EquipModel");
                    set[asset] = model;
                }
            }

            return set.Select(kv => (kv.Key, kv.Value ?? ""))
                      .OrderBy(x => x.Key)
                      .ToList();
        }

        public async Task<string[]> ProbeHeadersAsync(string csvPath)
        {
            using var sr = OpenReader(csvPath);
            using var csv = CreateCsvReader(sr);
            
            if (csv.Read())
            {
                csv.ReadHeader();
                return csv.HeaderRecord?.Select(h => h?.Trim() ?? "").ToArray() ?? Array.Empty<string>();
            }
            
            return Array.Empty<string>();
        }

        // ─────────────────────────────────────────────────────────────────────────
        // Step2: 선택 자산만 다시 스트리밍 → 라트별 요약
        // ─────────────────────────────────────────────────────────────────────────
        public async Task<List<LotSummary>> SummarizeByLotForAssetAsync(string csvPath, string assetNo)
        {
            // ── 유틸: 정규화/파싱 ──────────────────────────────────────────────
            static string Normalize(string? s)
            {
                if (string.IsNullOrEmpty(s)) 
                    return string.Empty;

                return s.Replace("\uFEFF", "").Replace("\u00A0", " ").Trim()
                        .Normalize(System.Text.NormalizationForm.FormKC);
            }
            
            static string? GetVal(IDictionary<string, object> dict, params string[] keys)
            {
                var norm = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
                foreach (var kv in dict) 
                    norm[Normalize(kv.Key)] = kv.Value;

                foreach (var k in keys)
                {
                    var nk = Normalize(k);
                    if (norm.TryGetValue(nk, out var v) && v != null) 
                        return v.ToString();
                }

                return null;
            }
            
            static DateTime? TryParseDateTime(string? s)
            {
                s = Normalize(s);
                if (string.IsNullOrWhiteSpace(s)) return null;
                string[] fmts = {
                    "yyyy-MM-dd HH:mm:ss","yyyy-MM-dd HH:mm",
                    "yyyy/MM/dd HH:mm:ss","yyyy/MM/dd HH:mm",
                    "yyyy.MM.dd HH:mm:ss","yyyy.MM.dd HH:mm",
                    "yyyyMMddHHmmss","yyyy-MM-dd","yyyy/MM/dd","yyyy.MM.dd"
                };
                
                if (DateTime.TryParseExact(s, fmts, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt)) 
                    return dt;

                if (DateTime.TryParse(s, out dt)) 
                    return dt;

                return null;
            }
            // ────────────────────────────────────────────────────────────────
            
            // lot별 집계 버킷
            var lotRunSpans = new Dictionary<string, List<(DateTime s, DateTime e)>>();     // 가동
            var lotAlarmSpans = new Dictionary<string, List<(DateTime s, DateTime e)>>();   // 알람
            var lotQtyMax = new Dictionary<string, int>();                                  // 생산량: 최대값만 채택
                                                                                            
            // 중복 제거를 위한 "키" 저장 (ISO 8601로 문자열화해 정확 비교)
            var lotRunKeys = new Dictionary<string, HashSet<string>>();
            var lotAlarmKeys = new Dictionary<string, HashSet<string>>();
            
            using var sr = OpenReader(csvPath);     // UTF-8/CP949 자동 판별 구현
            using var csv = CreateCsvReader(sr);    // DetectDelimiter = true (구분자 확인)
            
            await foreach (var rec in csv.GetRecordsAsync<dynamic>())
            {
                var d = (IDictionary<string, object>)rec;
                
                // 자산번호 매칭
                var asset = GetVal(d,
                    "자산번호", "자산 번호",
                    "AssetNo", "Asset No", "Asset"                
                );

                if (!string.Equals(Normalize(asset), Normalize(assetNo), StringComparison.OrdinalIgnoreCase))
                    continue;
                
                // 라트, 시간, 생산량 추출
                var lot = GetVal(d, "라트번호(Dcc)", "라트번호", "LotNo", "Lot", "RunLot") ?? "(미기재)";
                
                var runS = TryParseDateTime(GetVal(d, "투입시간", "투입 시간", "RunStart", "Start"));
                var runE = TryParseDateTime(GetVal(d, "종료시간", "종료 시간", "RunEnd", "End"));
                
                var alS = TryParseDateTime(GetVal(d, "알람 시작 시간", "알람시작시간", "AlarmStart", "Alarm_Start"));
                var alE = TryParseDateTime(GetVal(d, "알람 종료 시간", "알람종료시간", "AlarmEnd", "Alarm_End"));
                
                var qtyStr = GetVal(d, "생산량", "OutputQty", "Qty", "Quantity");
                var hasQty = int.TryParse(Normalize(qtyStr), out var qty);
                
                // 초기화
                if (!lotRunSpans.ContainsKey(lot)) lotRunSpans[lot] = new();
                if (!lotAlarmSpans.ContainsKey(lot)) lotAlarmSpans[lot] = new();
                if (!lotRunKeys.ContainsKey(lot)) lotRunKeys[lot] = new();
                if (!lotAlarmKeys.ContainsKey(lot)) lotAlarmKeys[lot] = new();
                if (!lotQtyMax.ContainsKey(lot)) lotQtyMax[lot] = 0;
                
                // 가동 구간: 동일 (s,e) 중복 제거
                if (runS.HasValue && runE.HasValue && runE > runS)
                {
                    var key = $"{runS.Value:O}|{runE.Value:O}";
                    if (!lotRunKeys[lot].Contains(key))
                    {
                        lotRunSpans[lot].Add((runS.Value, runE.Value));
                        lotRunKeys[lot].Add(key);
                    }
                }
                
                // 알람 구간: 동일 (s,e) 중복 제거
                if (alS.HasValue && alE.HasValue && alE > alS)
                {
                    var key = $"{alS.Value:O}|{alE.Value:O}";
                    if (!lotAlarmKeys[lot].Contains(key))
                    {
                        lotAlarmSpans[lot].Add((alS.Value, alE.Value));
                        lotAlarmKeys[lot].Add(key);
                    }
                }
                
                // 생산량: 라트별 최대값만 채택 (중복 합산 방지)
                if (hasQty && qty > lotQtyMax[lot])
                    lotQtyMax[lot] = qty;
                
                /* 만약 "구간별 개별 생산량"이라서 합산이 맞다면 위 대신 아래처럼:
                   var runKeyForQty = (runS.HasValue && runE.HasValue) ? $"{runS.Value:O}|{runE.Value:O}" : null;
                   if (runKeyForQty != null)
                       perLotRunQty[lot][runKeyForQty] = Math.Max(perLotRunQty[lot].GetValueOrDefault(runKeyForQty, 0), qty);
                   마지막에 lot별 perLotRunQty[lot].Values.Sum() 사용
                */
            }
            
            // 요약 변환
            var summaries = new List<LotSummary>();
            foreach (var lot in lotRunSpans.Keys.Union(lotAlarmSpans.Keys).Union(lotQtyMax.Keys).Distinct())
            {
                var runs = lotRunSpans.TryGetValue(lot, out var r1) ? r1 : new();
                var alarms = lotAlarmSpans.TryGetValue(lot, out var r2) ? r2 : new();
                
                var start = runs.Any() ? runs.Min(x => x.s) : (DateTime?)null;
                var end = runs.Any() ? runs.Max(x => x.e) : (DateTime?)null;
                
                var runSeconds = runs.Sum(x => (x.e - x.s).TotalSeconds);
                var alarmSeconds = alarms.Sum(x => (x.e - x.s).TotalSeconds);
                
                summaries.Add(new LotSummary
                {
                    LotNo = lot,
                    Start = start,
                    End = end,
                    RunDuration = TimeSpan.FromSeconds(runSeconds),
                    AlarmDuration = TimeSpan.FromSeconds(alarmSeconds),
                    TotalOutput = lotQtyMax.TryGetValue(lot, out var total) ? total : 0,
                    AlarmCount = alarms.Count
                });
            }

            return summaries
                .OrderBy(s => s.Start ?? DateTime.MaxValue)
                .ThenBy(s => s.LotNo)
                .ToList();
        }
    }
}
