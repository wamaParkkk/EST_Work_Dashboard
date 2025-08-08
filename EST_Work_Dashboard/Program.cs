using EST_Work_Dashboard.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddSingleton<DailyRawDataService>();
builder.Services.AddSingleton<EqSupportOverviewService>();
builder.Services.AddSingleton<LayOutService>();
builder.Services.AddSingleton<TrainingService>();

// ���� ��Ʈ 5020���� Listen (�ܺ� ���� ����)
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5020);
});

var app = builder.Build();

// ���� ����(exe)�� �ڵ� ������ ���� (���� PC������)
/*
System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
{
    FileName = "http://localhost:5020",
    UseShellExecute = true
});
*/

app.UseStaticFiles();

app.UseRouting();

app.MapRazorPages();

app.Run();
