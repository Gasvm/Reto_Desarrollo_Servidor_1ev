using Reto_Desarrollo_Servidor_1ev.Models;

namespace Reto_Desarrollo_Servidor_1ev.Repositories
{
    public interface ITipoIVARepository
    {
        Task<List<TipoIVA>> GetAllAsync(
                QueryParamsFilters? filtroTasaMinima = null, 
                QueryParamsFilters? filtroTasaMaxima = null, 
                QueryParamsFilters? filtroDescripcionTipoIVA = null,
                QueryParamsFilters? filtroFechaCreacionDesde = null,
                QueryParamsFilters? filtroFechaCreacionHasta = null,
                QueryParamsFilters? filtroEstadoActivo = true
                );
        Task<TipoIVA?> GetByIdAsync(int id);
        Task AddAsync(TipoIVA tipoIVA);
        Task UpdateAsync(TipoIVA tipoIVA);
        Task DeleteAsync(int id);
    }
}