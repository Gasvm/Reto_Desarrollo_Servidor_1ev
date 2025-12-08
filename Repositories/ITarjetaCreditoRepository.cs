using Reto_Desarrollo_Servidor_1ev.Models;

namespace Reto_Desarrollo_Servidor_1ev.Repositories
{
    public interface ITarjetaCreditoRepository
    {
        Task<List<TarjetaCredito>> GetAllAsync(QueryParamsFilters? filters);
        Task<TarjetaCredito?> GetByIdAsync(int id);
        Task<int> AddAsync(TarjetaCredito tarjetaCredito);
        Task UpdateAsync(TarjetaCredito tarjetaCredito);
        Task DeleteAsync(int id);
    }
}