using Reto_Desarrollo_Servidor_1ev.Models;

namespace Reto_Desarrollo_Servidor_1ev.Services
{
    public interface ITarjetaCreditoService
    {
        Task<List<TarjetaCredito>> GetAllAsync(QueryParamsFilters? filters);
        Task<TarjetaCredito?> GetByIdAsync(int id);
        Task AddAsync(TarjetaCredito tarjetaCredito);
        Task UpdateAsync(TarjetaCredito tarjetaCredito);
        Task DeleteAsync(int id);
    }
}
