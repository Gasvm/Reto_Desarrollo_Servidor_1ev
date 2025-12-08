using Microsoft.AspNetCore.Mvc;
using Reto_Desarrollo_Servidor_1ev.Services;
using Reto_Desarrollo_Servidor_1ev.Models;
using Reto_Desarrollo_Servidor_1ev.Models.DTOs;

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
        public async Task<ActionResult<List<TarjetaCreditoResponseDTO>>> GetTarjetasCredito([FromQuery] QueryParamsFilters? filtros)
        {
            var tarjetasDTO = await _tarjetaCreditoService.GetAllDTOAsync(filtros);
            return Ok(tarjetasDTO);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TarjetaCreditoResponseDTO>> GetById(int id)
        {
            var tarjetaDTO = await _tarjetaCreditoService.GetByIdDTOAsync(id);
            if (tarjetaDTO == null) return NotFound();
            return Ok(tarjetaDTO);
        }

        [HttpPost]
        public async Task<ActionResult<TarjetaCreditoResponseDTO>> Add([FromBody] TarjetaCreditoCreateDTO tarjetaDTO)
        {
            try
            {
                var tarjeta = new TarjetaCredito
                {
                    descripcion = tarjetaDTO.Descripcion,
                    numeroTarjeta = tarjetaDTO.NumeroTarjeta,
                    fechaCaducidad = tarjetaDTO.FechaCaducidad,
                    idCliente = tarjetaDTO.IdCliente,
                    fechaCreacion = DateTime.Now,
                    activo = true
                };

                var idGenerado = await _tarjetaCreditoService.AddAsync(tarjeta);

                // Obtener y devolver DTO con número enmascarado
                var tarjetaCreada = await _tarjetaCreditoService.GetByIdDTOAsync(idGenerado);
                
                return CreatedAtAction(nameof(GetById), new { id = idGenerado }, tarjetaCreada);
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
        public async Task<ActionResult> Update(int id, [FromBody] TarjetaCreditoUpdateDTO tarjetaDTO)
        {
            // Validar que el ID de la ruta coincida con el del body
            if (id != tarjetaDTO.IdTarjetaCredito)
                return BadRequest(new { mensaje = "El ID no coincide" });

            // Verificar que la tarjeta existe
            var existente = await _tarjetaCreditoService.GetByIdAsync(id);
            if (existente == null)
                return NotFound(new { mensaje = "Tarjeta de crédito no encontrada" });

            try
            {
                existente.descripcion = tarjetaDTO.Descripcion;
                existente.fechaCaducidad = tarjetaDTO.FechaCaducidad;
                existente.idCliente = tarjetaDTO.IdCliente;
                existente.activo = tarjetaDTO.Activo;

                if (!string.IsNullOrEmpty(tarjetaDTO.NumeroTarjeta))
                {
                    existente.numeroTarjeta = tarjetaDTO.NumeroTarjeta;
                }

                await _tarjetaCreditoService.UpdateAsync(existente);
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