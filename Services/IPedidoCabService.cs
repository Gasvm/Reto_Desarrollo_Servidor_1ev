using Reto_Desarrollo_Servidor_1ev.Models;

namespace Reto_Desarrollo_Servidor_1ev.Services
{
    public interface IPedidoCabService
    {
        Task<List<PedidoCab>> GetAllAsync(QueryParamsFilters? filters);
        Task<PedidoCab?> GetByIdAsync(int id);
        Task AddAsync(PedidoCab pedidoCab);
        Task UpdateAsync(PedidoCab pedidoCab);
        Task DeleteAsync(int id);
    }
}
