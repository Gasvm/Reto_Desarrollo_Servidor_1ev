using Reto_Desarrollo_Servidor_1ev.Models;

namespace Reto_Desarrollo_Servidor_1ev.Repositories
{
    public interface IPedidoCabRepository
    {
        Task<List<PedidoCab>> GetAllAsync(QueryParamsFilters? filters);
        Task<PedidoCab?> GetByIdAsync(int id);
        Task<int> AddAsync(PedidoCab pedidoCab); //Modificado para devolver el ID del pedidoCab
        Task UpdateAsync(PedidoCab pedidoCab);
        Task DeleteAsync(int id);
    }
}