using Reto_Desarrollo_Servidor_1ev.Models;
using Reto_Desarrollo_Servidor_1ev.Repositories;

namespace Reto_Desarrollo_Servidor_1ev.Services
{
    public class TipoIVAService : ITipoIVAService
    {
        private readonly ITipoIVARepository _tipoIVARepository;

        public TipoIVAService(ITipoIVARepository tipoIVARepository)
        {
            _tipoIVARepository = tipoIVARepository;
        }

        public async Task<List<TipoIVA>> GetAllAsync(QueryParamsFilters? filtroTasaMinima = null, QueryParamsFilters? filtroTasaMaxima = null, QueryParamsFilters? filtroDescripcionTipoIVA = null, QueryParamsFilters? filtroFechaCreacionDesde = null, QueryParamsFilters? filtroFechaCreacionHasta = null, QueryParamsFilters? filtroEstadoActivo = null)
        {
            return await _tipoIVARepository.GetAllAsync(filtroTasaMinima, filtroTasaMaxima, filtroDescripcionTipoIVA, filtroFechaCreacionDesde, filtroFechaCreacionHasta, filtroEstadoActivo);
        }

        public async Task<TipoIVA?> GetByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("El ID debe ser mayor que cero.");
            return await _tipoIVARepository.GetByIdAsync(id);
        }

        public async Task AddAsync(TipoIVA tipoIVA)
        {
            if (string.IsNullOrWhiteSpace(tipoIVA.descripcion))
                throw new ArgumentException("La descripción no puede estar vacía.");
            await _tipoIVARepository.AddAsync(tipoIVA);
        }

        public async Task UpdateAsync(TipoIVA tipoIVA)
        {
            if (tipoIVA.idTipoIVA <= 0)
                throw new ArgumentException("El ID debe ser mayor que cero.");
            if (string.IsNullOrWhiteSpace(tipoIVA.descripcion))
                throw new ArgumentException("La descripción no puede estar vacía.");
            await _tipoIVARepository.UpdateAsync(tipoIVA);
        }

        public async Task DeleteAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("El ID debe ser mayor que cero.");
            await _tipoIVARepository.DeleteAsync(id);
        }
    }
}
