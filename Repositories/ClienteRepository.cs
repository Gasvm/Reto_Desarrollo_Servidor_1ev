using System.Data;
using Micorosoft.Data.SqlClient;

namespace Reto_Desarrollo_Servidor_1ev.Repositories
{
    public class ClienteRepository : IClienteRepository
    {
        private readonly string _connectionString;

        public ClienteRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("PedidosBD") ?? "Not found";
        }

        
        //Método asíncrono para obtener todos los clientes de la base de datos
        public async Task<List<Cliente>> GetAllAsync(QueryParamsFilters? NombreCliente, QueryParamsFilters? estadoActivo)
        {
            var clientes = new List<Cliente>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                
                string query = "SELECT idCliente, nombre, apellido, email, password, telefono, fechaRegistro FROM tbClientes";
                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var cliente = new Cliente
                            {
                                idCliente = reader.GetInt32(0),
                                nombre = reader.GetString(1),
                                apellidos = reader.GetString(2),
                                email = reader.GetString(3),
                                password = reader.GetString(4),
                                telefono = reader.GetString(5),
                                fechaRegistro = reader.GetDateTime(6)
                            };

                            clientes.Add(cliente);
                        }
                    }
                }
                
                var _nombreCliente = NombreCliente.filtroNombreCliente ?? "";
                _nombreCliente.AsQueryable();
                
                var miQuery = clientes.AsQueryable();

                if (!string.IsNullOrEmpty(_nombreCliente))
                {
                    miQuery = miQuery.Where(c => c.nombre != null &&
                                            c.Nombre.Contains(_nombreCliente, StringComparison.OrdinalIgnoreCase));
                    // Añadido filtro para buscar también por apellidos
                    miQuery = miQuery.Where(c => c.apellidos != null &&
                                            c.apellidos.Contains(_nombreCliente, StringComparison.OrdinalIgnoreCase));
                }

                var _estadoActivoCliente = estadoActivo.filtroEstadoActivo;
                
                if (_estadoActivoCliente.HasValue)
                {
                    miQuery = miQuery.Where(c => c.activo == _estadoActivoCliente.Value);
                }

                
            }

            return clientes;
        }

    }
}