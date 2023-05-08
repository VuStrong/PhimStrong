using AutoMapper;
using PhimStrong.Models.Comment;
using PhimStrong.Models.Movie;
using PhimStrong.Models.Tag;
using PhimStrong.Models.User;
using PhimStrong.Models.Video;
using PhimStrong.Domain.Models;
using SharedLibrary.Helpers;
using PhimStrong.Areas.Admin.Models.Movie;
using PhimStrong.Areas.Admin.Models.Cast;
using PhimStrong.Areas.Admin.Models.Category;
using PhimStrong.Areas.Admin.Models.Director;
using PhimStrong.Areas.Admin.Models.Country;
using PhimStrong.Areas.Identity.Models;

namespace PhimStrong.Mapper
{
    public class ViewModelToDomainProfile : Profile
    {
        public ViewModelToDomainProfile()
        {
			CreateMap<MovieViewModel, Movie>();
			CreateMap<CreateMovieViewModel, Movie>()
				.ForMember(des => des.Casts, options => options.Ignore())
				.ForMember(des => des.Categories, options => options.Ignore())
				.ForMember(des => des.Directors, options => options.Ignore())
                .ForMember(des => des.Country, options => options.Ignore())
                .ForMember(des => des.Tags, options => options.Ignore())
                .ForMember(des => des.Videos, options => options.Ignore());
			CreateMap<EditMovieViewModel, Movie>()
                .ForMember(des => des.Casts, options => options.Ignore())
                .ForMember(des => des.Categories, options => options.Ignore())
                .ForMember(des => des.Directors, options => options.Ignore())
                .ForMember(des => des.Country, options => options.Ignore())
                .ForMember(des => des.Tags, options => options.Ignore())
                .ForMember(des => des.Videos, options => options.Ignore());

            CreateMap<CreateCastViewModel, Cast>();
			CreateMap<EditCastViewModel, Cast>();

			CreateMap<EditDirectorViewModel, Director>();
			CreateMap<CreateDirectorViewModel, Director>();

            CreateMap<EditCategoryViewModel, Category>();
			CreateMap<CreateCategoryViewModel, Category>();

            CreateMap<EditCountryViewModel, Country>();
            CreateMap<CreateCountryViewModel, Country>();

            CreateMap<CommentViewModel, Comment>();

			CreateMap<EditUserViewModel, User>();
            CreateMap<UserViewModel, User>();

			CreateMap<VideoViewModel, Video>()
				.ForMember(des => des.Movie, options => options.Ignore());

			CreateMap<TagViewModel, Tag>()
				.ForMember(des => des.Movie, options => options.Ignore());
		}
    }

}
