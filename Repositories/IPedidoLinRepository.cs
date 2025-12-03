using Reto_Desarrollo_Servidor_1ev.Models;

namespace Reto_Desarrollo_Servidor_1ev.Repositories
{
    public interface IPedidoLinRepository
    {
        Task<List<PedidoLin>> GetAllAsync(QueryParamsFilters? filters);
        Task<PedidoLin?> GetByIdAsync(int id);
        Task AddAsync(PedidoLin pedidoLin);
        Task UpdateAsync(PedidoLin pedidoLin);
        Task DeleteAsync(int id);
    }
}