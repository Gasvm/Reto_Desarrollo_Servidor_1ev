using Reto_Desarrollo_Servidor_1ev.Models;

namespace Reto_Desarrollo_Servidor_1ev.Repositories
{
    public interface IProductoRepository
    {
        Task<List<Producto>> GetAllAsync(
                QueryParamsFilters? filtroDescripcionProducto = null, 
                QueryParamsFilters? filtroPrecioMinimo = null, 
                QueryParamsFilters? filtroPrecioMaximo = null,
                QueryParamsFilters? filtroIdTipoIVA = null,
                QueryParamsFilters? filtroEstadoActivo = null
                );
        Task<Producto?> GetByIdAsync(int id);
        Task AddAsync(Producto producto);
        Task UpdateAsync(Producto producto);
        Task DeleteAsync(int id);
    }
}