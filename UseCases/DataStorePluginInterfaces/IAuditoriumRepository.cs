using CoreBusiness;

namespace UseCases.DataStorePluginInterfaces
{
    public interface IAuditoriumRepository
    {
        Task<IEnumerable<Auditorium>> GetAllAuditoriumsAsync();
        Task<Auditorium?> GetAuditoriumByIdAsync(int auditoriumId);
    }
}
