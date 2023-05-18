using AutoMapper;
using PhimStrong.Domain.Models;
using PhimStrong.Resources.Cast;
using PhimStrong.Resources.Category;
using PhimStrong.Resources.Country;
using PhimStrong.Resources.Director;
using PhimStrong.Resources.Movie;

namespace PhimStrong.Mapper
{
	public class DomainToResourceProfile : Profile
	{
		public DomainToResourceProfile()
		{
			CreateMap<Movie, MovieResource>();
			CreateMap<Movie, MovieDetailResource>()
				.ForMember(
					des => des.Tags,
					options => options.MapFrom(src => src.Tags != null ? src.Tags.Select(t => t.TagName) : null));

			CreateMap<Cast, CastResource>();

			CreateMap<Category, CategoryResource>();

			CreateMap<Country, CountryResource>();

			CreateMap<Director, DirectorResource>();

			CreateMap<Video, VideoResource>();
		}
	}
}
