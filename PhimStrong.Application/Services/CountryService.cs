using PhimStrong.Application.Interfaces;
using PhimStrong.Domain.Exceptions.NotFound;
using PhimStrong.Domain.Interfaces;
using PhimStrong.Domain.Models;
using PhimStrong.Domain.PagingModel;
using PhimStrong.Domain.Parameters;
using SharedLibrary.Helpers;

namespace PhimStrong.Application.Services
{
    public class CountryService : ICountryService
	{
		private readonly IUnitOfWork _unitOfWork;

        public CountryService(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
        }

        public async Task<Country> CreateAsync(Country country)
        {
            country.IdNumber = await _unitOfWork.CountryRepository.AnyAsync() ?
                await _unitOfWork.CountryRepository.MaxIdNumberAsync() + 1 : 1;

            country.Id = "country" + country.IdNumber.ToString();

            // chỉnh lại format tên :
            country.Name = country.Name.NormalizeString();
            country.NormalizeName = country.Name.RemoveMarks();

            _unitOfWork.CountryRepository.Create(country);
            await _unitOfWork.SaveAsync();

            return country;
        }

        public async Task DeleteAsync(string countryid)
        {
            Country? country = await _unitOfWork.CountryRepository.FirstOrDefaultAsync(c => c.Id == countryid);

            if (country == null) throw new CountryNotFoundException(countryid);

            _unitOfWork.CountryRepository.Delete(country);
            await _unitOfWork.SaveAsync();
        }

        public async Task<PagedList<Country>> GetAllAsync(PagingParameter pagingParameter)
        {
            return await _unitOfWork.CountryRepository.GetAsync(pagingParameter);
        }

        public async Task<IEnumerable<Country>> GetAllAsync()
        {
            return await _unitOfWork.CountryRepository.GetAsync();
        }

        public async Task<Country?> GetByIdAsync(string id)
        {
            return await _unitOfWork.CountryRepository.FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Country?> GetByNameAsync(string name)
		{
			name = name.RemoveMarks();

			return await _unitOfWork.CountryRepository.FirstOrDefaultAsync(x => x.NormalizeName == name);
		}

        public async Task<PagedList<Country>> SearchAsync(string? value, PagingParameter pagingParameter)
        {
            value = value?.RemoveMarks();

            PagedList<Country> countries = value != null ?
                await _unitOfWork.CountryRepository.GetAsync(
                    pagingParameter: pagingParameter,
                    c => (c.NormalizeName ?? "").Contains(value)) :
                await this.GetAllAsync(pagingParameter);

            return countries;
        }

        public async Task<Country> UpdateAsync(string countryid, Country country)
        {
            Country? countryToEdit = await _unitOfWork.CountryRepository.FirstOrDefaultAsync(c => c.Id == countryid);

            if (countryToEdit == null) throw new CountryNotFoundException(countryid);

            countryToEdit.Name = country.Name.NormalizeString();
            countryToEdit.NormalizeName = countryToEdit.Name.RemoveMarks();
            countryToEdit.About = country.About;

            _unitOfWork.CountryRepository.Update(countryToEdit);
            await _unitOfWork.SaveAsync();

            return countryToEdit;
        }
    }
}
