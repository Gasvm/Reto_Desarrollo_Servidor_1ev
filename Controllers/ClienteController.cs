using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using Reto_Desarrollo_Servidor_1ev.Services;
using Reto_Desarrollo_Servidor_1ev.Models;

namespace Reto_Desarrollo_Servidor_1ev.Controllers
{
   [Route("api/[controller]")]
   [ApiController]
   public class ClienteController : ControllerBase
   {
    private static List<Cliente> bebidas = new List<Cliente>();

    private readonly IClienteService _clienteService;

        public ClienteController(IClienteService clienteService)
        {
            _clienteService = clienteService;
        }


        [HttpGet]
        public async Task<ActionResult<List<Cliente>>> GetClientes([FromQuery] QueryParamsFilters? filters)
        {
            
            var clientes = await _clienteService.GetAllAsync(filters);
            
            return Ok(clientes);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<Cliente>> GetCliente(int id)
        {
            var cliente = await _clienteService.GetByIdAsync(id);
            if (cliente == null)
            {
                return NotFound();
            }
            return Ok(cliente);
        }

        [HttpPost]
        public async Task<ActionResult<Cliente>> CreateCliente(Cliente cliente)
        {
            try
            {
                var idGenerado = await _clienteService.AddAsync(cliente);
                cliente.idCliente = idGenerado;
                return CreatedAtAction(nameof(GetCliente), new { id = idGenerado }, cliente);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { mensaje = ex.Message }); // 409 Conflict
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = "Error al crear el cliente", detalle = ex.Message });
            }
        }

       [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCliente(int id, Cliente updatedCliente)
        {
            var existingCliente = await _clienteService.GetByIdAsync(id);
            if (existingCliente == null)
            {
                return NotFound();
            }

            existingCliente.idCliente = updatedCliente.idCliente;
            existingCliente.nombre = updatedCliente.nombre;
            existingCliente.apellidos = updatedCliente.apellidos;
            existingCliente.email = updatedCliente.email;
            existingCliente.password = updatedCliente.password;
            existingCliente.telefono = updatedCliente.telefono;
            existingCliente.fechaCreacion = updatedCliente.fechaCreacion;
            existingCliente.activo = updatedCliente.activo;
            

            await _clienteService.UpdateAsync(existingCliente);
            return NoContent();
        }

        
       [HttpDelete("{id}")]
       public async Task<IActionResult> DeleteCliente(int id)
       {
           var cliente = await _clienteService.GetByIdAsync(id);
           if (cliente == null)
           {
               return NotFound();
           }
           await _clienteService.DeleteAsync(id);
           return NoContent();
       }

   }
}