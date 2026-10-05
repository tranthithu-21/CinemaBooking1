using CoreBusiness;

namespace UseCases.DataStorePluginInterfaces
{
    public interface IMovieRepository
    {
        Task<IEnumerable<Movie>> GetAllMoviesAsync();
        Task<Movie?> GetMovieByIdAsync(int movieId);
    }
}
