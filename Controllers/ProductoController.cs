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

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] Producto producto)
        {
            if (id != producto.idProducto)
                return BadRequest(new { mensaje = "El ID no coincide" });

            var existente = await _productoService.GetByIdAsync(id);
            if (existente == null)
                return NotFound(new { mensaje = "Producto no encontrado" });

            try
            {
                await _productoService.UpdateAsync(producto);
                return Ok(new { mensaje = "Producto actualizado correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al actualizar", detalle = ex.Message });
            }
        }
        
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            await _productoService.DeleteAsync(id);
            return Ok();
        }
    }
}
