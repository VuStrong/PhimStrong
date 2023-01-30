using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhimStrong.Areas.Admin.Models;
using PhimStrong.Data;
using SharedLibrary.Constants;
using SharedLibrary.Helpers;
using SharedLibrary.Models;
using System.Net;
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
            int count = 0; // total of search result
            if (filter == null || filter.Trim() == "")
            {
                count = _db.Movies.Count();
				numberOfPages = (int)Math.Ceiling((double)count / MOVIES_PER_PAGE);
                TempData["TotalCount"] = count;

				if (page > numberOfPages) page = numberOfPages;
                if (page <= 0) page = 1;

                movies = _db.Movies.OrderByDescending(m => m.CreatedDate)
                    .Skip((page - 1) * MOVIES_PER_PAGE).Take(MOVIES_PER_PAGE).ToList();
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

                            movies = _db.Movies.Where(m =>
                                (m.NormalizeTranslateName ?? "").Contains(filterValue) ||
                                (m.NormalizeName ?? "").Contains(filterValue)
                            ).OrderByDescending(m => m.CreatedDate).ToList();

							break;
                        default:
                            break;
                    }
                }

				count = movies.Count;
				TempData["TotalCount"] = count;

				numberOfPages = (int)Math.Ceiling((double)count / MOVIES_PER_PAGE);
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
        public async Task<JsonResult> Create(MovieModel model)
        {
            if (model == null)
            {
                return Json(new { success = false, error = "Lỗi, không tìm thấy model :(" });
            }

            Movie movie = new()
            {
                Name = model.Name.NormalizeString(),
                TranslateName = model.TranslateName.NormalizeString(),
				Description = model.Description,
                ReleaseDate = model.ReleaseDate,
                CreatedDate = DateTime.Now,
                ShowInSlide = false,
                Length = model.Length == null ? 0 : model.Length,
                Type = model.Type,
                EpisodeCount = model.EpisodeCount,
                Rating = model.Rating,
                Status = model.Status,
                Trailer = model.Trailer
            };

			movie.NormalizeName = movie.Name.RemoveMarks();
            movie.NormalizeTranslateName = movie.TranslateName.RemoveMarks();

            if (model.Categories != null && model.Categories.Length > 0)
            {
                model.Categories = model.Categories[0].Split(",");
                foreach (string cate in model.Categories)
                {
                    Category? category = _db.Categories.FirstOrDefault(c => c.Name == cate);

                    if (category == null) continue;

                    movie.Categories ??= new List<Category>();

                    movie.Categories.Add(category);
                }
            }

            if (model.Casts != null && model.Casts.Length > 0)
            {
                model.Casts = model.Casts[0].Split(",");
                foreach (string castName in model.Casts)
                {

                    Cast? cast = _db.Casts.FirstOrDefault(c => c.Name == castName);

                    if (cast == null) continue;

                    movie.Casts ??= new List<Cast>();

                    movie.Casts.Add(cast);
                }
            }

            if (model.Directors != null && model.Directors.Length > 0)
            {
                model.Directors = model.Directors[0].Split(",");
                foreach (string directorName in model.Directors)
                {

                    Director? director = _db.Directors.FirstOrDefault(d => d.Name == directorName);

                    if (director == null) continue;

                    movie.Directors ??= new List<Director>();

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

            // videos
            if (model.Videos != null && model.Videos.Length > 0 )
            {
                int index = 0;
                if(!String.IsNullOrEmpty(model.Videos[0])) 
                    model.Videos = model.Videos[0].Split(",");
                
                foreach (var c in model.Videos)
                {
                    index += 1;
                    if (String.IsNullOrEmpty(c)) continue;

                    Video video = new()
                    {
                        VideoUrl = c,
                        Episode = index,
                        Movie = movie
                    };

                    movie.Videos ??= new List<Video>();
                    movie.Videos.Add(video);
                }
            }

            // tags
            if (model.Tags != null)
            {
                string[] tagNames = new string[0];
                if (!String.IsNullOrEmpty(model.Tags) && !String.IsNullOrWhiteSpace(model.Tags))
                    tagNames = new Regex(@", |,").Split(model.Tags);

                foreach (var tagName in tagNames)
                {
                    if (String.IsNullOrEmpty(tagName) || String.IsNullOrWhiteSpace(tagName)) continue;

                    Tag tag = new()
                    {
                        TagName = tagName.Trim(),
                        Movie = movie
                    };

                    movie.Tags ??= new List<Tag>();
                    movie.Tags.Add(tag);
                }
            }

            // use transaction to add movie :
            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                _db.Movies.Add(movie);
                await _db.SaveChangesAsync();

                // tạo movie image nếu có
                if (model.ImageFile != null)
                {
                    if (movie.Image != "/src/img/MovieImages/" + movie.Id.ToString() + ".jpg")
                        movie.Image = "/src/img/MovieImages/" + movie.Id.ToString() + ".jpg";
                }

                await _db.SaveChangesAsync();

                // nếu đến đc đây thì lưu ảnh vào file
                if (model.ImageFile != null)
                {
                    var file = Path.Combine(_environment.ContentRootPath, "wwwroot/src/img/MovieImages", movie.Id.ToString() + ".jpg");
                    using (FileStream fileStream = new(file, FileMode.Create))
                    {
                        await model.ImageFile.CopyToAsync(fileStream);
                    }
                }

                // commit
                await transaction.CommitAsync();
            }
            catch(Exception e)
            {
                // if error discard all change in database
                await transaction.RollbackAsync();
                return Json(new { success = false, error = e.Message } );
            }

            TempData["success"] = $"Đã thêm phim {model.Name}.";
            return Json(new { success = true });
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
        public async Task<JsonResult> Edit(int movieid, MovieModel model)
        {
            if (model == null)
            {
                return Json(new { success = false, error = "Lỗi, không tìm thấy model :(" });
            }

            var movieToEdit = _db.Movies.FirstOrDefault(c => c.Id == movieid);

            if (movieToEdit == null)
            {
                return Json(new { success = false, error = "Lỗi, không tìm thấy phim :(" });
            }

            if (model.Name != movieToEdit.Name)
            {
                movieToEdit.Name = model.Name.NormalizeString();
                movieToEdit.NormalizeName = movieToEdit.Name.RemoveMarks();
            }
			if (model.TranslateName != movieToEdit.TranslateName)
			{
				movieToEdit.TranslateName = model.TranslateName.NormalizeString();
				movieToEdit.NormalizeTranslateName = movieToEdit.TranslateName.RemoveMarks();
			}
			if (model.Description != movieToEdit.Description)
                movieToEdit.Description = model.Description;
            if (model.ReleaseDate != movieToEdit.ReleaseDate)
                movieToEdit.ReleaseDate = model.ReleaseDate;
            if (model.Length != movieToEdit.Length)
                movieToEdit.Length = model.Length;
            if (model.Trailer != movieToEdit.Trailer)
                movieToEdit.Trailer = model.Trailer;

            if (model.Rating != movieToEdit.Rating)
                movieToEdit.Rating = model.Rating;
            if (model.Type != movieToEdit.Type) 
                movieToEdit.Type = model.Type;
			if (model.Status != movieToEdit.Status)
				movieToEdit.Status = model.Status;
			if (model.EpisodeCount != movieToEdit.EpisodeCount)
                movieToEdit.EpisodeCount = model.EpisodeCount;

            // Edit country
            if (movieToEdit.Country != null)
            {
                if (!String.IsNullOrEmpty(model.Country) && model.Country != movieToEdit.Country.Name)
                {
                    Country? country = _db.Countries.FirstOrDefault(c => c.Name == model.Country);

                    if (country != null)
                    {
                        movieToEdit.Country = country;
                    }
                }
            }
            else
            {
                if (!String.IsNullOrEmpty(model.Country))
                {
                    Country? country = _db.Countries.FirstOrDefault(c => c.Name == model.Country);

                    if (country != null)
                    {
                        movieToEdit.Country = country;
                    }
                }
            }

			// Edit Category
			if (movieToEdit.Categories != null)
            {
                movieToEdit.Categories = null;
            }

            if (model.Categories != null && model.Categories.Length > 0)
            {
                model.Categories = model.Categories[0].Split(",");
                foreach (string cate in model.Categories)
                {
                    Category? category = _db.Categories.FirstOrDefault(c => c.Name == cate);

                    if (category == null) continue;

                    movieToEdit.Categories ??= new List<Category>();

                    movieToEdit.Categories.Add(category);
                }
            }

            // Edit Cast
            if (movieToEdit.Casts != null)
            {
                movieToEdit.Casts = null;
            }

            if (model.Casts != null && model.Casts.Length > 0)
            {
                model.Casts = model.Casts[0].Split(",");
                foreach (string castName in model.Casts)
                {
                    Cast? cast = _db.Casts.FirstOrDefault(c => c.Name == castName);

                    if (cast == null) continue;

                    movieToEdit.Casts ??= new List<Cast>();

                    movieToEdit.Casts.Add(cast);
                }
            }

			// Edit Director
			if (movieToEdit.Directors != null)
			{
				movieToEdit.Directors = null;
			}

			if (model.Directors != null && model.Directors.Length > 0)
			{
				model.Directors = model.Directors[0].Split(",");
				foreach (string directorName in model.Directors)
				{
					Director? director = _db.Directors.FirstOrDefault(d => d.Name == directorName);

					if (director == null) continue;

					movieToEdit.Directors ??= new List<Director>();

					movieToEdit.Directors.Add(director);
				}
			}

            // Edit video
            if (model.Videos != null && model.Videos.Length > 0)
            {
                int index = 0;
                if (!String.IsNullOrEmpty(model.Videos[0]))
                    model.Videos = model.Videos[0].Split(",");

                if (movieToEdit.Videos != null) movieToEdit.Videos = null;

                foreach (var c in model.Videos)
                {
                    index += 1;
                    if (String.IsNullOrEmpty(c)) continue;

                    Video video = new()
                    {
                        VideoUrl = c,
                        Episode = index,
                        Movie = movieToEdit
                    };

                    movieToEdit.Videos ??= new List<Video>();
                    movieToEdit.Videos.Add(video);
                }
            }

            // Edit tag
            if (model.Tags != null)
            {
                string[] tagNames = new string[0];
                if (!String.IsNullOrEmpty(model.Tags) && !String.IsNullOrWhiteSpace(model.Tags))
                    tagNames = new Regex(@", |,").Split(model.Tags);

                if (movieToEdit.Tags != null) movieToEdit.Tags = null;

                foreach (string tagName in tagNames)
                {
                    if (String.IsNullOrEmpty(tagName) || String.IsNullOrWhiteSpace(tagName)) continue;

                    Tag tag = new()
                    {
                        TagName = tagName,
                        Movie = movieToEdit
                    };

                    movieToEdit.Tags ??= new List<Tag>();
                    movieToEdit.Tags.Add(tag);
                }
            }

            // Edit Image
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

            // Update edit date
            movieToEdit.CreatedDate = DateTime.Now;

            try
            {
                _db.Movies.Update(movieToEdit);
                await _db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return Json(new { success = false, error = e.Message });
            }

            TempData["success"] = "Chỉnh sửa thành công";
            return Json(new { success = true });
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

                FileInfo fileInfo = new(file);
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
            MovieModel model = new()
            {
                Id = movie.Id,
                Name = movie.Name,
                TranslateName = movie.TranslateName,
                Description = movie.Description,
                ReleaseDate = movie.ReleaseDate,
                Length = movie.Length,
                View = movie.View,
                Type = movie.Type,
                EpisodeCount = movie.EpisodeCount,
                Image = movie.Image,
                Rating = movie.Rating,
                Status = movie.Status
            };

			if (movie.Tags != null)
			{
				model.Tags = String.Join(",", movie.Tags.Select(t => t.TagName));
			}

			if (movie.Categories != null)
            {
                List<string> cateList = new();

                movie.Categories.ForEach((c) =>
                {
                    cateList.Add(c.Name);
                });
             
                model.Categories = cateList.ToArray();
            }

            if (movie.Casts != null)
            {
                List<string> castList = new();

                movie.Casts.ForEach((c) =>
                {
                    castList.Add(c.Name);
                });

                model.Casts = castList.ToArray();
            }

            if (movie.Directors != null)
            {
                List<string> directorList = new();
                
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

            if (movie.Trailer != null)
            {
                model.Trailer = movie.Trailer;
            }

            if (movie.Videos != null)
            {
                List<string> videoList = new();
                model.Videos = new string[movie.EpisodeCount];

                for (int i = 0; i < movie.EpisodeCount; i++)
                {
                    Video? video = movie.Videos.FirstOrDefault(m => m.Episode == i + 1);

                    if (video != null)
                        model.Videos[i] = video.VideoUrl ?? "";
                    else
                        model.Videos[i] = "";
                }
            } 

            return model;
        }
    }
}
