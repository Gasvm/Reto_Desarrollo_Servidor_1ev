using Reto_Desarrollo_Servidor_1ev.Models;

namespace Reto_Desarrollo_Servidor_1ev.Repositories
{
    public interface IPedidoCabRepository
    {
        Task<List<PedidoCab>> GetAllAsync(
                QueryParamsFilters? filtroIdCliente = null, 
                QueryParamsFilters? filtroIdMedioPago = null, 
                QueryParamsFilters? filtroFechaPedidoDesde = null, 
                QueryParamsFilters? filtroFechaPedidoHasta = null, 
                QueryParamsFilters? filtroEstadoActivo = null
                );
        Task<PedidoCab?> GetByIdAsync(int id);
        Task AddAsync(PedidoCab pedidoCab);
        Task UpdateAsync(PedidoCab pedidoCab);
        Task DeleteAsync(int id);
    }
}