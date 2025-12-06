using Microsoft.AspNetCore.Mvc;
using Reto_Desarrollo_Servidor_1ev.Services;
using Reto_Desarrollo_Servidor_1ev.Models;

namespace Reto_Desarrollo_Servidor_1ev.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TipoIVAController : ControllerBase
    {
        private readonly ITipoIVAService _tipoIVAService;

        public TipoIVAController(ITipoIVAService tipoIVAService)
        {
            _tipoIVAService = tipoIVAService;
        }

        [HttpGet]
        public async Task<ActionResult<List<TipoIVA>>> GetTiposIVA([FromQuery] QueryParamsFilters? filtros)
        {
            var tipos = await _tipoIVAService.GetAllAsync(filtros);
            return Ok(tipos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TipoIVA>> GetById(int id)
        {
            var tipo = await _tipoIVAService.GetByIdAsync(id);
            if (tipo == null) return NotFound();
            return Ok(tipo);
        }

        [HttpPost]
        public async Task<ActionResult> Add([FromBody] TipoIVA tipoIVA)
        {
            await _tipoIVAService.AddAsync(tipoIVA);
            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] TipoIVA tipoIVA)
        {
            if (id != tipoIVA.idTipoIVA)
                return BadRequest(new { mensaje = "El ID no coincide" });
            
            await _tipoIVAService.UpdateAsync(tipoIVA);
            return Ok(new { mensaje = "Tipo IVA actualizado" });
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            await _tipoIVAService.DeleteAsync(id);
            return Ok();
        }
    }
}
