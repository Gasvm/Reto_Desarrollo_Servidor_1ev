using Reto_Desarrollo_Servidor_1ev.Models;
using Reto_Desarrollo_Servidor_1ev.Models.DTOs;

namespace Reto_Desarrollo_Servidor_1ev.Services
{
    public interface ITarjetaCreditoService
    {
        Task<List<TarjetaCredito>> GetAllAsync(QueryParamsFilters? filters);
        Task<TarjetaCredito?> GetByIdAsync(int id);
        Task<List<TarjetaCreditoResponseDTO>> GetAllDTOAsync(QueryParamsFilters? filters);
        Task<TarjetaCreditoResponseDTO?> GetByIdDTOAsync(int id);
        Task<int> AddAsync(TarjetaCredito tarjeta);
        Task UpdateAsync(TarjetaCredito tarjeta);
        Task DeleteAsync(int id);
    }
}