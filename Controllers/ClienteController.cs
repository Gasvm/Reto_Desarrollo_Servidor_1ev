using Microsoft.AspNetCore.Mvc;
using Reto_Desarrollo_Servidor_1ev.Services;
using Reto_Desarrollo_Servidor_1ev.Models;
using Reto_Desarrollo_Servidor_1ev.Models.DTOs;

namespace Reto_Desarrollo_Servidor_1ev.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClienteController : ControllerBase
    {
        private readonly IClienteService _clienteService;

        public ClienteController(IClienteService clienteService)
        {
            _clienteService = clienteService;
        }

        [HttpGet]
        public async Task<ActionResult<List<ClienteResponseDTO>>> GetClientes([FromQuery] QueryParamsFilters? filters)
        {
            var clientes = await _clienteService.GetAllAsync(filters);
            
            var clientesDTO = clientes.Select(c => new ClienteResponseDTO
            {
                IdCliente = c.idCliente ?? 0,
                Nombre = c.nombre,
                Apellidos = c.apellidos,
                Email = c.email,
                Telefono = c.telefono,
                FechaCreacion = c.fechaCreacion ?? DateTime.Now,
                Activo = c.activo
            }).ToList();
            
            return Ok(clientesDTO);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ClienteResponseDTO>> GetCliente(int id)
        {
            var cliente = await _clienteService.GetByIdAsync(id);
            if (cliente == null)
            {
                return NotFound();
            }

            var clienteDTO = new ClienteResponseDTO
            {
                IdCliente = cliente.idCliente ?? 0,
                Nombre = cliente.nombre,
                Apellidos = cliente.apellidos,
                Email = cliente.email,
                Telefono = cliente.telefono,
                FechaCreacion = cliente.fechaCreacion ?? DateTime.Now,
                Activo = cliente.activo
            };

            return Ok(clienteDTO);
        }

        [HttpPost]
        public async Task<ActionResult<ClienteResponseDTO>> CreateCliente(ClienteCreateDTO clienteDTO)
        {
            try
            {
                var cliente = new Cliente
                {
                    nombre = clienteDTO.Nombre,
                    apellidos = clienteDTO.Apellidos,
                    email = clienteDTO.Email,
                    password = clienteDTO.Password,
                    telefono = clienteDTO.Telefono,
                    fechaCreacion = DateTime.Now,
                    activo = true
                };

                var idGenerado = await _clienteService.AddAsync(cliente);
                cliente.idCliente = idGenerado;

                var responseDTO = new ClienteResponseDTO
                {
                    IdCliente = idGenerado,
                    Nombre = cliente.nombre,
                    Apellidos = cliente.apellidos,
                    Email = cliente.email,
                    Telefono = cliente.telefono,
                    FechaCreacion = cliente.fechaCreacion ?? DateTime.Now,
                    Activo = cliente.activo
                };

                return CreatedAtAction(nameof(GetCliente), new { id = idGenerado }, responseDTO);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = "Error al crear el cliente", detalle = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCliente(int id, ClienteUpdateDTO clienteDTO)
        {
            if (id != clienteDTO.IdCliente)
            {
                return BadRequest(new { mensaje = "El ID no coincide" });
            }

            var existingCliente = await _clienteService.GetByIdAsync(id);
            if (existingCliente == null)
            {
                return NotFound();
            }

            existingCliente.nombre = clienteDTO.Nombre;
            existingCliente.apellidos = clienteDTO.Apellidos;
            existingCliente.email = clienteDTO.Email;
            existingCliente.telefono = clienteDTO.Telefono;
            existingCliente.activo = clienteDTO.Activo;
            
            if (!string.IsNullOrEmpty(clienteDTO.Password))
            {
                existingCliente.password = clienteDTO.Password;
            }

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