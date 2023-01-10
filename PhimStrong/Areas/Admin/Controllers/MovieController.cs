using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhimStrong.Areas.Admin.Models;
using PhimStrong.Data;
using SharedLibrary.Constants;
using SharedLibrary.Models;
using System.Text.RegularExpressions;

namespace PhimStrong.Areas.Admin.Controllers
{
    [Area("Admin")]
	[Authorize(Roles = $"{RoleConstant.ADMIN}, {RoleConstant.THUY_TO}")]
    public class MovieController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _environment;

        public MovieController(AppDbContext db, IWebHostEnvironment environment)
        {
            _db = db;
            _environment = environment;
        }

        private const int MOVIES_PER_PAGE = 15;

        [HttpGet]
        public IActionResult Index(int page, string? filter = null)
        {
            if (page <= 0) page = 1;

            int numberOfPages = 0;

            List<Movie> movies = new List<Movie>();
            if (filter == null || filter.Trim() == "")
            {
                numberOfPages = (int)Math.Ceiling((double)_db.Movies.Count() / MOVIES_PER_PAGE);

                if (page > numberOfPages) page = numberOfPages;
                if (page <= 0) page = 1;

                movies = _db.Movies.Skip((page - 1) * MOVIES_PER_PAGE).Take(MOVIES_PER_PAGE).ToList();
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
                            movies = _db.Movies.Where(m =>
                                (m.Name ?? "").ToLower().Contains(filterValue.ToLower())
                            ).ToList();

                            TempData["FilterMessage"] = "tên là " + filterValue;

                            break;
                        default:
                            break;
                    }
                }

                numberOfPages = (int)Math.Ceiling((double)movies.Count / MOVIES_PER_PAGE);
                if (page > numberOfPages) page = numberOfPages;
                if (page <= 0) page = 1;

                movies = movies.Skip((page - 1) * MOVIES_PER_PAGE).Take(MOVIES_PER_PAGE).ToList();
            }

            TempData["NumberOfPages"] = numberOfPages;
            TempData["CurrentPage"] = page;
            TempData["filter"] = filter;

            return View(movies);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new MovieModel());
        }

        [HttpPost]
        public async Task<IActionResult> Create(MovieModel model)
        {
            if (model == null)
            {
                TempData["status"] = "Lỗi, không có phim được chọn.";
                return View(model);
            }

            var movie = new Movie();
            movie.Name = model.Name;
            movie.Description = model.Description;
            movie.ReleaseDate = model.ReleaseDate;
            movie.Length = model.Length;
            movie.Type = model.Type;

            movie.EpisodeCount = movie.Type == "Phim lẻ" ? 1 : model.EpisodeCount;

            if (model.Categories != null)
            {
                foreach (string cate in model.Categories)
                {

                    Category? category = _db.Categories.FirstOrDefault(c => c.Name == cate);

                    if (category == null) continue;

                    if (movie.Categories == null) movie.Categories = new List<Category>();

                    movie.Categories.Add(category);
                }
            }

            if (model.Casts != null)
            {
                foreach (string castName in model.Casts)
                {

                    Cast? cast = _db.Casts.FirstOrDefault(c => c.Name == castName);

                    if (cast == null) continue;

                    if (movie.Casts == null) movie.Casts = new List<Cast>();

                    movie.Casts.Add(cast);
                }
            }

            if (model.Directors != null)
            {
                foreach (string directorName in model.Directors)
                {

                    Director? director = _db.Directors.FirstOrDefault(d => d.Name == directorName);

                    if (director == null) continue;

                    if (movie.Directors == null) movie.Directors = new List<Director>();

                    movie.Directors.Add(director);
                }
            }

            if (!String.IsNullOrEmpty(model.Country))
            {
                Country? country = _db.Countries.FirstOrDefault(c => c.Name == model.Country);

                if (country != null)
                {
                    movie.Country = country;
                }
            }
            
            try
            {
                _db.Movies.Add(movie);
                await _db.SaveChangesAsync();

                if (model.ImageFile != null)
                {
                    var file = Path.Combine(_environment.ContentRootPath, "wwwroot/src/img/MovieImages", movie.Id.ToString() + ".jpg");
                    using (FileStream fileStream = new FileStream(file, FileMode.Create))
                    {
                        await model.ImageFile.CopyToAsync(fileStream);
                    }

                    if (movie.Image != "/src/img/MovieImages/" + movie.Id.ToString() + ".jpg")
                        movie.Image = "/src/img/MovieImages/" + movie.Id.ToString() + ".jpg";

                    await _db.SaveChangesAsync();
                }
            }
            catch(Exception e)
            {
                TempData["status"] = "Lỗi" + e.Message;
                return View(model);
            }

            foreach (var c in _db.Countries)
            {
                Console.Write(c.Name + " : ");
                if (c.Movies != null)
                {
                    Console.Write(c.Movies[0].Name);
                }
                Console.WriteLine();
            }

            TempData["success"] = $"Đã thêm phim {model.Name}.";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(int movieid)
        {
            var movie = _db.Movies.FirstOrDefault(m => m.Id == movieid);

            if (movie == null)
            {
                return NotFound("Không tìm thấy phim.");
            }

            return View(GetMovieModel(movie));
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int movieid, MovieModel model)
        {
            if (model == null)
            {
                return NotFound("Không tìm thấy phim.");
            }

            var movieToEdit = _db.Movies.FirstOrDefault(c => c.Id == movieid);

            if (movieToEdit == null)
            {
                return NotFound("Không tìm thấy phim.");
            }

            if (model.Name != movieToEdit.Name)
                movieToEdit.Name = Regex.Replace(model.Name.ToLower(), @"(^\w)|(\s\w)", m => m.Value.ToUpper());
            if (model.ImageFile != null)
            {
                var file = Path.Combine(_environment.ContentRootPath, "wwwroot/src/img/MovieImages", movieToEdit.Id.ToString() + ".jpg");

                using (var fileStream = new FileStream(file, FileMode.Create))
                {
                    await model.ImageFile.CopyToAsync(fileStream);
                }

                if (movieToEdit.Image != "/src/img/MovieImages/" + movieToEdit.Id.ToString() + ".jpg")
                    movieToEdit.Image = "/src/img/MovieImages/" + movieToEdit.Id.ToString() + ".jpg";
            }

            try
            {
                _db.Movies.Update(movieToEdit);
                await _db.SaveChangesAsync();
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
        public async Task<IActionResult> Delete(int movieid)
        {
            var movie = _db.Movies.FirstOrDefault(m => m.Id == movieid);

            if (movie == null)
            {
                return NotFound("Không tìm thấy phim.");
            }

            try
            {
                var file = Path.Combine(_environment.ContentRootPath, "wwwroot/src/img/MovieImages", movie.Id.ToString() + ".jpg");

                FileInfo fileInfo = new FileInfo(file);
                fileInfo.Delete();

                _db.Movies.Remove(movie);
                await _db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                TempData["status"] = "Lỗi, " + e.Message;
                return RedirectToAction("Edit", new { movieid = movieid });
            }

            TempData["success"] = "Xóa thành công";
            return RedirectToAction("Index");
        }

        private MovieModel GetMovieModel(Movie movie)
        {
            MovieModel model = new MovieModel();

            model.Id = movie.Id;
            model.Name = movie.Name;
            model.Description = movie.Description;
            model.ReleaseDate = movie.ReleaseDate;
            model.Length = movie.Length;
            model.View = movie.View;
            model.Type = movie.Type;
            model.EpisodeCount = movie.EpisodeCount;

            if (movie.Trailer != null)
            {
                model.Trailer = movie.Trailer.Clip;
            }

            if (movie.Categories != null)
            {
                List<string> cateList = new List<string>();

                movie.Categories.ForEach((c) =>
                {
                    cateList.Add(c.Name);
                });
             
                model.Categories = cateList.ToArray();
            }

            if (movie.Casts != null)
            {
                List<string> castList = new List<string>();

                movie.Casts.ForEach((c) =>
                {
                    castList.Add(c.Name);
                });

                model.Casts = castList.ToArray();
            }

            if (movie.Directors != null)
            {
                List<string> directorList = new List<string>();
                
                movie.Directors.ForEach((d) =>
                {
                    directorList.Add(d.Name);
                });

                model.Directors = directorList.ToArray();
            }

            if (movie.Country != null)
            {
                model.Country = movie.Country.Name;
            }

            return model;
        }
    }
}
