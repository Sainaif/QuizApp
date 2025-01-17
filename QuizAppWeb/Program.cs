using Microsoft.EntityFrameworkCore;
using QuizAppDB.Data;

var builder = WebApplication.CreateBuilder(args);

// Rejestracja DbContext
builder.Services.AddDbContext<QuizDbContext>(options =>
    options.UseSqlite("Data Source=QuizAppDB.sqlite"));

// Rejestracja repozytorium
builder.Services.AddScoped<QuizRepository>();

// Rejestracja kontrolerów i widoków
builder.Services.AddControllersWithViews();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Quiz}/{action=Index}/{id?}");

app.Run();
