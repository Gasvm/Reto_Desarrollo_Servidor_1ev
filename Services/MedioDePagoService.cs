using Reto_Desarrollo_Servidor_1ev.Models;
using Reto_Desarrollo_Servidor_1ev.Repositories;

namespace Reto_Desarrollo_Servidor_1ev.Services
{
    public class MedioDePagoService : IMedioDePagoService
    {
        private readonly IMedioDePagoRepository _medioDePagoRepository;

        public MedioDePagoService(IMedioDePagoRepository medioDePagoRepository)
        {
            _medioDePagoRepository = medioDePagoRepository;
        }

        public async Task<List<MedioDePago>> GetAllAsync(QueryParamsFilters? filtroDescripcionMedioDePago = null, QueryParamsFilters? filtroEstadoActivo = null)
        {
            return await _medioDePagoRepository.GetAllAsync(filtroDescripcionMedioDePago, filtroEstadoActivo);
        }

        public async Task<MedioDePago?> GetByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("El ID debe ser mayor que cero.");
            return await _medioDePagoRepository.GetByIdAsync(id);
        }

        public async Task AddAsync(MedioDePago medioDePago)
        {
            if (string.IsNullOrWhiteSpace(medioDePago.descripcion))
                throw new ArgumentException("La descripción no puede estar vacía.");
            await _medioDePagoRepository.AddAsync(medioDePago);
        }

        public async Task UpdateAsync(MedioDePago medioDePago)
        {
            if (medioDePago.idMedioDePago <= 0)
                throw new ArgumentException("El ID debe ser mayor que cero.");
            if (string.IsNullOrWhiteSpace(medioDePago.descripcion))
                throw new ArgumentException("La descripción no puede estar vacía.");
            await _medioDePagoRepository.UpdateAsync(medioDePago);
        }

        public async Task DeleteAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("El ID debe ser mayor que cero.");
            await _medioDePagoRepository.DeleteAsync(id);
        }
    }
}
