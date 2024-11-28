using Microsoft.AspNetCore.Http.Features;
using Xabe.FFmpeg;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
FFmpeg.SetExecutablesPath(Path.Combine(Directory.GetCurrentDirectory(), "ffmpeg"));
builder.Services.AddScoped<MSI.Models.DataManagementcs>();
builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 10L * 1024L * 1024L * 1024L; // 10 GB, adjust as necessary
});
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 10L * 1024L * 1024L * 1024L; // 10 GB, adjust as necessary
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Master}/{action=MasterDetails}/{id?}");

app.Run();
