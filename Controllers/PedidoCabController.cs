using Microsoft.AspNetCore.Mvc;
using Reto_Desarrollo_Servidor_1ev.Services;
using Reto_Desarrollo_Servidor_1ev.Models;
using Reto_Desarrollo_Servidor_1ev.Services.DTOs;
using Reto_Desarrollo_Servidor_1ev.Models.DTOs;

namespace Reto_Desarrollo_Servidor_1ev.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PedidoCabController : ControllerBase
    {
        private readonly IPedidoCabService _pedidoCabService;

        public PedidoCabController(IPedidoCabService pedidoCabService)
        {
            _pedidoCabService = pedidoCabService;
        }

        [HttpGet]
        public async Task<ActionResult<List<PedidoCab>>> GetPedidosCab([FromQuery] QueryParamsFilters? filtros)
        {
            var pedidos = await _pedidoCabService.GetAllAsync(filtros);
            return Ok(pedidos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PedidoCab>> GetById(int id)
        {
            var pedido = await _pedidoCabService.GetByIdAsync(id);
            if (pedido == null) return NotFound();
            return Ok(pedido);
        }

        // ENDPOINT: Obtener pedido completo con toda la información
        [HttpGet("{id}/completo")]
        public async Task<ActionResult<PedidoCompletoDTO>> GetPedidoCompleto(int id)
        {
            try
            {
                var pedidoCompleto = await _pedidoCabService.GetPedidoCompletoAsync(id);
                
                if (pedidoCompleto == null)
                    return NotFound(new { mensaje = $"Pedido con ID {id} no encontrado" });
                
                return Ok(pedidoCompleto);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error interno del servidor", detalle = ex.Message });
            }
        }

        // ENDPOINT: Crear pedido completo (cabecera + líneas en una transacción)
        [HttpPost("completo")]
        public async Task<ActionResult<PedidoCompletoDTO>> CreatePedidoCompleto([FromBody] CreatePedidoCompletoDTO pedidoDTO)
        {
            try
            {
                var idGenerado = await _pedidoCabService.CreatePedidoCompletoAsync(pedidoDTO);
                
                // Obtener el pedido completo recién creado
                var pedidoCompleto = await _pedidoCabService.GetPedidoCompletoAsync(idGenerado);
                
                return CreatedAtAction(
                    nameof(GetPedidoCompleto), 
                    new { id = idGenerado }, 
                    pedidoCompleto
                );
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
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


        [HttpPost]
        public async Task<ActionResult> Add([FromBody] PedidoCab pedidoCab)
        {
            try
            {
                var idGenerado = await _pedidoCabService.AddAsync(pedidoCab);
                return Ok(new { idPedido = idGenerado, mensaje = "Pedido creado exitosamente" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] PedidoCab pedidoCab)
        {
            if (id != pedidoCab.idPedido)
                return BadRequest(new { mensaje = "El ID no coincide" });
            
            await _pedidoCabService.UpdateAsync(pedidoCab);
            return Ok(new { mensaje = "Pedido actualizado" });
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            await _pedidoCabService.DeleteAsync(id);
            return Ok();
        }
    }
}
