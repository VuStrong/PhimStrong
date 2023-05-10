using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using System.Security.Claims;
using PhimStrong.Mapper;
using PhimStrong.Infrastructure;
using PhimStrong.Application;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews().AddNewtonsoftJson();
builder.Services.AddAutoMapper(typeof(DomainToViewModelProfile), typeof(ViewModelToDomainProfile));

// Add Send Email Service
builder.Services.AddOptions(); // Kích hoạt Options

builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApplicationServices();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/login";
    options.LogoutPath = "/Identity/Authentication/Logout";
    options.AccessDeniedPath = "/Identity/Authentication/AccessDenied";
});

// Authentication services :
builder.Services.AddAuthentication()
    .AddFacebook(facebookOptions => {
        facebookOptions.AppId = builder.Configuration["Authentication:Facebook:AppId"];
        facebookOptions.AppSecret = builder.Configuration["Authentication:Facebook:AppSecret"];
        facebookOptions.CallbackPath = "/dang-nhap-facebook";

        facebookOptions.Fields.Add("picture");
        facebookOptions.Events = new OAuthEvents
        {
            OnCreatingTicket = (context) =>
            {
                ClaimsIdentity? identity = context.Principal != null ? (ClaimsIdentity?)context.Principal.Identity : null;
                string profileImg = context.User.GetProperty("picture").GetProperty("data").GetProperty("url").ToString();
                if (identity != null) identity.AddClaim(new Claim("image", profileImg));
                return Task.CompletedTask;
            }
        };
    })
    .AddGoogle(googleOptions =>
    {
        googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"];
        googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];

        // Cấu hình Url callback lại từ Google (không thiết lập thì mặc định là /signin-google)
        googleOptions.CallbackPath = "/login-google";

        googleOptions.ClaimActions.MapJsonKey("image", "picture");
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

app.UseAuthentication();
app.UseAuthorization();

// Area Route
app.MapControllerRoute(
    name: "MyArea",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
