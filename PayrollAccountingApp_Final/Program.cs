using Microsoft.EntityFrameworkCore;
using PayrollAccountingApp.Data;
using PayrollAccountingApp.Services;

var builder = WebApplication.CreateBuilder(args);

// Config
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddHttpClient<AccountingApiClient>();

builder.Services.Configure<AccountingApiOptions>(
    builder.Configuration.GetSection("AccountingApi"));

var app = builder.Build();

// Migrate at startup (optional / dev only)
//using (var scope = app.Services.CreateScope())
//{
//    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
//    db.Database.Migrate();
//}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Transactions}/{action=Index}/{id?}");

app.Run();
