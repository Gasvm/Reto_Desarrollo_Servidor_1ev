using Models;

namespace Reto_Desarrollo_Servidor_1ev.Repositories
{
    public interface IMedioDePagoRepository
    {
        Task<List<MedioDePago>> GetAllAsync(
                QueryParamsFilters? filtroDescripcionMedioDePago = null, 
                QueryParamsFilters? filtroEstadoActivo = true //¿Debería ser null por defecto?
            );
        Task<MedioDePago?> GetByIdAsync(int id);
        Task AddAsync(MedioDePago medioDePago);
        Task UpdateAsync(MedioDePago medioDePago);
        Task DeleteAsync(int id);
    }
}