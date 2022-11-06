using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration.UserSecrets;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;
using Watchlist.Contracts;
using Watchlist.Models;

namespace Watchlist.Controllers
{
    [Authorize]
    public class MoviesController : Controller
    {
        private readonly IMovieService movieService;

        public MoviesController(IMovieService movieService)
        {
            this.movieService = movieService;
        }

        [HttpGet]
        public async Task<IActionResult> All()
        {
            var model = await movieService.GetAllAsync();

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var model = new AddMovieViewModel()
            {
                Genres = await movieService.GetGenresAsync()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddMovieViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                await movieService.AddMovieAsync(model);

                return RedirectToAction(nameof(All));
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Something went terribly wrong my friend...");

                return View(model);
            }

        }

        [HttpPost]
        public async Task<IActionResult> AddToCollection(int movieId)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                await movieService.AddMovieToCollectionAsyc(movieId, userId);
            }
            catch (Exception)
            {

                throw;
            }
            

            return RedirectToAction(nameof(All));
        }

        public async Task<IActionResult> Watched()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var model = await movieService.GetWatchedByUserAsync(userId);

            return View("Watched", model);
        }


        public async Task<IActionResult> RemoveFromCollection(int movieId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            await movieService.RemoveMovieFromCollectionAsyc(movieId, userId);

            return RedirectToAction(nameof(Watched));
        }
    }
}
