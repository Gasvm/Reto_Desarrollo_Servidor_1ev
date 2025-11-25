using Reto_Desarrollo_Servidor_1ev.Models;

namespace Reto_Desarrollo_Servidor_1ev.Services
{
    public interface IPedidoLinService
    {
        Task<List<PedidoLin>> GetAllAsync(QueryParamsFilters? filtroIdPedido = null, QueryParamsFilters? filtroIdProducto = null, QueryParamsFilters? filtroEstadoActivo = null);
        Task<PedidoLin?> GetByIdAsync(int id);
        Task AddAsync(PedidoLin pedidoLin);
        Task UpdateAsync(PedidoLin pedidoLin);
        Task DeleteAsync(int id);
    }
}
