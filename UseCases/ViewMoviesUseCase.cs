using CoreBusiness;
using UseCases.DataStorePluginInterfaces;

namespace UseCases
{
    public class ViewMoviesUseCase
    {
        private readonly IMovieRepository _movieRepository;

        public ViewMoviesUseCase(IMovieRepository movieRepository)
        {
            _movieRepository = movieRepository;
        }

        public async Task<IEnumerable<Movie>> ExecuteAsync()
        {
            return await _movieRepository.GetAllMoviesAsync();
        }
    }
}
