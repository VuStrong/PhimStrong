using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PhimStrong.Application.Interfaces;
using PhimStrong.Areas.Admin.Models.Movie;
using PhimStrong.Domain.Models;
using PhimStrong.Domain.PagingModel;
using PhimStrong.Models.Movie;
using SharedLibrary.Constants;
using System.Linq.Expressions;

namespace PhimStrong.Areas.Admin.Controllers
{
    [Area("Admin")]
	[Authorize(Roles = $"{RoleConstant.ADMIN}, {RoleConstant.THUY_TO}")]
    public class MovieController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IMovieService _movieService;
        private readonly ICountryService _countryService;
        private readonly IWebHostEnvironment _environment;

        public MovieController(
            IMapper mapper,
            IMovieService movieService,
            ICountryService countryService,
            IWebHostEnvironment environment)
        {
            _mapper = mapper;
            _environment = environment;
            _movieService = movieService;
            _countryService = countryService;
        }

        private const int MOVIES_PER_PAGE = 15;

        [HttpGet]
        public async Task<IActionResult> Index(int page, string? value = null)
        {
            PagedList<Movie> movies = await _movieService.SearchAsync(value, new PagingParameter(page, MOVIES_PER_PAGE));

            if (value != null) ViewData["value"] = value;
            
            return View(_mapper.Map<PagedList<MovieViewModel>>(movies));
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var countries = await _countryService.GetAllAsync();
            ViewData["countries"] = new SelectList(countries, "Name", "Name");

            return View(new CreateMovieViewModel());
        }

        [HttpPost]
        public async Task<JsonResult> Create(CreateMovieViewModel model)
        {
            Movie movie = _mapper.Map<Movie>(model);
            try
            {
                await _movieService.CreateAsync(movie, model.Casts, model.Directors, model.Categories, 
                    model.Country, model.Tags, model.Videos);

                // nếu có hình ảnh thì update lại movie và lưu vào file
                if (model.ImageFile != null)
                {
                    movie.Image = "/src/img/MovieImages/" + movie.Id + ".jpg";
                    await _movieService.UpdateAsync(movie.Id, movie);

                    var file = Path.Combine(_environment.ContentRootPath, "wwwroot/src/img/MovieImages", movie.Id + ".jpg");
                    using FileStream fileStream = new(file, FileMode.Create);
                    await model.ImageFile.CopyToAsync(fileStream);
                }

            }
            catch(Exception e)
            {
                return Json(new { success = false, error = e.Message } );
            }

            TempData["success"] = $"Đã thêm phim {model.Name}.";
            return Json(new { success = true });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string movieid)
        {
            var movie = await _movieService.GetByIdAsync(
                movieid,
                new Expression<Func<Movie, object?>>[]
                {
                    m => m.Casts,
                    m => m.Categories,
                    m => m.Country,
                    m => m.Directors,
                    m => m.Tags,
                    m => m.Videos
                });

            if (movie == null)
            {
                return NotFound("Không tìm thấy phim.");
            }

            var countries = await _countryService.GetAllAsync();
            ViewData["countries"] = new SelectList(countries, "Name", "Name");

            return View(EditMovieViewModel.FromMovie(movie));
        }

        [HttpPost]
        public async Task<JsonResult> Edit(string movieid, EditMovieViewModel model)
        {
            if (model == null)
            {
                return Json(new { success = false, error = "Lỗi, không tìm thấy model :(" });
            }

            Movie movie = _mapper.Map<Movie>(model);

            if (model.ImageFile != null)
                movie.Image = "/src/img/MovieImages/" + movieid + ".jpg";

            try
            {
                await _movieService.UpdateAsync(movieid, movie, model.Casts, model.Directors,
                    model.Categories, model.Country, model.Tags, model.Videos);

                if (model.ImageFile != null)
                {
                    var file = Path.Combine(_environment.ContentRootPath, "wwwroot/src/img/MovieImages", movieid + ".jpg");

                    using var fileStream = new FileStream(file, FileMode.Create);
                    await model.ImageFile.CopyToAsync(fileStream);
                }
            }
            catch (Exception e)
            {
                return Json(new { success = false, error = e.Message });
            }

            TempData["success"] = "Chỉnh sửa thành công";
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string movieid)
        {
            try
            {
                await _movieService.DeleteAsync(movieid);

                var file = Path.Combine(_environment.WebRootPath, "src\\img\\MovieImages", movieid + ".jpg");

                if (System.IO.File.Exists(file)) System.IO.File.Delete(file);
            }
            catch (Exception e)
            {
                TempData["status"] = "Lỗi, " + e.Message;
                return RedirectToAction("Edit", new { movieid = movieid });
            }

            TempData["success"] = "Xóa thành công";
            return RedirectToAction("Index");
        }
    }
}
