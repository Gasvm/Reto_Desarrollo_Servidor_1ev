using Reto_Desarrollo_Servidor_1ev.Models;

namespace Reto_Desarrollo_Servidor_1ev.Repositories
{
    public interface ITarjetaCreditoRepository
    {
        Task<List<TarjetaCredito>> GetAllAsync(
                QueryParamsFilters? filtroIdClienteTarjeta = null, 
                QueryParamsFilters? filtroDescripcionTarjeta = null, 
                QueryParamsFilters? filtroNumeroTarjeta = null,
                QueryParamsFilters? filtroFechaCaducidadDesde = null,
                QueryParamsFilters? filtroFechaCaducidadHasta = null,
                QueryParamsFilters? filtroEstadoActivo = true //¿Debería ser null por defecto?
                );
        Task<TarjetaCredito?> GetByIdAsync(int id);
        Task AddAsync(TarjetaCredito tarjetaCredito);
        Task UpdateAsync(TarjetaCredito tarjetaCredito);
        Task DeleteAsync(int id);
    }
}