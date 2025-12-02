using Reto_Desarrollo_Servidor_1ev.Models;
using Reto_Desarrollo_Servidor_1ev.Repositories;

namespace Reto_Desarrollo_Servidor_1ev.Services
{
    public class PedidoLinService : IPedidoLinService
    {
        private readonly IPedidoLinRepository _pedidoLinRepository;

        public PedidoLinService(IPedidoLinRepository pedidoLinRepository)
        {
            _pedidoLinRepository = pedidoLinRepository;
        }

        public async Task<List<PedidoLin>> GetAllAsync(QueryParamsFilters? filtroIdPedido = null, QueryParamsFilters? filtroIdProducto = null, QueryParamsFilters? filtroEstadoActivo = null)
        {
            return await _pedidoLinRepository.GetAllAsync(filtroIdPedido, filtroIdProducto, filtroEstadoActivo);
        }

        public async Task<PedidoLin?> GetByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("El ID debe ser mayor que cero.");
            return await _pedidoLinRepository.GetByIdAsync(id);
        }

        public async Task AddAsync(PedidoLin pedidoLin)
        {
            await _pedidoLinRepository.AddAsync(pedidoLin);
        }

        public async Task UpdateAsync(PedidoLin pedidoLin)
        {
            if (pedidoLin.idLineaPedido <= 0)
                throw new ArgumentException("El ID debe ser mayor que cero.");
            await _pedidoLinRepository.UpdateAsync(pedidoLin);
        }

        public async Task DeleteAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("El ID debe ser mayor que cero.");
            await _pedidoLinRepository.DeleteAsync(id);
        }
    }
}
