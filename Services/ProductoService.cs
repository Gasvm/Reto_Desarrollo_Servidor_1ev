using Reto_Desarrollo_Servidor_1ev.Models;
using Reto_Desarrollo_Servidor_1ev.Repositories;

namespace Reto_Desarrollo_Servidor_1ev.Services
{
    public class ProductoService : IProductoService
    {
        private readonly IProductoRepository _productoRepository;

        public ProductoService(IProductoRepository productoRepository)
        {
            _productoRepository = productoRepository;
        }

        public async Task<List<Producto>> GetAllAsync(QueryParamsFilters? filtroDescripcionProducto = null, QueryParamsFilters? filtroPrecioMinimo = null, QueryParamsFilters? filtroPrecioMaximo = null, QueryParamsFilters? filtroIdTipoIVA = null, QueryParamsFilters? filtroEstadoActivo = null)
        {
            return await _productoRepository.GetAllAsync(filtroDescripcionProducto, filtroPrecioMinimo, filtroPrecioMaximo, filtroIdTipoIVA, filtroEstadoActivo);
        }

        public async Task<Producto?> GetByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("El ID debe ser mayor que cero.");
            return await _productoRepository.GetByIdAsync(id);
        }

        public async Task AddAsync(Producto producto)
        {
            if (string.IsNullOrWhiteSpace(producto.descripcion))
                throw new ArgumentException("La descripción no puede estar vacía.");
            await _productoRepository.AddAsync(producto);
        }

        public async Task UpdateAsync(Producto producto)
        {
            if (producto.idProducto <= 0)
                throw new ArgumentException("El ID debe ser mayor que cero.");
            if (string.IsNullOrWhiteSpace(producto.descripcion))
                throw new ArgumentException("La descripción no puede estar vacía.");
            await _productoRepository.UpdateAsync(producto);
        }

        public async Task DeleteAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("El ID debe ser mayor que cero.");
            await _productoRepository.DeleteAsync(id);
        }
    }
}
