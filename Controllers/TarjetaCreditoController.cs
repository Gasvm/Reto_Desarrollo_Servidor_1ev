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
            var tarjetas = await _tarjetaCreditoService.GetAllAsync(filtros, filtros, filtros, filtros, filtros, filtros);
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
            await _tarjetaCreditoService.AddAsync(tarjetaCredito);
            return Ok();
        }

        [HttpPut]
        public async Task<ActionResult> Update([FromBody] TarjetaCredito tarjetaCredito)
        {
            await _tarjetaCreditoService.UpdateAsync(tarjetaCredito);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            await _tarjetaCreditoService.DeleteAsync(id);
            return Ok();
        }
    }
}
