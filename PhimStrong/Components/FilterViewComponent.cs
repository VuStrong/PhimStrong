using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PhimStrong.Application.Interfaces;
using PhimStrong.Models;
using PhimStrong.Models.Category;
using PhimStrong.Models.Country;
using PhimStrong.Models.User;

namespace PhimStrong.Components
{
    public class FilterViewComponent : ViewComponent
    {
		private readonly ICategoryService _categoryService;
		private readonly ICountryService _countryService;
        private readonly IMapper _mapper;

        public FilterViewComponent(
			ICategoryService categoryService,
			ICountryService countryService,
            IMapper mapper)
        {
            _categoryService = categoryService;
            _countryService = countryService;
            _mapper = mapper;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            List<SelectListItem> categories = (await _categoryService.GetAllAsync()).Select(c => new SelectListItem
            {
                Value = c.Id,
                Text = c.Name
            }).ToList();
            
            SelectList countries = new(await _countryService.GetAllAsync(), "Id", "Name");

            int curYear = DateTime.Now.Year;
            SelectList years = new(Enumerable.Range(curYear - 23, 24));

			SelectList orderByOptions = new(new List<object>()
            {
                new {
                    Field = "Thời gian cập nhật",
                    Value = "createdDate_desc"
				},
				new {
					Field = "Lượt xem",
					Value = "view_desc"
				},
				new {
					Field = "Năm sản xuất",
					Value = "releaseDate_desc"
				}
			}, "Value", "Field");

			SelectList types = new(new List<object>()
			{
				new {
					Field = "Phim lẻ",
					Value = "phimle"
				},
				new {
					Field = "Phim bộ",
					Value = "phimbo"
				}
			}, "Value", "Field");

			return View(new FilterViewModel
            {
                Categories = categories,
                Countries = countries,
                Years = years,
                OrderByOptions = orderByOptions,
                Types = types
            });
        }
    }
}
