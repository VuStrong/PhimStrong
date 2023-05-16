using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhimStrong.Application.Interfaces;
using PhimStrong.Areas.Admin.Models.Director;
using PhimStrong.Domain.Models;
using PhimStrong.Domain.PagingModel;
using PhimStrong.Domain.Parameters;
using PhimStrong.Models.Director;
using SharedLibrary.Constants;

namespace PhimStrong.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = $"{RoleConstant.ADMIN}, {RoleConstant.THUY_TO}")]
    public class DirectorController : Controller
    {
        private readonly IDirectorService _directorService;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _environment;

        public DirectorController(
            IDirectorService directorService,
            IMapper mapper,
            IWebHostEnvironment environment)
        {
            _mapper = mapper;
            _directorService = directorService;
            _environment = environment;
        }

        private const int DIRECTOR_PER_PAGE = 15;

        [HttpGet]
        public async Task<IActionResult> Index(int page, string? value = null)
        {
            PagedList<Director> directors = await _directorService.SearchAsync(value, new PagingParameter(page, DIRECTOR_PER_PAGE));

            if (value != null) ViewData["value"] = value;

            return View(_mapper.Map<PagedList<DirectorViewModel>>(directors));
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new CreateDirectorViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateDirectorViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "Lỗi không thể thêm đạo diễn.");
                return View(model);
            }

            Director director = _mapper.Map<Director>(model);
            try
            {
                await _directorService.CreateAsync(director);

                // nếu có hình ảnh thì update lại director và lưu vào file
                if (model.AvatarFile != null)
                {
                    director.Avatar = "/src/img/DirectorAvatars/" + director.Id + ".jpg";
                    await _directorService.UpdateAsync(director.Id, director);

                    var file = Path.Combine(_environment.WebRootPath, "src/img/DirectorAvatars", director.Id + ".jpg");

                    using var fileStream = new FileStream(file, FileMode.Create);
                    await model.AvatarFile.CopyToAsync(fileStream);
                }
            }
            catch (Exception e)
            {
                TempData["status"] = "Lỗi, " + e.Message;
                return View(model);
            }

            TempData["success"] = $"Đã thêm đạo diễn {model.Name}.";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string directorid)
        {
            var director = await _directorService.GetByIdAsync(directorid);

            if (director == null)
            {
                return NotFound("Không tìm thấy đạo diễn.");
            }

            return View(_mapper.Map<EditDirectorViewModel>(director));
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string directorid, EditDirectorViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "Lỗi không thể sửa.");
                return View(model);
            }

            Director director = _mapper.Map<Director>(model);

            if (model.AvatarFile != null)
                director.Avatar = "/src/img/DirectorAvatars/" + director.Id + ".jpg";

            try
            {
                await _directorService.UpdateAsync(directorid, director);

                // nếu có hình ảnh thì update lại director và lưu vào file
                if (model.AvatarFile != null)
                {
                    var file = Path.Combine(_environment.ContentRootPath, "wwwroot/src/img/DirectorAvatars", directorid + ".jpg");

                    using var fileStream = new FileStream(file, FileMode.Create);
                    await model.AvatarFile.CopyToAsync(fileStream);
                }
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
        public async Task<IActionResult> Delete(string directorid)
        {
            try
            {
                await _directorService.DeleteAsync(directorid);

                var file = Path.Combine(_environment.WebRootPath, "src\\img\\DirectorAvatars", directorid + ".jpg");

                if (System.IO.File.Exists(file)) System.IO.File.Delete(file);
            }
            catch (Exception e)
            {
                TempData["status"] = "Lỗi, " + e.Message;
                return RedirectToAction("Edit", new { directorid = directorid });
            }

            TempData["success"] = "Xóa thành công";
            return RedirectToAction("Index");
        }
    }
}

