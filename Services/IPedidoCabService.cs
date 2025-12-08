using Reto_Desarrollo_Servidor_1ev.Models;
using Reto_Desarrollo_Servidor_1ev.Models.DTOs;
using Reto_Desarrollo_Servidor_1ev.Services.DTOs;

namespace Reto_Desarrollo_Servidor_1ev.Services
{
    public interface IPedidoCabService
    {
        Task<List<PedidoCab>> GetAllAsync(QueryParamsFilters? filters);
        Task<PedidoCab?> GetByIdAsync(int id);
        Task<int> AddAsync(PedidoCab pedidoCab); //Modificado para devolver el ID del pedidoCab
        Task UpdateAsync(PedidoCab pedidoCab);
        Task DeleteAsync(int id);

        Task<PedidoCompletoDTO?> GetPedidoCompletoAsync(int idPedido);
        Task<int> CreatePedidoCompletoAsync(CreatePedidoCompletoDTO pedidoDTO);
    }
}
