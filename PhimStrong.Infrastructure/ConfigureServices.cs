using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PhimStrong.Application.Interfaces;
using PhimStrong.Domain.Interfaces;
using PhimStrong.Domain.Models;
using PhimStrong.Infrastructure.Context;
using PhimStrong.Infrastructure.Email;
using PhimStrong.Infrastructure.Identity;

namespace PhimStrong.Infrastructure
{
	public static class ConfigureServices
	{
		public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
		{
			// config mailsettings
			services.Configure<MailSettings>(setting =>
			{
				configuration.GetSection("MailSettings").Bind(setting);
			});
			services.AddSingleton<IEmailSender, SendMailService>();

			// Add DbContext.
			services.AddDbContext<PhimStrongDbContext>(options =>
			{
				options.UseSqlServer(configuration.GetConnectionString("DefaultConnection") ?? "");
			});

			services.AddIdentity<User, IdentityRole>()
							.AddEntityFrameworkStores<PhimStrongDbContext>()
							.AddDefaultTokenProviders();

			services.Configure<IdentityOptions>(options =>
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
				options.User.AllowedUserNameCharacters = configuration["Characters"];
				options.User.RequireUniqueEmail = true;  // Email là duy nhất

				// Cấu hình đăng nhập.
				options.SignIn.RequireConfirmedEmail = false;
				options.SignIn.RequireConfirmedAccount = false;

			});

			services.AddScoped<IUnitOfWork, UnitOfWork.UnitOfWork>();
			services.AddScoped<IAuthenticationService, AuthenticationService>();
			services.AddScoped<IUserService, UserService>();
			services.AddScoped<IRoleService, RoleService>();

			return services;
		}
	}
}
