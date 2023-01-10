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
    public class DirectorController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _environment;

        public DirectorController(AppDbContext db, IWebHostEnvironment environment)
        {
            _db = db;
            _environment = environment;
        }

        private const int DIRECTOR_PER_PAGE = 15;

        [HttpGet]
        public IActionResult Index(int page, string? filter = null)
        {
            if (page <= 0) page = 1;

            int numberOfPages = 0;

            List<Director> directors = new List<Director>();
            if (filter == null || filter.Trim() == "")
            {
                numberOfPages = (int)Math.Ceiling((double)_db.Directors.Count() / DIRECTOR_PER_PAGE);

                if (page > numberOfPages) page = numberOfPages;
                if (page <= 0) page = 1;

				directors = _db.Directors.Skip((page - 1) * DIRECTOR_PER_PAGE).Take(DIRECTOR_PER_PAGE).ToList();
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
							directors = _db.Directors.Where(m =>
                                (m.Name ?? "").ToLower().Contains(filterValue.ToLower())
                            ).ToList();

                            TempData["FilterMessage"] = "tên là " + filterValue;

                            break;
                        default:
                            break;
                    }
                }

                numberOfPages = (int)Math.Ceiling((double)directors.Count / DIRECTOR_PER_PAGE);
                if (page > numberOfPages) page = numberOfPages;
                if (page <= 0) page = 1;

                directors = directors.Skip((page - 1) * DIRECTOR_PER_PAGE).Take(DIRECTOR_PER_PAGE).ToList();
            }

            TempData["NumberOfPages"] = numberOfPages;
            TempData["CurrentPage"] = page;
            TempData["filter"] = filter;

            return View(directors);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new Director());
        }

        [HttpPost]
        public async Task<IActionResult> Create(Director director)
        {
            if (director == null)
            {
                TempData["status"] = "Lỗi, không có đạo diễn được chọn.";
                return View(director);
            }

            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "Lỗi không thể thêm đạo diễn.");
                return View();
            }

            // chỉnh lại format tên :
            director.Name = Regex.Replace(director.Name.ToLower(), @"(^\w)|(\s\w)", m => m.Value.ToUpper());

            try
            {
                _db.Directors.Add(director);
                await _db.SaveChangesAsync();

                if (director.AvatarFile != null)
                {
                    var file = Path.Combine(_environment.ContentRootPath, "wwwroot/src/img/DirectorAvatars", director.Id.ToString() + ".jpg");

                    using (var fileStream = new FileStream(file, FileMode.Create))
                    {
                        await director.AvatarFile.CopyToAsync(fileStream);
                    }

                    director.Avatar = "/src/img/DirectorAvatars/" + director.Id.ToString() + ".jpg";

                    await _db.SaveChangesAsync();
                }
            }
            catch (Exception e)
            {
                TempData["status"] = "Lỗi, " + e.Message;
                return View(director);
            }

            TempData["success"] = $"Đã thêm đạo diễn {director.Name}.";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(int directorid)
        {
            var director = _db.Directors.FirstOrDefault(c => c.Id == directorid);

            if (director == null)
            {
                return NotFound("Không tìm thấy đạo diễn.");
            }

            return View(director);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int directorid, Director director)
        {
            if (director == null)
            {
                return NotFound("Không tìm thấy đạo diễn.");
            }

            var directorToEdit = _db.Directors.FirstOrDefault(c => c.Id == directorid);

            if (directorToEdit == null)
            {
                return NotFound("Không tìm thấy đạo diễn.");
            }

            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "Lỗi không thể sửa.");
                return View(directorToEdit);
            }

            if (director.Name != directorToEdit.Name) 
                directorToEdit.Name = Regex.Replace(director.Name.ToLower(), @"(^\w)|(\s\w)", m => m.Value.ToUpper());
            if(director.About != directorToEdit.About) directorToEdit.About = director.About;
            if(director.DateOfBirth != directorToEdit.DateOfBirth) directorToEdit.DateOfBirth = director.DateOfBirth;
            if (director.AvatarFile != null)
            {
                var file = Path.Combine(_environment.ContentRootPath, "wwwroot/src/img/DirectorAvatars", directorToEdit.Id.ToString() + ".jpg");

                using (var fileStream = new FileStream(file, FileMode.Create))
                {
                    await director.AvatarFile.CopyToAsync(fileStream);
                }

                if(directorToEdit.Avatar != "/src/img/DirectorAvatars/" + directorToEdit.Id.ToString() + ".jpg")
                    directorToEdit.Avatar = "/src/img/DirectorAvatars/" + directorToEdit.Id.ToString() + ".jpg";
            }

            try
            {
                _db.Directors.Update(directorToEdit);
                await _db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                TempData["status"] = "Lỗi, " + e.Message;
                return View(director);
            }

            TempData["success"] = "Chỉnh sửa thành công";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int directorid)
        {
            var director = _db.Directors.FirstOrDefault(c => c.Id == directorid);

            if (director == null)
            {
                return NotFound("Không tìm thấy đạo diễn.");
            }

            try
            {
                var file = Path.Combine(_environment.ContentRootPath, "wwwroot/src/img/DirectorAvatars", director.Id.ToString() + ".jpg");

                FileInfo fileInfo = new FileInfo(file);
                fileInfo.Delete();

                _db.Directors.Remove(director);
                await _db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                TempData["status"] = "Lỗi, " + e.Message;
                return RedirectToAction("Edit", new { castid = directorid });
            }

            TempData["success"] = "Xóa thành công";
            return RedirectToAction("Index");
        }
    }
}

