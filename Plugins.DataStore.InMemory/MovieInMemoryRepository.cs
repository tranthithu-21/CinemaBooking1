using CoreBusiness;
using UseCases.DataStorePluginInterfaces;

namespace Plugins.DataStore.InMemory
{
    public class MovieInMemoryRepository : IMovieRepository
    {
        private readonly List<Movie> _movies = new()
        {
            new Movie
            {
                MovieId = 1,
                Title = "Dune: Part Two",
                Genre = "Sci-Fi / Adventure",
                DurationMinutes = 166,
                Description = "Paul Atreides unites with Chani and the Fremen while seeking revenge against the conspirators who destroyed his family.",
                PosterUrl = "https://images.unsplash.com/photo-1534447677768-be436bb09401?w=600&auto=format&fit=crop&q=80",
                ReleaseDate = DateTime.Now.AddDays(-10)
            },
            new Movie
            {
                MovieId = 2,
                Title = "Kung Fu Panda 4",
                Genre = "Animation / Action",
                DurationMinutes = 94,
                Description = "Po must train a new Dragon Warrior while facing a wicked sorceress who can shapeshift.",
                PosterUrl = "https://images.unsplash.com/photo-1489599849927-2ee91cede3ba?w=600&auto=format&fit=crop&q=80",
                ReleaseDate = DateTime.Now.AddDays(-5)
            },
            new Movie
            {
                MovieId = 3,
                Title = "Godzilla x Kong",
                Genre = "Action / Sci-Fi",
                DurationMinutes = 115,
                Description = "Two ancient titans, Godzilla and Kong, clash in an epic battle as humans unravel their origins.",
                PosterUrl = "https://images.unsplash.com/photo-1518676590629-3dcbd9c5a5c9?w=600&auto=format&fit=crop&q=80",
                ReleaseDate = DateTime.Now
            }
        };

        public Task<IEnumerable<Movie>> GetAllMoviesAsync()
        {
            return Task.FromResult<IEnumerable<Movie>>(_movies);
        }

        public Task<Movie?> GetMovieByIdAsync(int movieId)
        {
            var movie = _movies.FirstOrDefault(m => m.MovieId == movieId);
            return Task.FromResult(movie);
        }
    }
}
