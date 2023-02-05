using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhimStrong.Data;
using SharedLibrary.Constants;
using SharedLibrary.Helpers;
using SharedLibrary.Models;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;

namespace PhimStrong.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = $"{RoleConstant.ADMIN}, {RoleConstant.THUY_TO}")]
    public class CategoryController : Controller
    {
        private readonly AppDbContext _db;

        public CategoryController(AppDbContext db)
        {
            _db = db;
        }

        private const int CATE_PER_PAGE = 15;

        [HttpGet]
        public IActionResult Index(int page, string? filter = null)
        {
            if (page <= 0) page = 1;

            int numberOfPages = 0;

            List<Category> categories = new List<Category>();
			int count = 0; // total of search result
			if (filter == null || filter.Trim() == "")
            {
                count = _db.Categories.Count();

				numberOfPages = (int)Math.Ceiling((double)count / CATE_PER_PAGE);
				TempData["TotalCount"] = count;

				if (page > numberOfPages) page = numberOfPages;
                if (page <= 0) page = 1;

                categories = _db.Categories.Skip((page - 1) * CATE_PER_PAGE).Take(CATE_PER_PAGE).ToList();
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

                            categories = _db.Categories.ToList().Where(m => (m.NormalizeName ?? "").Contains(filterValue)).ToList();

                            break;
                        default:
                            break;
                    }
                }

				count = categories.Count;
				TempData["TotalCount"] = count;

				numberOfPages = (int)Math.Ceiling((double)count / CATE_PER_PAGE);
                if (page > numberOfPages) page = numberOfPages;
                if (page <= 0) page = 1;

                categories = categories.Skip((page - 1) * CATE_PER_PAGE).Take(CATE_PER_PAGE).ToList();
            }

            TempData["NumberOfPages"] = numberOfPages;
            TempData["CurrentPage"] = page;
            TempData["filter"] = filter;

            return View(categories);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new Category());
        }

        [HttpPost]
        public async Task<IActionResult> Create(Category category)
        {
            if (category == null)
            {
                TempData["status"] = "Lỗi, không có thể loại được chọn.";
                return View(category);
            }

            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "Lỗi không thể thêm.");
                return View();
            }

            category.IdNumber = _db.Categories.Any() ? _db.Categories.Max(x => x.IdNumber) + 1 : 1;
            category.Id = "cate" + category.IdNumber.ToString();

            // chỉnh lại format tên :
            category.Name = category.Name[0].ToString().ToUpper() + category.Name.Substring(1).ToLower();
            category.NormalizeName = category.Name.RemoveMarks();

            try
            {
                _db.Categories.Add(category);
                await _db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                TempData["status"] = "Lỗi, " + e.Message;
                return View(category);
            }

            TempData["success"] = $"Đã thêm thể loại {category.Name}.";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(string cateid)
        {
            var category = _db.Categories.FirstOrDefault(c => c.Id == cateid);

            if (category == null)
            {
                return NotFound("Không tìm thấy thể loại.");
            }

            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string cateid, Category category)
        {
            if (category == null)
            {
                return NotFound("Không tìm thấy thể loại.");
            }

            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "Lỗi không thể sửa.");
                category.Id = cateid;
                return View(category);
            }

            var categoryToEdit = _db.Categories.FirstOrDefault(c => c.Id == cateid);

            if (categoryToEdit == null)
            {
                return NotFound("Không tìm thấy thể loại.");
            }

            categoryToEdit.Name = category.Name[0].ToString().ToUpper() + category.Name.Substring(1).ToLower();
            categoryToEdit.NormalizeName = categoryToEdit.Name.RemoveMarks();
            categoryToEdit.Description = category.Description;

            try
            {
                _db.Categories.Update(categoryToEdit);
                await _db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                TempData["status"] = "Lỗi, " + e.Message;
                return View(category);
            }

            TempData["success"] = "Chỉnh sửa thành công";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string cateid)
        {
            var category = _db.Categories.FirstOrDefault(c => c.Id == cateid);

            if (category == null)
            {
                return NotFound("Không tìm thấy thể loại.");
            }

            try
            {
                _db.Categories.Remove(category);
                await _db.SaveChangesAsync();
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
