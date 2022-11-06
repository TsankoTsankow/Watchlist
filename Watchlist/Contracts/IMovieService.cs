using Watchlist.Data.Models;
using Watchlist.Models;

namespace Watchlist.Contracts
{
    public interface IMovieService
    {
        Task<IEnumerable<MovieViewModel>> GetAllAsync();

        Task<IEnumerable<Genre>> GetGenresAsync();

        Task AddMovieAsync(AddMovieViewModel model);

        Task AddMovieToCollectionAsyc(int movieId, string userId);

        Task<IEnumerable<MovieViewModel>> GetWatchedByUserAsync(string userId);

        Task RemoveMovieFromCollectionAsyc(int movieId, string userId);
    }
}
