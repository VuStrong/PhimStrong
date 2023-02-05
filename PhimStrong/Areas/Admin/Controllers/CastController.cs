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
			int count = 0; // total of search result
			if (filter == null || filter.Trim() == "")
            {
                count = _db.Casts.Count();

				numberOfPages = (int)Math.Ceiling((double)count / CAST_PER_PAGE);
				TempData["TotalCount"] = count;

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
                            TempData["FilterMessage"] = "tên là " + filterValue;
                            filterValue = filterValue.RemoveMarks();

                            casts = _db.Casts.ToList().Where(m =>
                                (m.NormalizeName ?? "").Contains(filterValue)
                            ).ToList();

                            break;
                        default:
                            break;
                    }
                }

                count = casts.Count;
				TempData["TotalCount"] = count;

				numberOfPages = (int)Math.Ceiling((double)count / CAST_PER_PAGE);
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

            cast.IdNumber = _db.Casts.Any() ? _db.Casts.Max(x => x.IdNumber) + 1 : 1;
            cast.Id = "cast" + cast.IdNumber.ToString();

            // chỉnh lại format tên :
            cast.Name = Regex.Replace(cast.Name.ToLower().Trim(), @"(^\w)|(\s\w)", m => m.Value.ToUpper());
            cast.NormalizeName = cast.Name.RemoveMarks();

            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                _db.Casts.Add(cast);
                await _db.SaveChangesAsync();

                if (cast.AvatarFile != null)
                {
                    var file = Path.Combine(_environment.WebRootPath, "src/img/CastAvatars", cast.Id + ".jpg");

                    using (var fileStream = new FileStream(file, FileMode.Create))
                    {
                        await cast.AvatarFile.CopyToAsync(fileStream);
                    }

                    cast.Avatar = "/src/img/CastAvatars/" + cast.Id + ".jpg";
                    await _db.SaveChangesAsync();
                }

                await transaction.CommitAsync();
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                TempData["status"] = "Lỗi, " + e.Message;
                return View(cast);
            }

            TempData["success"] = $"Đã thêm diễn viên {cast.Name}.";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(string castid)
        {
            var cast = _db.Casts.FirstOrDefault(c => c.Id == castid);

            if (cast == null)
            {
                return NotFound("Không tìm thấy diễn viên.");
            }

            return View(cast);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string castid, Cast cast)
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
            {
                castToEdit.Name = Regex.Replace(cast.Name.ToLower().Trim(), @"(^\w)|(\s\w)", m => m.Value.ToUpper());
                castToEdit.NormalizeName = castToEdit.Name.RemoveMarks();
            }
            if(cast.About != castToEdit.About) castToEdit.About = cast.About;
            if(cast.DateOfBirth != castToEdit.DateOfBirth) castToEdit.DateOfBirth = cast.DateOfBirth;
            if (cast.AvatarFile != null)
            {
                var file = Path.Combine(_environment.WebRootPath, "src/img/CastAvatars", castToEdit.Id + ".jpg");

                using (var fileStream = new FileStream(file, FileMode.Create))
                {
                    await cast.AvatarFile.CopyToAsync(fileStream);
                }

                if(castToEdit.Avatar != "/src/img/CastAvatars/" + castToEdit.Id + ".jpg")
                    castToEdit.Avatar = "/src/img/CastAvatars/" + castToEdit.Id + ".jpg";
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
        public async Task<IActionResult> Delete(string castid)
        {
            var cast = _db.Casts.FirstOrDefault(c => c.Id == castid);

            if (cast == null)
            {
                return NotFound("Không tìm thấy diễn viên.");
            }

            try
            {
                var file = Path.Combine(_environment.WebRootPath, "src\\img\\CastAvatars", cast.Id + ".jpg");
                
                if (System.IO.File.Exists(file)) System.IO.File.Delete(file);

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

