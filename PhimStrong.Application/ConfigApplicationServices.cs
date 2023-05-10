using Microsoft.Extensions.DependencyInjection;
using PhimStrong.Application.Interfaces;
using PhimStrong.Application.Services;

namespace PhimStrong.Application
{
	public static class ConfigApplicationServices
	{
		public static IServiceCollection AddApplicationServices(this IServiceCollection services)
		{
			// add app services
			services.AddScoped<IMovieService, MovieService>();
			services.AddScoped<ICastService, CastService>();
			services.AddScoped<ICategoryService, CategoryService>();
			services.AddScoped<IDirectorService, DirectorService>();
			services.AddScoped<ICountryService, CountryService>();
			services.AddScoped<ICommentService, CommentService>();

			return services;
		}
	}
}
