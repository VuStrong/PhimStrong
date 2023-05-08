using AutoMapper;
using PhimStrong.Areas.Admin.Models.Cast;
using PhimStrong.Areas.Admin.Models.Category;
using PhimStrong.Areas.Admin.Models.Country;
using PhimStrong.Areas.Admin.Models.Director;
using PhimStrong.Domain.Models;
using PhimStrong.Domain.PagingModel;
using PhimStrong.Models.Cast;
using PhimStrong.Models.Category;
using PhimStrong.Models.Comment;
using PhimStrong.Models.Country;
using PhimStrong.Models.Director;
using PhimStrong.Models.Movie;
using PhimStrong.Models.Tag;
using PhimStrong.Models.User;
using PhimStrong.Models.Video;

namespace PhimStrong.Mapper
{
    public class DomainToViewModelProfile : Profile
    {
        public DomainToViewModelProfile()
        {
            CreateMap<Movie, MovieViewModel>();

            CreateMap<Cast, EditCastViewModel>();
			CreateMap<Cast, CastViewModel>();
            
            CreateMap<Director, EditDirectorViewModel>();
            CreateMap<Director, DirectorViewModel>();

            CreateMap<Category, EditCategoryViewModel>();
            CreateMap<Category, CategoryViewModel>();

            CreateMap<Country, EditCountryViewModel>();
            CreateMap<Country, CountryViewModel>();

            CreateMap<Comment, CommentViewModel>();
            CreateMap<User, UserViewModel>();

            CreateMap<Video, VideoViewModel>()
                .ForMember(des => des.MovieId, options => options.MapFrom(src => src.Movie.Id));
            
            CreateMap<Tag, TagViewModel>()
				.ForMember(des => des.MovieId, options => options.MapFrom(src => src.Movie.Id));

            CreateMap(typeof(PagedList<>), typeof(PagedList<>)).ConvertUsing(typeof(Converter<,>));
        }
    }

    public class Converter<TSource, TDestination> : ITypeConverter<PagedList<TSource>, PagedList<TDestination>>
    {
        public PagedList<TDestination> Convert(
            PagedList<TSource> source,
            PagedList<TDestination> destination,
            ResolutionContext context) =>
            new PagedList<TDestination>(
                context.Mapper.Map<List<TSource>, List<TDestination>>(source), source.CurrentPage, source.PageSize, source.TotalItems);
    }
}
