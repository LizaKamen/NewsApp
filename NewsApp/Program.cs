using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using NewsApp.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using NewsApp.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews()
    .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix);

builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

var supportedCultures = new[] { "en", "ru", "ja-jp" };
var supportedUICultures = new[] { "en", "ru", "ja-jp" };

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.SetDefaultCulture("en");
    options.AddSupportedCultures(supportedCultures);
    options.AddSupportedUICultures(supportedUICultures);
});

builder.Services.AddDbContext<NewsAppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDefaultIdentity<User>(options =>
    {
        options.Password.RequireDigit = false;
        options.Password.RequireLowercase = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
        options.Password.RequiredUniqueChars = 0;
        options.SignIn.RequireConfirmedAccount = false;
    })
    .AddRoles<IdentityRole>().AddEntityFrameworkStores<NewsAppDbContext>();
builder.Services.AddRazorPages();
var app = builder.Build();

app.UseRequestLocalization();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();
app.MapRazorPages();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    try
    {
        var context = services.GetRequiredService<NewsAppDbContext>();
        var userManager = services.GetRequiredService<UserManager<User>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        await context.Database.MigrateAsync();

        var roles = new[] { "Admin", "User" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
                Console.WriteLine("Created role {0}", role);
            }
        }

        var user = await userManager.FindByEmailAsync("admin@news.app");
        if (user == null)
        {
            user = new User()
            {
                Email = "admin@news.app",
                UserName = "admin@news.app"
            };

            var pass = "adminPass";
            var passValidator = new PasswordValidator<User>();
            var isValid = await passValidator.ValidateAsync(userManager, user, pass);
            if (isValid.Succeeded)
            {
                var res = await userManager.CreateAsync(user, pass);

                if (res.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Admin");
                    await userManager.AddClaimsAsync(user, [
                        new Claim(ClaimTypes.Name, user.UserName),
                        new Claim(ClaimTypes.Role, "Admin")
                    ]);
                    Console.WriteLine("Created user {0}", user.UserName);
                }
                else
                {
                    Console.WriteLine("Error creating admin user:  " + res.Errors);
                }
            }

            else
            {
                Console.WriteLine(isValid.Errors);
            }
        }
        else
        {
            Console.WriteLine("User already exists");
        }
    }
    catch (Exception e)
    {
        Console.WriteLine(e);
    }
}

app.Run();