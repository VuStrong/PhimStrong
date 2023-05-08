using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using PhimStrong.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using System.Security.Claims;
using PhimStrong.Application.Interfaces;
using PhimStrong.Application.Services;
using PhimStrong.Domain.Interfaces;
using PhimStrong.Infrastructure.UnitOfWork;
using PhimStrong.Domain.Models;
using PhimStrong.Mapper;
using PhimStrong.Infrastructure.Context;
using PhimStrong.Infrastructure.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews().AddNewtonsoftJson();
builder.Services.AddAutoMapper(typeof(DomainToViewModelProfile), typeof(ViewModelToDomainProfile));

// Add Send Email Service
builder.Services.AddOptions(); // Kích hoạt Options
var mailsettings = builder.Configuration.GetSection("MailSettings"); // đọc config
builder.Services.Configure<MailSettings>(mailsettings);
builder.Services.AddSingleton<IEmailSender, SendMailService>();

// Add DbContext.
builder.Services.AddDbContext<PhimStrongDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<PhimStrongDbContext>()
                .AddDefaultTokenProviders();

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
        // Thiết lập ClientID và ClientSecret để truy cập API google
        googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"];
        googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
        // Cấu hình Url callback lại từ Google (không thiết lập thì mặc định là /signin-google)
        googleOptions.CallbackPath = "/login-google";

        // Map the external picture claim to the internally used image claim
        googleOptions.ClaimActions.MapJsonKey("image", "picture");
    }); 

// Truy cập IdentityOptions
builder.Services.Configure<IdentityOptions>(options =>
{
    // Thiết lập về Password
    options.Password.RequireDigit = false; // Không bắt phải có số
    options.Password.RequireLowercase = false; // Không bắt phải có chữ thường
    options.Password.RequireNonAlphanumeric = false; // Không bắt ký tự đặc biệt
    options.Password.RequireUppercase = false; // Không bắt buộc chữ in
    options.Password.RequiredLength = 6; // Số ký tự tối thiểu của password
    options.Password.RequiredUniqueChars = 1; // Số ký tự riêng biệt

    //// Cấu hình Lockout - khóa user
    //options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); // Khóa 5 phút
    //options.Lockout.MaxFailedAccessAttempts = 5; // Thất bại 5 lần thì khóa
    //options.Lockout.AllowedForNewUsers = true;

    // Cấu hình về User.
    options.User.AllowedUserNameCharacters = builder.Configuration["Characters"];
    options.User.RequireUniqueEmail = true;  // Email là duy nhất
    
    // Cấu hình đăng nhập.
    options.SignIn.RequireConfirmedEmail = false;
    options.SignIn.RequireConfirmedAccount = false;

});

// add app services
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IMovieService, MovieService>();
builder.Services.AddScoped<ICastService, CastService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IDirectorService, DirectorService>();
builder.Services.AddScoped<ICountryService, CountryService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<PhimStrong.Application.Interfaces.IAuthenticationService, PhimStrong.Infrastructure.Identity.AuthenticationService>();
builder.Services.AddScoped<ICommentService, CommentService>();

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
