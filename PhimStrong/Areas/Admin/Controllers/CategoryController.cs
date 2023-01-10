using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhimStrong.Data;
using SharedLibrary.Constants;
using SharedLibrary.Models;
using System.Data;
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
            if (filter == null || filter.Trim() == "")
            {
                numberOfPages = (int)Math.Ceiling((double)_db.Categories.Count() / CATE_PER_PAGE);

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
                            categories = _db.Categories.Where(m =>
                                (m.Name ?? "").ToLower().Contains(filterValue.ToLower())
                            ).ToList();

                            TempData["FilterMessage"] = "tên là " + filterValue;

                            break;
                        default:
                            break;
                    }
                }

                numberOfPages = (int)Math.Ceiling((double)categories.Count / CATE_PER_PAGE);
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
        public IActionResult Create(Category category)
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

            // chỉnh lại format tên :
            category.Name = category.Name[0].ToString().ToUpper() + category.Name.Substring(1).ToLower();

            try
            {
                _db.Categories.Add(category);
                _db.SaveChanges();
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
        public IActionResult Edit(int cateid)
        {
            var category = _db.Categories.FirstOrDefault(c => c.Id == cateid);

            if (category == null)
            {
                return NotFound("Không tìm thấy thể loại.");
            }

            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int cateid, Category category)
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
        public async Task<IActionResult> Delete(int cateid)
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
