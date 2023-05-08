using PhimStrong.Application.Interfaces;
using PhimStrong.Domain.Exceptions.NotFound;
using PhimStrong.Domain.Interfaces;
using PhimStrong.Domain.Models;
using PhimStrong.Domain.PagingModel;
using SharedLibrary.Helpers;

namespace PhimStrong.Application.Services
{
    public class CategoryService : ICategoryService
	{
		private readonly IUnitOfWork _unitOfWork;

        public CategoryService(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
        }

		public async Task<Category> CreateAsync(Category category)
		{
            category.IdNumber = await _unitOfWork.CategoryRepository.AnyAsync() ?
                await _unitOfWork.CategoryRepository.MaxIdNumberAsync() + 1 : 1;

            category.Id = "cate" + category.IdNumber.ToString();

            // chỉnh lại format tên :
            category.Name = category.Name.NormalizeString();
            category.NormalizeName = category.Name.RemoveMarks();

            _unitOfWork.CategoryRepository.Create(category);
            await _unitOfWork.SaveAsync();

            return category;
        }

		public async Task DeleteAsync(string categoryid)
		{
            Category? category = await _unitOfWork.CategoryRepository.FirstOrDefaultAsync(c => c.Id == categoryid);

            if (category == null) throw new CategoryNotFoundException(categoryid);

            _unitOfWork.CategoryRepository.Delete(category);
            await _unitOfWork.SaveAsync();
        }

		public async Task<IEnumerable<Category>> GetAllAsync()
		{
			return await _unitOfWork.CategoryRepository.GetAsync();
		}

        public async Task<PagedList<Category>> GetAllAsync(PagingParameter pagingParameter)
        {
            return await _unitOfWork.CategoryRepository.GetAsync(pagingParameter);
        }

        public async Task<Category?> GetByIdAsync(string id)
		{
            return await _unitOfWork.CategoryRepository.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Category?> GetByNameAsync(string name)
		{
			name = name.RemoveMarks();

			return await _unitOfWork.CategoryRepository.FirstOrDefaultAsync(x => x.NormalizeName == name);
		}

        public async Task<PagedList<Category>> SearchAsync(string? value, PagingParameter pagingParameter)
        {
            value = value?.RemoveMarks();

            PagedList<Category> categories = value != null ?
                await _unitOfWork.CategoryRepository.GetAsync(
                    pagingParameter: pagingParameter,
                    c => (c.NormalizeName ?? "").Contains(value)) :
                await this.GetAllAsync(pagingParameter);

            return categories;
        }
        public async Task<Category> UpdateAsync(string categoryid, Category category)
		{
            Category? categoryToEdit = await _unitOfWork.CategoryRepository.FirstOrDefaultAsync(c => c.Id == categoryid);

            if (categoryToEdit == null) throw new CategoryNotFoundException(categoryid);

            categoryToEdit.Name = category.Name.NormalizeString();
            categoryToEdit.NormalizeName = categoryToEdit.Name.RemoveMarks();
            categoryToEdit.Description = category.Description;

            _unitOfWork.CategoryRepository.Update(categoryToEdit);
            await _unitOfWork.SaveAsync();

            return categoryToEdit;
        }
	}
}
