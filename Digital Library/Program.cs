using Digital_Library.Data;
using Digital_Library.Domain.Entities;
using Digital_Library.Domain.Services;
using Digital_Library.Infrastructure;
using Digital_Library.Middleware;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .WriteTo.File("Logger/log.txt")
    .CreateLogger();

DbContextOptionsBuilder optionsBuilder = new DbContextOptionsBuilder();
optionsBuilder.UseSqlServer(builder.Configuration.GetConnectionString("express"));
//try
//{
//    optionsBuilder.UseSqlServer(builder.Configuration.GetConnectionString("express"));
//    using (var context = new DigitalLibraryContext(optionsBuilder.Options))
//    {
//        EFInitialSeed.Seed(context);
//    }
//}
//catch (Exception e)
//{
//    Log.Error(e.Message);
//}
builder.Services.AddControllersWithViews();
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(opt =>
    {
        opt.ExpireTimeSpan = TimeSpan.FromHours(1);
        opt.Cookie.Name = "library_session";
        opt.Cookie.HttpOnly = true;
        opt.Cookie.SameSite = SameSiteMode.Strict;
        opt.LoginPath = "/User/Login";
        opt.AccessDeniedPath = "/User/AccessDenied";
    });
builder.Services.AddDbContext<DigitalLibraryContext>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("express")));
builder.Services.AddScoped<IRepository<User>, EFRepository<User>>();
builder.Services.AddScoped<IRepository<Role>, EFRepository<Role>>();
builder.Services.AddScoped<IRepository<Book>, EFRepository<Book>>();
builder.Services.AddScoped<IRepository<Category>, EFRepository<Category>>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IBooksReader, BooksReader>();
builder.Services.AddScoped<IBooksService, BooksService>();

builder.Logging.ClearProviders();

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.UseBooksProtection();
app.UseStaticFiles();

app.MapControllerRoute("default", "{Controller=Books}/{Action=Index}");
app.Run();
