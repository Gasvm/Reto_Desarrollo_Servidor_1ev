using Microsoft.AspNetCore.Mvc;
using Reto_Desarrollo_Servidor_1ev.Services;
using Reto_Desarrollo_Servidor_1ev.Models;

namespace Reto_Desarrollo_Servidor_1ev.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductoController : ControllerBase
    {
        private readonly IProductoService _productoService;

        public ProductoController(IProductoService productoService)
        {
            _productoService = productoService;
        }

        [HttpGet]
        public async Task<ActionResult<List<Producto>>> GetProductos([FromQuery] QueryParamsFilters? filtros)
        {
            var productos = await _productoService.GetAllAsync(filtros);
            return Ok(productos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Producto>> GetById(int id)
        {
            var producto = await _productoService.GetByIdAsync(id);
            if (producto == null) return NotFound();
            return Ok(producto);
        }

        [HttpPost]
        public async Task<ActionResult> Add([FromBody] Producto producto)
        {
            await _productoService.AddAsync(producto);
            return Ok();
        }

        [HttpPut]
        public async Task<ActionResult> Update([FromBody] Producto producto)
        {
            await _productoService.UpdateAsync(producto);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            await _productoService.DeleteAsync(id);
            return Ok();
        }
    }
}
