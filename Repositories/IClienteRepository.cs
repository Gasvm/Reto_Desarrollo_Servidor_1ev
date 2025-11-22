using Models;

namespace Reto_Desarrollo_Servidor_1ev.Repositories
{
    public interface IClienteRepository
    {
        Task<List<Cliente>> GetAllAsync(QueryParamsFilters? filtroNombreCliente = null, QueryParamsFilters? filtroEstadoActivo = true);
        Task<Cliente?> GetByIdAsync(int id);
        Task AddAsync(Cliente cliente);
        Task UpdateAsync(Cliente cliente);
        Task DeleteAsync(int id);
    }
}