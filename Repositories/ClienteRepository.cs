using Reto_Desarrollo_Servidor_1ev.Models;
using System.Data;
using Microsoft.Data.SqlClient;

namespace Reto_Desarrollo_Servidor_1ev.Repositories
{
    public class ClienteRepository : IClienteRepository
    {
        private readonly string _connectionString;

        public ClienteRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("SistemaPedidosDB") ?? "Not found";
        }

        
        //Método asíncrono para obtener todos los clientes de la base de datos
        public async Task<List<Cliente>> GetAllAsync(QueryParamsFilters? filters)
        {
            var clientes = new List<Cliente>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                
                string query = "SELECT idCliente, nombre, apellidos, email, password, telefono, fechaCreacion, activo FROM tbClientes";
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
                                telefono = reader.IsDBNull(5) ? null : reader.GetString(5),
                                fechaCreacion = reader.GetDateTime(6),
                                activo = reader.GetBoolean(7)
                            };

                            clientes.Add(cliente);
                        }
                    }
                }
                
                
                                
                var miQuery = clientes.AsQueryable();
                
                // Filtro por nombre y apellidos
                var _nombreCliente = filters.filtroNombreCliente ?? "";

                if (!string.IsNullOrEmpty(_nombreCliente))
                {
                    miQuery = miQuery.Where(c => c.nombre != null &&  c.nombre.Contains(_nombreCliente) || c.apellidos.Contains(_nombreCliente) );
                    clientes = miQuery.ToList();
                }

                // Filtro por email
                var _emailCliente = filters?.filtroEmailCliente ?? "";
                
                if (!string.IsNullOrEmpty(_emailCliente))
                {
                    miQuery = miQuery.Where(c => c.email != null && c.email.Contains(_emailCliente));
                    clientes = miQuery.ToList();
                }

                // Filtro por estadoActivo
                var _estadoActivoCliente = filters.filtroEstadoActivo;
                
                if (_estadoActivoCliente.HasValue)
                {
                    miQuery = miQuery.Where(c => c.activo == _estadoActivoCliente.Value);
                    clientes = miQuery.ToList();
                }

                // Ordenamiento
                if (filters != null && !string.IsNullOrEmpty(filters.campoOrden))
                {
                    var esDescendente = filters.direccionOrden?.ToUpper() == "DESC";
                    
                    miQuery = filters.campoOrden.ToLower() switch
                    {
                        "nombre" => esDescendente ? miQuery.OrderByDescending(c => c.nombre) : miQuery.OrderBy(c => c.nombre),
                        "apellidos" => esDescendente ? miQuery.OrderByDescending(c => c.apellidos) : miQuery.OrderBy(c => c.apellidos),
                        "email" => esDescendente ? miQuery.OrderByDescending(c => c.email) : miQuery.OrderBy(c => c.email),
                        "telefono" => esDescendente ? miQuery.OrderByDescending(c => c.telefono) : miQuery.OrderBy(c => c.telefono),
                        "fechacreacion" => esDescendente ? miQuery.OrderByDescending(c => c.fechaCreacion) : miQuery.OrderBy(c => c.fechaCreacion),
                        _ => miQuery.OrderBy(c => c.idCliente)
                    };
                    clientes = miQuery.ToList();
                }
                else
                {
                    miQuery = miQuery.OrderBy(c => c.idCliente);
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

                string query = "SELECT idCliente, nombre, apellidos, email, password, telefono, fechaCreacion FROM tbClientes WHERE idCliente = @Id";
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
                                telefono = reader.IsDBNull(5) ? null : reader.GetString(5),
                                fechaCreacion = reader.GetDateTime(6)
                            };
                        }
                    }
                }
            }

            return cliente;
        }


        //Nos estamos planteando si sería conveniente devolver el id generado al insertar un nuevo cliente
        public async Task<int> AddAsync(Cliente cliente)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = "INSERT INTO tbClientes (nombre, apellidos, email, password, telefono, fechaCreacion, activo) " +
                               "VALUES (@Nombre, @Apellidos, @Email, @Password, @Telefono, @FechaCreacion, @Activo)";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Nombre", cliente.nombre ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Apellidos", cliente.apellidos ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Email", cliente.email ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Password", cliente.password ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Telefono", cliente.telefono ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@FechaCreacion", cliente.fechaCreacion ?? DateTime.Now);
                    command.Parameters.AddWithValue("@Activo", cliente.activo);

                    try
                    {
                        var idGenerado = await command.ExecuteScalarAsync();
                        return Convert.ToInt32(idGenerado);
                    }
                    catch (SqlException ex) when (ex.Number == 2627) // Violation of UNIQUE constraint
                    {
                        throw new InvalidOperationException($"Ya existe un cliente con el email '{cliente.email}'.", ex);
                    }
                }
            }
            
        }


        public async Task UpdateAsync(Cliente cliente)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = "UPDATE tbClientes SET nombre = @Nombre, apellidos = @Apellidos, email = @Email, " +
                               "password = @Password, telefono = @Telefono, fechaCreacion = @FechaCreacion, activo = @Activo " +
                               "WHERE idCliente = @Id";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", cliente.idCliente ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Nombre", cliente.nombre ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Apellidos", cliente.apellidos ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Email", cliente.email ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Password", cliente.password ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Telefono", cliente.telefono ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@FechaCreacion", cliente.fechaCreacion ?? DateTime.Now);
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


        }
    }
}