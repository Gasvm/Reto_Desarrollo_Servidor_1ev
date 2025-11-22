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

                if (miQuery.Any())
                {
                    clientes = miQuery.ToList();
                }
                
            }

            return clientes;
        }


        //No aplicado filtro de estadoActivo ya que se asume que se quiere obtener el cliente aunque esté inactivo
        public async Task<Cliente?> GetByIdAsync(int id)
        {
            Cliente? cliente = null;

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = "SELECT idCliente, nombre, apellido, email, password, telefono, fechaRegistro FROM tbClientes WHERE idCliente = @Id";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            cliente = new Cliente
                            {
                                idCliente = reader.GetInt32(0),
                                nombre = reader.GetString(1),
                                apellidos = reader.GetString(2),
                                email = reader.GetString(3),
                                password = reader.GetString(4),
                                telefono = reader.GetString(5),
                                fechaRegistro = reader.GetDateTime(6)
                            };
                        }
                    }
                }
            }

            return cliente;
        }


        //Nos estamos planteando si sería conveniente devolver el id generado al insertar un nuevo cliente
        public async Task AddAsync(Cliente cliente)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = "INSERT INTO tbClientes (nombre, apellido, email, password, telefono, fechaRegistro, activo) " +
                               "VALUES (@Nombre, @Apellido, @Email, @Password, @Telefono, @FechaRegistro, @Activo)";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Nombre", cliente.nombre ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Apellido", cliente.apellidos ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Email", cliente.email ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Password", cliente.password ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Telefono", cliente.telefono ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@FechaRegistro", cliente.fechaRegistro ?? DateTime.Now);
                    command.Parameters.AddWithValue("@Activo", cliente.activo);

                    await command.ExecuteNonQueryAsync();
                }
            }
            
        }


        public async Task UpdateAsync(Cliente cliente)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = "UPDATE tbClientes SET nombre = @Nombre, apellido = @Apellido, email = @Email, " +
                               "password = @Password, telefono = @Telefono, fechaRegistro = @FechaRegistro, activo = @Activo " +
                               "WHERE idCliente = @Id";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", cliente.idCliente ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Nombre", cliente.nombre ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Apellido", cliente.apellidos ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Email", cliente.email ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Password", cliente.password ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Telefono", cliente.telefono ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@FechaRegistro", cliente.fechaRegistro ?? DateTime.Now);
                    command.Parameters.AddWithValue("@Activo", cliente.activo);

                    await command.ExecuteNonQueryAsync();
                }
            }

        }

        //Aplicamos soft delete cambiando el estado del cliente a inactivo
        public async Task DeleteAsync(int id)
        {

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = "UPDATE tbClientes SET activo = 0 WHERE idCliente = @Id";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    await command.ExecuteNonQueryAsync();
                }
            }


            throw new NotImplementedException();
        }
    }
}