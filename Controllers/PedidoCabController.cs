using Microsoft.AspNetCore.Mvc;
using Reto_Desarrollo_Servidor_1ev.Services;
using Reto_Desarrollo_Servidor_1ev.Models;

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

        [HttpPut]
        public async Task<ActionResult> Update([FromBody] PedidoCab pedidoCab)
        {
            await _pedidoCabService.UpdateAsync(pedidoCab);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            await _pedidoCabService.DeleteAsync(id);
            return Ok();
        }
    }
}
