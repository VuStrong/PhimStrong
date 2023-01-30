using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhimStrong.Data;
using SharedLibrary.Constants;
using SharedLibrary.Helpers;
using SharedLibrary.Models;
using System.Data;
using System.Text.RegularExpressions;

namespace PhimStrong.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = $"{RoleConstant.ADMIN}, {RoleConstant.THUY_TO}")]
    public class CountryController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _environment;

        public CountryController(AppDbContext db, IWebHostEnvironment environment)
        {
            _db = db;
            _environment = environment;
        }

        private const int COUNTRY_PER_PAGE = 15;

        [HttpGet]
        public IActionResult Index(int page, string? filter = null)
        {
            if (page <= 0) page = 1;

            int numberOfPages = 0;

            List<Country> countries = new List<Country>();
			int count = 0; // total of search result
			if (filter == null || filter.Trim() == "")
            {
                count = _db.Countries.Count();

                numberOfPages = (int)Math.Ceiling((double)count / COUNTRY_PER_PAGE);
				TempData["TotalCount"] = count;

				if (page > numberOfPages) page = numberOfPages;
                if (page <= 0) page = 1;

                countries = _db.Countries.Skip((page - 1) * COUNTRY_PER_PAGE).Take(COUNTRY_PER_PAGE).ToList();
            }
            else
            {
                MatchCollection match = Regex.Matches(filter ?? "", @"^<.+>");

                if (match.Count > 0)
                {
                    string matchValue = new Regex(@"<|>").Replace(match[0].ToString(), "");

                    string filterValue = new Regex(@"^<.+>").Replace(filter ?? "", "");
                    switch (matchValue)
                    {
                        case PageFilterConstant.FILTER_BY_NAME:
                            TempData["FilterMessage"] = "tên là " + filterValue;
                            filterValue = filterValue.RemoveMarks();

                            countries = _db.Countries.Where(m =>
                                (m.NormalizeName ?? "").Contains(filterValue)
                            ).ToList();

                            break;
                        default:
                            break;
                    }
                }

                count = countries.Count;
				TempData["TotalCount"] = count;

				numberOfPages = (int)Math.Ceiling((double)count / COUNTRY_PER_PAGE);
                if (page > numberOfPages) page = numberOfPages;
                if (page <= 0) page = 1;

                countries = countries.Skip((page - 1) * COUNTRY_PER_PAGE).Take(COUNTRY_PER_PAGE).ToList();
            }

            TempData["NumberOfPages"] = numberOfPages;
            TempData["CurrentPage"] = page;
            TempData["filter"] = filter;

            return View(countries);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new Country());
        }

        [HttpPost]
        public async Task<IActionResult> Create(Country country)
        {
            if (country == null)
            {
                TempData["status"] = "Lỗi, không có quốc gia được chọn.";
                return View(country);
            }

            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "Lỗi không thể thêm quốc gia.");
                return View();
            }

            // chỉnh lại format tên :
            country.Name = Regex.Replace(country.Name.ToLower().Trim(), @"(^\w)|(\s\w)", m => m.Value.ToUpper());
            country.NormalizeName = country.Name.RemoveMarks();

            try
            {
                _db.Countries.Add(country);
                await _db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                TempData["status"] = "Lỗi, " + e.Message;
                return View(country);
            }

            TempData["success"] = $"Đã thêm quốc gia {country.Name}.";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(int countryid)
        {
            var country = _db.Countries.FirstOrDefault(c => c.Id == countryid);

            if (country == null)
            {
                return NotFound("Không tìm thấy quốc gia.");
            }

            return View(country);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int countryid, Country country)
        {
            if (country == null)
            {
                return NotFound("Không tìm thấy quốc gia.");
            }

            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "Lỗi không thể sửa.");
                country.Id = countryid;
                return View(country);
            }

            var countryToEdit = _db.Countries.FirstOrDefault(c => c.Id == countryid);

            if (countryToEdit == null)
            {
                return NotFound("Không tìm thấy quốc gia.");
            }

            if(country.Name != countryToEdit.Name)
            {
                countryToEdit.Name = Regex.Replace(country.Name.ToLower().Trim(), @"(^\w)|(\s\w)", m => m.Value.ToUpper());
                countryToEdit.NormalizeName = countryToEdit.Name.RemoveMarks();
			}
            if(country.About != countryToEdit.About) countryToEdit.About = country.About;

            try
            {
                _db.Countries.Update(countryToEdit);
                await _db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                TempData["status"] = "Lỗi, " + e.Message;
                return View(country);
            }

            TempData["success"] = "Chỉnh sửa thành công";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int countryid)
        {
            var country = _db.Countries.FirstOrDefault(c => c.Id == countryid);

            if (country == null)
            {
                return NotFound("Không tìm thấy quốc gia.");
            }

            try
            {
                _db.Countries.Remove(country);
                await _db.SaveChangesAsync();
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

