using AutoMapper;
using PhimStrong.Domain.Parameters;
using PhimStrong.Resources.Movie;

namespace PhimStrong.Mapper
{
	public class ResourceToDomainProfile : Profile
	{
		public ResourceToDomainProfile()
		{
			CreateMap<MovieParameterResource, MovieParameter>();
		}
	}
}
