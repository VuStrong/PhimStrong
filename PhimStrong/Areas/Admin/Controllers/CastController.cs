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
    public class CastController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _environment;

        public CastController(AppDbContext db, IWebHostEnvironment environment)
        {
            _db = db;
            _environment = environment;
        }

        private const int CAST_PER_PAGE = 15;

        [HttpGet]
        public IActionResult Index(int page, string? filter = null)
        {
            if (page <= 0) page = 1;

            int numberOfPages = 0;

            List<Cast> casts = new List<Cast>();
            if (filter == null || filter.Trim() == "")
            {
                numberOfPages = (int)Math.Ceiling((double)_db.Casts.Count() / CAST_PER_PAGE);

                if (page > numberOfPages) page = numberOfPages;
                if (page <= 0) page = 1;

                casts = _db.Casts.Skip((page - 1) * CAST_PER_PAGE).Take(CAST_PER_PAGE).ToList();
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
                            casts = _db.Casts.Where(m =>
                                (m.Name ?? "").ToLower().Contains(filterValue.ToLower())
                            ).ToList();

                            TempData["FilterMessage"] = "tên là " + filterValue;

                            break;
                        default:
                            break;
                    }
                }

                numberOfPages = (int)Math.Ceiling((double)casts.Count / CAST_PER_PAGE);
                if (page > numberOfPages) page = numberOfPages;
                if (page <= 0) page = 1;

                casts = casts.Skip((page - 1) * CAST_PER_PAGE).Take(CAST_PER_PAGE).ToList();
            }

            TempData["NumberOfPages"] = numberOfPages;
            TempData["CurrentPage"] = page;
            TempData["filter"] = filter;

            return View(casts);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new Cast());
        }

        [HttpPost]
        public async Task<IActionResult> Create(Cast cast)
        {
            if (cast == null)
            {
                TempData["status"] = "Lỗi, không có diễn viên được chọn.";
                return View(cast);
            }

            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "Lỗi không thể thêm diễn viên.");
                return View();
            }

            // chỉnh lại format tên :
            cast.Name = Regex.Replace(cast.Name.ToLower(), @"(^\w)|(\s\w)", m => m.Value.ToUpper());

            try
            {
                _db.Casts.Add(cast);
                await _db.SaveChangesAsync();

                if (cast.AvatarFile != null)
                {
                    var file = Path.Combine(_environment.ContentRootPath, "wwwroot/src/img/CastAvatars", cast.Id.ToString() + ".jpg");

                    using (var fileStream = new FileStream(file, FileMode.Create))
                    {
                        await cast.AvatarFile.CopyToAsync(fileStream);
                    }

                    cast.Avatar = "/src/img/CastAvatars/" + cast.Id.ToString() + ".jpg";
                    await _db.SaveChangesAsync();
                }

            }
            catch (Exception e)
            {
                TempData["status"] = "Lỗi, " + e.Message;
                return View(cast);
            }

            TempData["success"] = $"Đã thêm diễn viên {cast.Name}.";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(int castid)
        {
            var cast = _db.Casts.FirstOrDefault(c => c.Id == castid);

            if (cast == null)
            {
                return NotFound("Không tìm thấy diễn viên.");
            }

            return View(cast);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int castid, Cast cast)
        {
            if (cast == null)
            {
                return NotFound("Không tìm thấy diễn viên.");
            }

            var castToEdit = _db.Casts.FirstOrDefault(c => c.Id == castid);

            if (castToEdit == null)
            {
                return NotFound("Không tìm thấy diễn viên.");
            }

            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "Lỗi không thể sửa.");
                return View(castToEdit);
            }

            if (cast.Name != castToEdit.Name) 
                castToEdit.Name = Regex.Replace(cast.Name.ToLower(), @"(^\w)|(\s\w)", m => m.Value.ToUpper());
            if(cast.About != castToEdit.About) castToEdit.About = cast.About;
            if(cast.DateOfBirth != castToEdit.DateOfBirth) castToEdit.DateOfBirth = cast.DateOfBirth;
            if (cast.AvatarFile != null)
            {
                var file = Path.Combine(_environment.ContentRootPath, "wwwroot/src/img/CastAvatars", castToEdit.Id.ToString() + ".jpg");

                using (var fileStream = new FileStream(file, FileMode.Create))
                {
                    await cast.AvatarFile.CopyToAsync(fileStream);
                }

                if(castToEdit.Avatar != "/src/img/CastAvatars/" + castToEdit.Id.ToString() + ".jpg")
                    castToEdit.Avatar = "/src/img/CastAvatars/" + castToEdit.Id.ToString() + ".jpg";
            }

            try
            {
                _db.Casts.Update(castToEdit);
                await _db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                TempData["status"] = "Lỗi, " + e.Message;
                return View(cast);
            }

            TempData["success"] = "Chỉnh sửa thành công";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int castid)
        {
            var cast = _db.Casts.FirstOrDefault(c => c.Id == castid);

            if (cast == null)
            {
                return NotFound("Không tìm thấy diễn viên.");
            }

            try
            {
                var file = Path.Combine(_environment.ContentRootPath, "wwwroot/src/img/CastAvatars", cast.Id.ToString() + ".jpg");

                FileInfo fileInfo = new FileInfo(file);
                fileInfo.Delete();

                _db.Casts.Remove(cast);
                await _db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                TempData["status"] = "Lỗi, " + e.Message;
                return RedirectToAction("Edit", new { castid = castid });
            }

            TempData["success"] = "Xóa thành công";
            return RedirectToAction("Index");
        }
    }
}

