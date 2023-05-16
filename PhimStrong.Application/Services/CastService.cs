using PhimStrong.Application.Interfaces;
using PhimStrong.Domain.Exceptions.NotFound;
using PhimStrong.Domain.Interfaces;
using PhimStrong.Domain.Models;
using PhimStrong.Domain.PagingModel;
using PhimStrong.Domain.Parameters;
using SharedLibrary.Helpers;

namespace PhimStrong.Application.Services
{
    public class CastService : ICastService
	{
		private readonly IUnitOfWork _unitOfWork;

        public CastService(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
        }

        public async Task<Cast> CreateAsync(Cast cast)
        {
			cast.IdNumber = await _unitOfWork.CastRepository.AnyAsync() ?
				await _unitOfWork.CastRepository.MaxIdNumberAsync() + 1 : 1;
			
			cast.Id = "cast" + cast.IdNumber.ToString();

			// chỉnh lại format tên :
			cast.Name = cast.Name.NormalizeString();
			cast.NormalizeName = cast.Name.RemoveMarks();

			_unitOfWork.CastRepository.Create(cast);
			await _unitOfWork.SaveAsync();
			return cast;
		}

        public async Task DeleteAsync(string castid)
        {
			Cast? cast = await _unitOfWork.CastRepository.FirstOrDefaultAsync(c => c.Id == castid);

			if (cast == null) throw new CastNotFoundException(castid);

			_unitOfWork.CastRepository.Delete(cast);
			await _unitOfWork.SaveAsync();
        }

		public async Task<PagedList<Cast>> GetAllAsync(PagingParameter pagingParameter)
		{
			return await _unitOfWork.CastRepository.GetAsync(pagingParameter: pagingParameter);
		}

        public async Task<IEnumerable<Cast>> GetAllAsync()
        {
            return await _unitOfWork.CastRepository.GetAsync();
        }

        public async Task<Cast?> GetByIdAsync(string id)
		{
			return await _unitOfWork.CastRepository.FirstOrDefaultAsync(x => x.Id == id);
		}

		public async Task<Cast?> GetByNameAsync(string name)
        {
			name = name.RemoveMarks();

			return await _unitOfWork.CastRepository.FirstOrDefaultAsync(x => x.NormalizeName == name);
        }

		public async Task<PagedList<Cast>> SearchAsync(string? value, PagingParameter pagingParameter)
		{
			value = value?.RemoveMarks();

			PagedList<Cast> casts = value != null ?
                await _unitOfWork.CastRepository.GetAsync(
					pagingParameter: pagingParameter,
					c => (c.NormalizeName ?? "").Contains(value)) :
				await this.GetAllAsync(pagingParameter);

			return casts;
		}

		public async Task<Cast> UpdateAsync(string castid, Cast cast)
        {
            Cast? castToEdit = await _unitOfWork.CastRepository.FirstOrDefaultAsync(c => c.Id == castid);

            if (castToEdit == null) throw new CastNotFoundException(castid);

			castToEdit.Name = cast.Name.NormalizeString();
            castToEdit.NormalizeName = castToEdit.Name.RemoveMarks();
            castToEdit.About = cast.About;
            castToEdit.DateOfBirth = cast.DateOfBirth;
			castToEdit.Avatar = cast.Avatar;

            _unitOfWork.CastRepository.Update(castToEdit);
			await _unitOfWork.SaveAsync();
			
			return castToEdit;
		}
    }
}
