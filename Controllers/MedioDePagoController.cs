using Microsoft.AspNetCore.Mvc;
using Reto_Desarrollo_Servidor_1ev.Services;
using Reto_Desarrollo_Servidor_1ev.Models;

namespace Reto_Desarrollo_Servidor_1ev.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MedioDePagoController : ControllerBase
    {
        private readonly IMedioDePagoService _medioDePagoService;

        public MedioDePagoController(IMedioDePagoService medioDePagoService)
        {
            _medioDePagoService = medioDePagoService;
        }

        [HttpGet]
        public async Task<ActionResult<List<MedioDePago>>> GetMediosDePago([FromQuery] QueryParamsFilters? filtros)
        {
            var medios = await _medioDePagoService.GetAllAsync(filtros);
            return Ok(medios);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MedioDePago>> GetById(int id)
        {
            var medio = await _medioDePagoService.GetByIdAsync(id);
            if (medio == null) return NotFound();
            return Ok(medio);
        }

        [HttpPost]
        public async Task<ActionResult> Add([FromBody] MedioDePago medioDePago)
        {
            await _medioDePagoService.AddAsync(medioDePago);
            return Ok();
        }

        [HttpPut]
        public async Task<ActionResult> Update([FromBody] MedioDePago medioDePago)
        {
            await _medioDePagoService.UpdateAsync(medioDePago);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            await _medioDePagoService.DeleteAsync(id);
            return Ok();
        }
    }
}
