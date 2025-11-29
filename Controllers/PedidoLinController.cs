using Microsoft.AspNetCore.Mvc;
using Reto_Desarrollo_Servidor_1ev.Services;
using Reto_Desarrollo_Servidor_1ev.Models;

namespace Reto_Desarrollo_Servidor_1ev.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PedidoLinController : ControllerBase
    {
        private readonly IPedidoLinService _pedidoLinService;

        public PedidoLinController(IPedidoLinService pedidoLinService)
        {
            _pedidoLinService = pedidoLinService;
        }

        [HttpGet]
        public async Task<ActionResult<List<PedidoLin>>> GetPedidosLin([FromQuery] QueryParamsFilters? filtros)
        {
            var lineas = await _pedidoLinService.GetAllAsync(filtros, filtros, filtros);
            return Ok(lineas);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PedidoLin>> GetById(int id)
        {
            var linea = await _pedidoLinService.GetByIdAsync(id);
            if (linea == null) return NotFound();
            return Ok(linea);
        }

        [HttpPost]
        public async Task<ActionResult> Add([FromBody] PedidoLin pedidoLin)
        {
            await _pedidoLinService.AddAsync(pedidoLin);
            return Ok();
        }

        [HttpPut]
        public async Task<ActionResult> Update([FromBody] PedidoLin pedidoLin)
        {
            await _pedidoLinService.UpdateAsync(pedidoLin);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            await _pedidoLinService.DeleteAsync(id);
            return Ok();
        }
    }
}
