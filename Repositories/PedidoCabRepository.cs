using System.Data;
using Microsoft.Data.SqlClient;
using Reto_Desarrollo_Servidor_1ev.Models;

namespace Reto_Desarrollo_Servidor_1ev.Repositories
{
    public class PedidoCabRepository : IPedidoCabRepository
    {
        private readonly string _connectionString;

        public PedidoCabRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("SistemaPedidosDB") ?? "Not found";
        }

        
        //Método asíncrono para obtener todos los medios de pago de la base de datos
        public async Task<List<PedidoCab>> GetAllAsync(QueryParamsFilters? filters)
        {
            var pedidosCab = new List<PedidoCab>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                
                string query = "SELECT idPedido, idCliente, fechaPedido, idMedioPago, idTarjetaCredito, activo FROM tbPedidosCab";
                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var pedidoCab = new PedidoCab
                            {
                                idPedido = reader.GetInt32(0),
                                idCliente = reader.GetInt32(1),
                                fechaPedido = reader.GetDateTime(2),
                                idMedioPago = reader.GetInt32(3),
                                idTarjetaCredito = reader.GetInt32(4),
                                activo = reader.GetBoolean(5)
                            };

                            pedidosCab.Add(pedidoCab);
                        }
                    }
                }
                // Aplicación de filtros
                var miQuery = pedidosCab.AsQueryable();
                
                // Filtro por IdCliente
                var _filtroIdCliente = filters.filtroIdCliente ?? -1;
                if (_filtroIdCliente > 0)
                {
                    miQuery = miQuery.Where(p => p.idCliente == _filtroIdCliente);                 
                }

                // Filtro por IdMedioPago
                var _filtroIdMedioPago = filters.filtroIdMedioPago ?? -1;
                if (_filtroIdMedioPago > 0)
                {
                    miQuery = miQuery.Where(p => p.idMedioPago == _filtroIdMedioPago);                   
                }

                // Filtro por FechaPedidoDesde
                var _filtroFechaPedidoDesde = filters.filtroFechaPedidoDesde ?? null;
                if (_filtroFechaPedidoDesde.HasValue)
                {
                    miQuery = miQuery.Where(p => p.fechaPedido >= _filtroFechaPedidoDesde.Value);
                }

                // Filtro por FechaPedidoHasta
                var _filtroFechaPedidoHasta = filters.filtroFechaPedidoHasta;
                if (_filtroFechaPedidoHasta.HasValue)
                {
                    miQuery = miQuery.Where(p => p.fechaPedido <= _filtroFechaPedidoHasta.Value);
                }

                // Filtro por EstadoActivo
                var _estadoActivoPedidoCab = filters.filtroEstadoActivo;
                
                if (_estadoActivoPedidoCab.HasValue)
                {
                    miQuery = miQuery.Where(p => p.activo == _estadoActivoPedidoCab.Value);
                }

                // Ordenamiento
                if (filters != null && !string.IsNullOrEmpty(filters.campoOrden))
                {
                    var esDescendente = filters.direccionOrden?.ToUpper() == "DESC";
                    
                    miQuery = filters.campoOrden.ToLower() switch
                    {
                        "idcliente" => esDescendente ? miQuery.OrderByDescending(p => p.idCliente) : miQuery.OrderBy(p => p.idCliente),
                        "fechapedido" => esDescendente ? miQuery.OrderByDescending(p => p.fechaPedido) : miQuery.OrderBy(p => p.fechaPedido),
                        "mediopago" => esDescendente ? miQuery.OrderByDescending(p => p.idMedioPago) : miQuery.OrderBy(p => p.idMedioPago),
                        "tarjetacredito" => esDescendente ? miQuery.OrderByDescending(p => p.idTarjetaCredito) : miQuery.OrderBy(p => p.idTarjetaCredito),
                        _ => miQuery.OrderBy(c => c.idPedido)
                    };

                }
                else
                {
                    miQuery = miQuery.OrderBy(p => p.idPedido);
                }
                
                pedidosCab = miQuery.ToList();
            }

            return pedidosCab;
        }


        //No aplicado filtro de estadoActivo ya que se asume que se quiere obtener el medio de pago aunque esté inactivo
        public async Task<PedidoCab?> GetByIdAsync(int id)
        {
            PedidoCab? pedidoCab = null;

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = "SELECT idPedido, idCliente, fechaPedido, idMedioPago, idTarjetaCredito, activo FROM tbPedidosCab WHERE idPedido = @Id";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            pedidoCab = new PedidoCab
                            {
                                idPedido = id,
                                idCliente = reader.GetInt32(0),
                                fechaPedido = reader.GetDateTime(1),
                                idMedioPago = reader.GetInt32(2),
                                idTarjetaCredito = reader.GetInt32(3),
                                activo = reader.GetBoolean(4)
                            };
                        }
                    }
                }
            }

            return pedidoCab;
        }


        //Nos estamos planteando si sería conveniente devolver el id generado al insertar un nuevo medio de pago
        public async Task<int> AddAsync(PedidoCab pedidoCab)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = @"INSERT INTO tbPedidosCab (idCliente, fechaPedido, idMedioPago, idTarjetaCredito, activo) 
                        VALUES (@IdCliente, @FechaPedido, @IdMedioPago, @IdTarjetaCredito, @Activo);
                        SELECT CAST(SCOPE_IDENTITY() AS INT);";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdCliente", pedidoCab.idCliente ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@FechaPedido", pedidoCab.fechaPedido ?? DateTime.Now);
                    command.Parameters.AddWithValue("@IdMedioPago", pedidoCab.idMedioPago ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@IdTarjetaCredito", pedidoCab.idTarjetaCredito ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Activo", pedidoCab.activo);
                    
                    try
                    {
                        var idGenerado = await command.ExecuteScalarAsync();
                        return Convert.ToInt32(idGenerado);
                    }
                    catch (SqlException ex)
                    {
                        throw new InvalidOperationException($"Error al crear el pedido: {ex.Message}", ex);
                    }
                }
            }
            
        }
        

        public async Task UpdateAsync(PedidoCab pedidoCab)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = "UPDATE tbPedidosCab SET idCliente = @IdCliente, " +
                                "fechaPedido = @FechaPedido, idMedioPago = @IdMedioPago, " +
                                "idTarjetaCredito = @IdTarjetaCredito, activo = @Activo " +
                                "WHERE idMedioDePago = @Id";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdCliente", pedidoCab.idCliente ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@FechaPedido", pedidoCab.fechaPedido ?? DateTime.Now);
                    command.Parameters.AddWithValue("@IdMedioPago", pedidoCab.idMedioPago ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@IdTarjetaCredito", pedidoCab.idTarjetaCredito ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Activo", pedidoCab.activo);

                    await command.ExecuteNonQueryAsync();
                }
            }

        }

        //Aplicamos soft delete cambiando el estado del medio de pago a inactivo
        public async Task DeleteAsync(int id)
        {

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = "UPDATE tbPedidosCab SET activo = 0 WHERE idPedido = @Id";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    await command.ExecuteNonQueryAsync();
                }
            }


        }
    }
}