using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Munkanaplo2.Data;
//using RPauth.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.Configure<IdentityOptions>(options =>
{
    // Default User settings.
    options.User.AllowedUserNameCharacters =
            "aábcdeéfghiíjklmnoóöőpqrstuúüűvwxyzAÁBCDEÉFGHIÍJKLMNOÓÖŐPQRSTUÚÜŰVWXYZ0123456789-._@+ ";
    options.User.RequireUniqueEmail = false;

});

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
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
    name: "feladatok/új",
    pattern: "feladatok/új",
    defaults: new { controller = "Jobs", action = "Create" }
    );
app.MapControllerRoute(
    name: "feladatok",
    pattern: "feladatok",
    defaults: new { controller = "Jobs", action = "Index"}
    );
app.MapControllerRoute(
    name: "feladatok/tanári-nézet",
    pattern: "feladatok/tanári-nézet",
    defaults: new { controller = "Jobs", action = "TeacherView"}
    );
app.MapControllerRoute(
    name: "feladatok/részletek",
    pattern: "feladatok/{id}",
    defaults: new { controller = "Jobs", action = "Details", id = "" }
    );
app.MapControllerRoute(
    name: "feladatok/szerkesztés",
    pattern: "feladatok/szekesztés/{id}",
    defaults: new { controller = "Jobs", action = "Edit", id = "" }
    );
app.MapControllerRoute(
    name: "feladatok/feladat-törlése",
    pattern: "feladatok/feladat-törlése/{id}",
    defaults: new { controller = "Jobs", action = "Delete", id = "" }
    );

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();

