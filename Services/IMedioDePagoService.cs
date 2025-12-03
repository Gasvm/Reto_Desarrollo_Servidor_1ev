using Reto_Desarrollo_Servidor_1ev.Models;

namespace Reto_Desarrollo_Servidor_1ev.Services
{
    public interface IMedioDePagoService
    {
        Task<List<MedioDePago>> GetAllAsync(QueryParamsFilters? filters);
        Task<MedioDePago?> GetByIdAsync(int id);
        Task AddAsync(MedioDePago medioDePago);
        Task UpdateAsync(MedioDePago medioDePago);
        Task DeleteAsync(int id);
    }
}
