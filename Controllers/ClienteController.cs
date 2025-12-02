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

        /* OPCIÓN 1 - FILTRAMOS A NIVEL DE CONTROLLER - NO ES BUENA PRÁCTICA */
        /*
        [HttpGet]
        public async Task<ActionResult<List<Bebida>>> GetBebidas([FromQuery] QueryParamsFiltters? nombreProducto)
        {
            var _nombreProducto = nombreProducto.filtroNombreProducto ?? "";
            //_nombreProducto.AsQueryable();
            var bebidas = await _clienteService.GetAllAsync();

            var query = bebidas.AsQueryable();

            if (!string.IsNullOrEmpty(_nombreProducto))
            {
                query = query.Where(b => b.Nombre != null &&
                                        b.Nombre.Contains(_nombreProducto, StringComparison.OrdinalIgnoreCase));
            }

            if (query.Any())
            {
                bebidas = query.ToList();
            }

            return Ok(bebidas);
        }
        */

        /* OPCIÓN 2 - PASAMOS EL OBJETO A LA CAPA SERVICE - MEJOR OPCIÓN */

        [HttpGet]
        public async Task<ActionResult<List<Cliente>>> GetClientes([FromQuery] QueryParamsFilters? nombreCliente = null, QueryParamsFilters? estadoActivo = null)
        {
            var _nombreCliente = nombreCliente ?? null;
            var _estadoActivo = estadoActivo ?? null;
            //_nombreProducto.AsQueryable();
            var clientes = await _clienteService.GetAllAsync(_nombreCliente, _estadoActivo);
            
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
        public async Task<ActionResult<Cliente>> CreateBebida(Cliente cliente)
        {
            await _clienteService.AddAsync(cliente);
            return CreatedAtAction(nameof(GetCliente), new { id = cliente.idCliente }, cliente);
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