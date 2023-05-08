using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhimStrong.Application.Interfaces;
using PhimStrong.Areas.Admin.Models.Country;
using PhimStrong.Domain.Models;
using PhimStrong.Domain.PagingModel;
using PhimStrong.Models.Country;
using SharedLibrary.Constants;

namespace PhimStrong.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = $"{RoleConstant.ADMIN}, {RoleConstant.THUY_TO}")]
    public class CountryController : Controller
    {
        private readonly ICountryService _countryService;
        private readonly IMapper _mapper;

        public CountryController(ICountryService countryService, IMapper mapper)
        {
            _countryService = countryService;
            _mapper = mapper;
        }

        private const int COUNTRY_PER_PAGE = 15;

        [HttpGet]
        public async Task<IActionResult> Index(int page, string? value = null)
        {
            PagedList<Country> countries = await _countryService.SearchAsync(value, new PagingParameter(page, COUNTRY_PER_PAGE));

            if (value != null) ViewData["value"] = value;

            return View(_mapper.Map<PagedList<CountryViewModel>>(countries));
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new CreateCountryViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateCountryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "Lỗi không thể thêm quốc gia.");
                return View(model);
            }

            Country country = _mapper.Map<Country>(model);
            try
            {
                await _countryService.CreateAsync(country);
            }
            catch (Exception e)
            {
                TempData["status"] = "Lỗi, " + e.Message;
                return View(model);
            }

            TempData["success"] = $"Đã thêm quốc gia {model.Name}.";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string countryid)
        {
            var country = await _countryService.GetByIdAsync(countryid);

            if (country == null)
            {
                return NotFound("Không tìm thấy quốc gia.");
            }

            return View(_mapper.Map<EditCountryViewModel>(country));
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string countryid, EditCountryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "Lỗi không thể sửa.");
                return View(model);
            }

            Country country = _mapper.Map<Country>(model);
            try
            {
                await _countryService.UpdateAsync(countryid, country);
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
        public async Task<IActionResult> Delete(string countryid)
        {
            try
            {
                await _countryService.DeleteAsync(countryid);
            }
            catch (Exception e)
            {
                TempData["status"] = "Lỗi, " + e.Message;
                return RedirectToAction("Edit", new { countryid = countryid });
            }

            TempData["success"] = "Xóa thành công";
            return RedirectToAction("Index");
        }
    }
}

