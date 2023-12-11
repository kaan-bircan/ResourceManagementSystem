using Business;
using Business.Services;
using DataAccess.Contexts;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Localization;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MVC.Settings;
using System;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

#region Localization

List<CultureInfo> cultures = new List<CultureInfo>()
{
    new CultureInfo("en-US") 
};

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.DefaultRequestCulture = new RequestCulture(cultures.FirstOrDefault().Name);
    options.SupportedCultures = cultures;
    options.SupportedUICultures = cultures;
});
#endregion

#region AppSettings
//var section = builder.Configuration.GetSection("AppSettings");
var section = builder.Configuration.GetSection(nameof(MVC.Settings.AppSettings));
section.Bind(new MVC.Settings.AppSettings());
#endregion


// Add services to the container.
#region IoC (Inversion of Control) Container
// IoC Container manages the initialization operations of the objects which are
// injected to classes by Constructor Injection. Alternatively Autofac or Ninject
// libraries can also be used under the Business layer.
// "Unable to reslove service..." exceptions should be resolved here.
builder.Services.AddDbContext<Db>(options => options // options used in the AddDbContext method is a delegate
                                                     // of type DbContextOptionsBuilder. This delegate
                                                     // is also called an Action which doesn't return anything.
                                                     // Actions are generally used for configurations.
                                                     // Through the Actions properties or methods
                                                     // (such as UseMySql method) can be used therefore
                                                     // the Actions can provide these to the method
                                                     // which they are used in.
                                                     // We are saying that use MySQL with the provided
                                                     // connection string through the options Action
                                                     // to the AddDbContext method which uses the type of Db,
                                                     // therefore we should provide the type of our class
                                                     // inherited from the DbContext as the generic type
                                                     // for AddDbContext method.
    .UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// AddScoped: The object's reference (usually an interface or abstract class) is used to instantiate an object
// through constructor injection when a request is received and the object lives until the response is returned.
// AddDbContext is a scoped method by default.
// AddSingleton: The object's reference (usually an interface or abstract class) is used to instantiate an object
// through constructor injection when a request is received and the only one object lives throughout
// the application's lifetime (as long as the application is running and not stopped or restarted for example
// via IIS: Internet Information Services or Kestrel web server applications).
// AddTransient: The object is instantiated every time whenever a constructor injection through the
// object's reference (usually an interface or abstract class) is used, independent from the request.
// Generally the AddScoped method is used.
// Way 1:
// builder.Services.AddSingleton<IUserService, UserService>();
// Way 2:
// builder.Services.AddTransient<IUserService, UserService>();
// Way 3:
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddScoped<IRoleService, RoleService>();

builder.Services.AddScoped<IResourceService, ResourceService>();
#endregion

builder.Services.AddControllersWithViews();

#region
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(config =>
     {
         config.LoginPath = "/Account/Login";//giriþ yapýlmadan iþlem yapýlýrsa yönlendir
         config.AccessDeniedPath = "/Account/AccessDenied";//giriþ yaptýktan sonra yetki dýþý iþlem
         config.ExpireTimeSpan = TimeSpan.FromMinutes(AppSettings.CookieExpirationInMinutes);//cookie 30 dakika kullanýlabilir,iþlem yapýlýrsa 30 yenilenir
         config.SlidingExpiration = true;
     });
#endregion

var app = builder.Build();

#region Localization
app.UseRequestLocalization(new RequestLocalizationOptions()
{
    DefaultRequestCulture = new RequestCulture(cultures.FirstOrDefault().Name),
    SupportedCultures = cultures,
    SupportedUICultures = cultures
});
#endregion

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
#region Authentication
app.UseAuthentication();
#endregion
app.UseAuthorization();

// default Route
app.MapControllerRoute(
      name: "register",
      pattern: "register",
      defaults: new { controller = "Users", action = "Create" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
