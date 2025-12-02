using Reto_Desarrollo_Servidor_1ev.Models;
using Reto_Desarrollo_Servidor_1ev.Repositories;

namespace Reto_Desarrollo_Servidor_1ev.Services
{
    public class TarjetaCreditoService : ITarjetaCreditoService
    {
        private readonly ITarjetaCreditoRepository _tarjetaCreditoRepository;

        public TarjetaCreditoService(ITarjetaCreditoRepository tarjetaCreditoRepository)
        {
            _tarjetaCreditoRepository = tarjetaCreditoRepository;
        }

        public async Task<List<TarjetaCredito>> GetAllAsync(QueryParamsFilters? filtroIdClienteTarjeta = null, QueryParamsFilters? filtroDescripcionTarjeta = null, QueryParamsFilters? filtroNumeroTarjeta = null, QueryParamsFilters? filtroFechaCaducidadDesde = null, QueryParamsFilters? filtroFechaCaducidadHasta = null, QueryParamsFilters? filtroEstadoActivo = null)
        {
            return await _tarjetaCreditoRepository.GetAllAsync(filtroIdClienteTarjeta, filtroDescripcionTarjeta, filtroNumeroTarjeta, filtroFechaCaducidadDesde, filtroFechaCaducidadHasta, filtroEstadoActivo);
        }

        public async Task<TarjetaCredito?> GetByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("El ID debe ser mayor que cero.");
            return await _tarjetaCreditoRepository.GetByIdAsync(id);
        }

        public async Task AddAsync(TarjetaCredito tarjetaCredito)
        {
            if (string.IsNullOrWhiteSpace(tarjetaCredito.descripcion))
                throw new ArgumentException("La descripción no puede estar vacía.");
            await _tarjetaCreditoRepository.AddAsync(tarjetaCredito);
        }

        public async Task UpdateAsync(TarjetaCredito tarjetaCredito)
        {
            if (tarjetaCredito.idTarjetaCredito <= 0)
                throw new ArgumentException("El ID debe ser mayor que cero.");
            if (string.IsNullOrWhiteSpace(tarjetaCredito.descripcion))
                throw new ArgumentException("La descripción no puede estar vacía.");
            await _tarjetaCreditoRepository.UpdateAsync(tarjetaCredito);
        }

        public async Task DeleteAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("El ID debe ser mayor que cero.");
            await _tarjetaCreditoRepository.DeleteAsync(id);
        }
    }
}
