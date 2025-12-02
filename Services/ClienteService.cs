using Reto_Desarrollo_Servidor_1ev.Models;
using Reto_Desarrollo_Servidor_1ev.Repositories;

namespace Reto_Desarrollo_Servidor_1ev.Services
{
    public class ClienteService : IClienteService
    {
        private readonly IClienteRepository _ClienteRepository;

        public ClienteService(IClienteRepository ClienteRepository)
        {
            _ClienteRepository = ClienteRepository;
            
        }

        public async Task<List<Cliente>> GetAllAsync(QueryParamsFilters? nombreCliente = null, QueryParamsFilters? estadoActivo = null)
        {
            return await _ClienteRepository.GetAllAsync(nombreCliente, estadoActivo);
        }

        public async Task<Cliente?> GetByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("El ID debe ser mayor que cero.");

            return await _ClienteRepository.GetByIdAsync(id);
        }

        public async Task AddAsync(Cliente cliente)
        {
            if (string.IsNullOrWhiteSpace(cliente.nombre))
                throw new ArgumentException("El nombre del cliente no puede estar vacío.");

            if (string.IsNullOrWhiteSpace(cliente.apellidos))
                throw new ArgumentException("El apellido del cliente no puede estar vacío.");
            
            if (string.IsNullOrWhiteSpace(cliente.email))
                throw new ArgumentException("El email del cliente no puede estar vacío.");


            await _ClienteRepository.AddAsync(cliente);
        }

        public async Task UpdateAsync(Cliente cliente)
        {
            if (cliente.idCliente <= 0)
                throw new ArgumentException("El ID no es válido para actualización.");

            if (string.IsNullOrWhiteSpace(cliente.nombre))
                throw new ArgumentException("El nombre del cliente no puede estar vacío.");

            if (string.IsNullOrWhiteSpace(cliente.apellidos))
                throw new ArgumentException("El apellido del cliente no puede estar vacío.");
            
            if (string.IsNullOrWhiteSpace(cliente.email))
                throw new ArgumentException("El email del cliente no puede estar vacío.");

            await _ClienteRepository.UpdateAsync(cliente);
        }

        public async Task DeleteAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("El ID no es válido para eliminación.");

            await _ClienteRepository.DeleteAsync(id);
        }

    }
}