using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Watchlist.Contracts;
using Watchlist.Data;
using Watchlist.Data.Models;
using Watchlist.Models;

namespace Watchlist.Services
{
    public class MovieService : IMovieService
    {
        private readonly WatchlistDbContext context;

        public MovieService(WatchlistDbContext _context)
        {
            this.context = _context;
        }

        public async Task AddMovieAsync(AddMovieViewModel model)
        {
            var entity = new Movie()
            {

                GenreId = model.GenreId,
                ImageUrl = model.ImageUrl,
                Rating = model.Rating,
                Director = model.Director,
                Title = model.Title
            };

            await context.Movies.AddAsync(entity);
            await context.SaveChangesAsync();
        }

        public async Task AddMovieToCollectionAsyc(int movieId, string userId)
        {
            var user = await context.Users
                .Where(u => u.Id == userId)
                .Include(u => u.UsersMovies)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                throw new ArgumentException("Invalid user Id");
            }

            var movie = await context.Movies.FirstOrDefaultAsync(m => m.Id == movieId);

            if (movie == null)
            {
                throw new ArgumentException("Invalid movie Id");
            }

            if (user.UsersMovies.Any(um =>um.MovieId == movieId))
            {
                return;
            }
            
            var curMovie = new UserMovie()
            {
                MovieId = movie.Id,
                UserId = user.Id,
                Movie = movie,
                User = user
            };
                       
            user.UsersMovies.Add(curMovie);
            await context.SaveChangesAsync();
        }

        public async Task<IEnumerable<MovieViewModel>> GetAllAsync()
        {
            return await context.Movies
                .Select(m => new MovieViewModel()
                {
                    Id = m.Id,
                    Title = m.Title,
                    Director = m.Director,
                    Genre = m.Genre.Name, 
                    ImageUrl = m.ImageUrl,
                    Rating = m.Rating,

                }).ToListAsync();
        }

        public async Task<IEnumerable<Genre>> GetGenresAsync()
        {
            return await context.Genres.ToListAsync();
        }

        public async Task<IEnumerable<MovieViewModel>> GetWatchedByUserAsync(string userId)
        {
            var user = await context.Users
                .Where(u => u.Id == userId)
                .Include(u => u.UsersMovies)
                .ThenInclude(um => um.Movie)
                .ThenInclude(m => m.Genre)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                throw new ArgumentException("Ivalid User ID");
            }

            return user.UsersMovies.Select(m => new MovieViewModel()
            {
                Director = m.Movie.Director,
                Genre = m.Movie.Genre.Name,
                Id = m.MovieId,
                ImageUrl = m.Movie.ImageUrl,
                Rating = m.Movie.Rating,
                Title = m.Movie.Title
            });
        }

        public async Task RemoveMovieFromCollectionAsyc(int movieId, string userId)
        {
            var user = await context.Users
                .Where(u => u.Id == userId)
                .Include(u => u.UsersMovies)
                .ThenInclude(um => um.Movie)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                throw new ArgumentException("Invalid user ID");
            }

            var movie = user.UsersMovies.FirstOrDefault(m => m.MovieId == movieId);

            if (movie != null)
            {
                user.UsersMovies.Remove(movie);
                await context.SaveChangesAsync();
            }
           
        }
    }
}
