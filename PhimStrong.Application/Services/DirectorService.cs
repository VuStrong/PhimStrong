using PhimStrong.Application.Interfaces;
using PhimStrong.Domain.Exceptions.NotFound;
using PhimStrong.Domain.Interfaces;
using PhimStrong.Domain.Models;
using PhimStrong.Domain.PagingModel;
using SharedLibrary.Helpers;

namespace PhimStrong.Application.Services
{
    public class DirectorService : IDirectorService
	{
		private readonly IUnitOfWork _unitOfWork;

        public DirectorService(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
        }

        public async Task<Director> CreateAsync(Director director)
        {
            director.IdNumber = await _unitOfWork.DirectorRepository.AnyAsync() ?
                await _unitOfWork.DirectorRepository.MaxIdNumberAsync() + 1 : 1;

            director.Id = "director" + director.IdNumber.ToString();

            // chỉnh lại format tên :
            director.Name = director.Name.NormalizeString();
            director.NormalizeName = director.Name.RemoveMarks();

            _unitOfWork.DirectorRepository.Create(director);
            await _unitOfWork.SaveAsync();
            return director;
        }

        public async Task DeleteAsync(string directorid)
        {
            Director? director = await _unitOfWork.DirectorRepository.FirstOrDefaultAsync(d => d.Id == directorid);

            if (director == null) throw new DirectorNotFoundException(directorid);

            _unitOfWork.DirectorRepository.Delete(director);
            await _unitOfWork.SaveAsync();
        }

        public async Task<PagedList<Director>> GetAllAsync(PagingParameter pagingParameter)
        {
            return await _unitOfWork.DirectorRepository.GetAsync(pagingParameter: pagingParameter);
        }

        public async Task<IEnumerable<Director>> GetAllAsync()
        {
            return await _unitOfWork.DirectorRepository.GetAsync();
        }

        public async Task<Director?> GetByIdAsync(string id)
        {
            return await _unitOfWork.DirectorRepository.FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<Director?> GetByNameAsync(string name)
        {
            name = name.RemoveMarks();

            return await _unitOfWork.DirectorRepository.FirstOrDefaultAsync(x => x.NormalizeName == name);
        }

        public async Task<PagedList<Director>> SearchAsync(string? value, PagingParameter pagingParameter)
        {
            value = value?.RemoveMarks();

            PagedList<Director> directors = value != null ?
                await _unitOfWork.DirectorRepository.GetAsync(
                    pagingParameter: pagingParameter,
                    d => (d.NormalizeName ?? "").Contains(value)) :
                await this.GetAllAsync(pagingParameter);

            return directors;
        }

        public async Task<Director> UpdateAsync(string directorid, Director director)
        {
            Director? directorToEdit = await _unitOfWork.DirectorRepository.FirstOrDefaultAsync(d => d.Id == directorid);

            if (directorToEdit == null) throw new DirectorNotFoundException(directorid);

            directorToEdit.Name = director.Name.NormalizeString();
            directorToEdit.NormalizeName = directorToEdit.Name.RemoveMarks();
            directorToEdit.About = director.About;
            directorToEdit.DateOfBirth = director.DateOfBirth;
            directorToEdit.Avatar = director.Avatar;

            _unitOfWork.DirectorRepository.Update(directorToEdit);
            await _unitOfWork.SaveAsync();

            return directorToEdit;
        }
    }
}
