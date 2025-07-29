using EST_Work_Dashboard.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<DailyRawDataService>();

// Add services to the container.
builder.Services.AddRazorPages();

// 고정 포트 5010으로 Listen (외부 접속 가능)
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5020);
});

var app = builder.Build();

// 서버 실행(exe)시 자동 브라우저 열기 (서버 PC에서만)

System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
{
    FileName = "http://localhost:5020",
    UseShellExecute = true
});


app.UseStaticFiles();

app.UseRouting();

app.MapRazorPages();

app.Run();
