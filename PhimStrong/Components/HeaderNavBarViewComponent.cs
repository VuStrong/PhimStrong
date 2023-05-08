using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PhimStrong.Application.Interfaces;
using PhimStrong.Models.Category;
using PhimStrong.Models.Country;
using PhimStrong.Models.User;

namespace PhimStrong.Components
{
    public class HeaderNavBarViewComponent : ViewComponent
    {
		private readonly IUserService _userService;
		private readonly ICategoryService _categoryService;
		private readonly ICountryService _countryService;
        private readonly IMapper _mapper;

        public HeaderNavBarViewComponent(
			IUserService userService,
			ICategoryService categoryService,
			ICountryService countryService,
            IMapper mapper)
        {
			_userService = userService;
            _categoryService = categoryService;
            _countryService = countryService;
            _mapper = mapper;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            ViewData["Categories"] = _mapper.Map<List<CategoryViewModel>>(
                (await _categoryService.GetAllAsync()).ToList());
            
            ViewData["Countries"] = _mapper.Map<List<CountryViewModel>>(
                (await _countryService.GetAllAsync()).ToList());

            ViewData["User"] = _mapper.Map<UserViewModel>(
                await _userService.GetByClaims((System.Security.Claims.ClaimsPrincipal)User));

            List<int> years = new();
            int currentYear = DateTime.Now.Year;
            
            for (int i = currentYear - 22; i <= currentYear; i++)
            {
                years.Add(i);
            }
            ViewData["Years"] = years;
            
            return View();
        }
    }
}
