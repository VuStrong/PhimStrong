using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhimStrong.Application.Interfaces;
using PhimStrong.Areas.Admin.Models.Category;
using PhimStrong.Domain.Models;
using PhimStrong.Domain.PagingModel;
using PhimStrong.Models.Category;
using SharedLibrary.Constants;

namespace PhimStrong.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = $"{RoleConstant.ADMIN}, {RoleConstant.THUY_TO}")]
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly IMapper _mapper;

        public CategoryController(ICategoryService categoryService, IMapper mapper)
        {
            _categoryService = categoryService;
            _mapper = mapper;
        }

        private const int CATE_PER_PAGE = 15;

        [HttpGet]
        public async Task<IActionResult> Index(int page, string? value = null)
        {
            PagedList<Category> categories = await _categoryService.SearchAsync(value, new PagingParameter(page, CATE_PER_PAGE));

            if (value != null) ViewData["value"] = value;

            return View(_mapper.Map<PagedList<CategoryViewModel>>(categories));
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new CreateCategoryViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateCategoryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "Lỗi không thể thêm.");
                return View();
            }

            Category category = _mapper.Map<Category>(model);
            try
            {
                await _categoryService.CreateAsync(category);
            }
            catch (Exception e)
            {
                TempData["status"] = "Lỗi, " + e.Message;
                return View(model);
            }

            TempData["success"] = $"Đã thêm thể loại {model.Name}.";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string cateid)
        {
            var category = await _categoryService.GetByIdAsync(cateid);

            if (category == null)
            {
                return NotFound("Không tìm thấy thể loại.");
            }

            return View(_mapper.Map<EditCategoryViewModel>(category));
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string cateid, EditCategoryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "Lỗi không thể sửa.");
                return View(model);
            }

            Category category = _mapper.Map<Category>(model);
            try
            {
                await _categoryService.UpdateAsync(cateid, category);
            }
            catch (Exception e)
            {
                TempData["status"] = "Lỗi, " + e.Message;
                return View(model);
            }

            TempData["success"] = "Chỉnh sửa thành công";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string cateid)
        {
            try
            {
                await _categoryService.DeleteAsync(cateid);
            }
            catch (Exception e)
            {
                TempData["status"] = "Lỗi, " + e.Message;
                return RedirectToAction("Edit", new { cateid = cateid});
            }

            TempData["success"] = "Xóa thành công";
            return RedirectToAction("Index");
        }
    }
}
