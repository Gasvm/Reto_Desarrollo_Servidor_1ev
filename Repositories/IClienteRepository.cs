using Reto_Desarrollo_Servidor_1ev.Models;

namespace Reto_Desarrollo_Servidor_1ev.Repositories
{
    public interface IClienteRepository
    {
        Task<List<Cliente>> GetAllAsync(QueryParamsFilters? filters);
        Task<Cliente?> GetByIdAsync(int id);
        Task<int> AddAsync(Cliente cliente); //Modificado para devolver el ID del cliente añadido
        Task UpdateAsync(Cliente cliente);
        Task DeleteAsync(int id);
    }
}