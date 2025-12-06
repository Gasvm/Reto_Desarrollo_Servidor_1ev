using Microsoft.AspNetCore.Mvc;
using Reto_Desarrollo_Servidor_1ev.Services;
using Reto_Desarrollo_Servidor_1ev.Models;

namespace Reto_Desarrollo_Servidor_1ev.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TarjetaCreditoController : ControllerBase
    {
        private readonly ITarjetaCreditoService _tarjetaCreditoService;

        public TarjetaCreditoController(ITarjetaCreditoService tarjetaCreditoService)
        {
            _tarjetaCreditoService = tarjetaCreditoService;
        }

        [HttpGet]
        public async Task<ActionResult<List<TarjetaCredito>>> GetTarjetasCredito([FromQuery] QueryParamsFilters? filtros)
        {
            var tarjetas = await _tarjetaCreditoService.GetAllAsync(filtros);
            return Ok(tarjetas);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TarjetaCredito>> GetById(int id)
        {
            var tarjeta = await _tarjetaCreditoService.GetByIdAsync(id);
            if (tarjeta == null) return NotFound();
            return Ok(tarjeta);
        }

        [HttpPost]
        public async Task<ActionResult> Add([FromBody] TarjetaCredito tarjetaCredito)
        {
            try
            {
                await _tarjetaCreditoService.AddAsync(tarjetaCredito);
                return Ok(new { mensaje = "Tarjeta de crédito creada exitosamente" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error interno del servidor", detalle = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] TarjetaCredito tarjetaCredito)
        {
            // Validar que el ID de la ruta coincida con el del body
            if (id != tarjetaCredito.idTarjetaCredito)
                return BadRequest(new { mensaje = "El ID no coincide" });

            // Verificar que la tarjeta existe
            var existente = await _tarjetaCreditoService.GetByIdAsync(id);
            if (existente == null)
                return NotFound(new { mensaje = "Tarjeta de crédito no encontrada" });

            try
            {
                await _tarjetaCreditoService.UpdateAsync(tarjetaCredito);
                return Ok(new { mensaje = "Tarjeta de crédito actualizada exitosamente" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error interno del servidor", detalle = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                await _tarjetaCreditoService.DeleteAsync(id);
                return Ok(new { mensaje = "Tarjeta de crédito eliminada exitosamente" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error interno del servidor", detalle = ex.Message });
            }
        }
    }
}
