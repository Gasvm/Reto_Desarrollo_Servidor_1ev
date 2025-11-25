using Reto_Desarrollo_Servidor_1ev.Models;

namespace Reto_Desarrollo_Servidor_1ev.Repositories
{
    public interface IPedidoLinRepository
    {
        Task<List<PedidoLin>> GetAllAsync(
                QueryParamsFilters? filtroIdPedido = null, 
                QueryParamsFilters? filtroIdProducto = null, 
                QueryParamsFilters? filtroEstadoActivo = true
                );
        Task<PedidoLin?> GetByIdAsync(int id);
        Task AddAsync(PedidoLin pedidoLin);
        Task UpdateAsync(PedidoLin pedidoLin);
        Task DeleteAsync(int id);
    }
}