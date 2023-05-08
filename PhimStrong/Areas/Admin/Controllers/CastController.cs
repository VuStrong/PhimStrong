using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhimStrong.Application.Interfaces;
using PhimStrong.Areas.Admin.Models.Cast;
using PhimStrong.Domain.Models;
using PhimStrong.Domain.PagingModel;
using PhimStrong.Models.Cast;
using SharedLibrary.Constants;

namespace PhimStrong.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = $"{RoleConstant.ADMIN}, {RoleConstant.THUY_TO}")]
    public class CastController : Controller
    {
        private readonly ICastService _castService;
        private readonly IMapper _mapper;
		private readonly IWebHostEnvironment _environment;

        public CastController(
            ICastService castService,
            IMapper mapper,
            IWebHostEnvironment environment)
        {
            _mapper = mapper;
            _castService = castService;
            _environment = environment;
        }

        private const int CASTS_PER_PAGE = 15;

        [HttpGet]
        public async Task<IActionResult> Index(int page, string? value = null)
        {
			PagedList<Cast> casts = await _castService.SearchAsync(value, new PagingParameter(page, CASTS_PER_PAGE));

            if(value != null) ViewData["value"] = value;

			return View(_mapper.Map<PagedList<CastViewModel>>(casts));
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new CreateCastViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateCastViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "Lỗi không thể thêm diễn viên.");
                return View(model);
            }

            Cast cast = _mapper.Map<Cast>(model);

            try
            {
                await _castService.CreateAsync(cast);

                // nếu có hình ảnh thì update lại cast và lưu vào file
				if (model.AvatarFile != null)
				{
                    cast.Avatar = "/src/img/CastAvatars/" + cast.Id + ".jpg";
                    await _castService.UpdateAsync(cast.Id, cast);

                    var file = Path.Combine(_environment.WebRootPath, "src/img/CastAvatars", cast.Id + ".jpg");

					using var fileStream = new FileStream(file, FileMode.Create);
					await model.AvatarFile.CopyToAsync(fileStream);
				}
			}
            catch(Exception e)
            {
				TempData["status"] = "Lỗi, " + e.Message;
				return View(model);
			}

            TempData["success"] = $"Đã thêm diễn viên {model.Name}.";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string castid)
        {
            var cast = await _castService.GetByIdAsync(castid);

            if (cast == null)
            {
                return NotFound("Không tìm thấy diễn viên.");
            }

            return View(_mapper.Map<EditCastViewModel>(cast));
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string castid, EditCastViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "Lỗi không thể sửa.");
                return View(model);
            }

            Cast cast = _mapper.Map<Cast>(model);

            if (model.AvatarFile != null)
                cast.Avatar = "/src/img/CastAvatars/" + cast.Id + ".jpg";

            try
            {
                await _castService.UpdateAsync(castid, cast);

                // nếu có hình ảnh thì update lại cast và lưu vào file
                if (model.AvatarFile != null)
                {
                    var file = Path.Combine(_environment.WebRootPath, "src/img/CastAvatars", castid + ".jpg");

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
        public async Task<IActionResult> Delete(string castid)
        {
            try
            {
                await _castService.DeleteAsync(castid);
                
                var file = Path.Combine(_environment.WebRootPath, "src\\img\\CastAvatars", castid + ".jpg");

                if (System.IO.File.Exists(file)) System.IO.File.Delete(file);
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

