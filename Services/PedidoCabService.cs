using Reto_Desarrollo_Servidor_1ev.Models;
using Reto_Desarrollo_Servidor_1ev.Repositories;

namespace Reto_Desarrollo_Servidor_1ev.Services
{
    public class PedidoCabService : IPedidoCabService
    {
        private readonly IPedidoCabRepository _pedidoCabRepository;

        public PedidoCabService(IPedidoCabRepository pedidoCabRepository)
        {
            _pedidoCabRepository = pedidoCabRepository;
        }

        public async Task<List<PedidoCab>> GetAllAsync(QueryParamsFilters? filters)
        {
            return await _pedidoCabRepository.GetAllAsync(filters);
        }

        public async Task<PedidoCab?> GetByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("El ID debe ser mayor que cero.");
            return await _pedidoCabRepository.GetByIdAsync(id);
        }

        public async Task AddAsync(PedidoCab pedidoCab)
        {
            await _pedidoCabRepository.AddAsync(pedidoCab);
        }

        public async Task UpdateAsync(PedidoCab pedidoCab)
        {
            if (pedidoCab.idPedido <= 0)
                throw new ArgumentException("El ID debe ser mayor que cero.");
            await _pedidoCabRepository.UpdateAsync(pedidoCab);
        }

        public async Task DeleteAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("El ID debe ser mayor que cero.");
            await _pedidoCabRepository.DeleteAsync(id);
        }
    }
}
