using System.Data;
using Microsoft.Data.SqlClient;
using Reto_Desarrollo_Servidor_1ev.Models;

namespace Reto_Desarrollo_Servidor_1ev.Repositories
{
    public class PedidoLinRepository : IPedidoLinRepository
    {
        private readonly string _connectionString;

        public PedidoLinRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("PedidosBD") ?? "Not found";
        }

        
        //Método asíncrono para obtener todos los medios de pago de la base de datos
        public async Task<List<PedidoLin>> GetAllAsync(QueryParamsFilters? filters)
        {
            var pedidosLin = new List<PedidoLin>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                
                string query = "SELECT idLineaPedido, idPedido, idProducto, precio, descuento, " + 
                "idTipoIVA, cantidad, totalLinea FROM tbPedidosLin";
                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var pedidoLin = new PedidoLin
                            {
                                idLineaPedido = reader.GetInt32(0),
                                idPedido = reader.GetInt32(1),
                                idProducto = reader.GetInt32(2),
                                precio = reader.GetDouble(3),
                                descuento = reader.GetDouble(4),
                                idTipoIVA = reader.GetInt32(5),
                                cantidad = reader.GetInt32(6),
                                totalLinea = reader.GetDouble(7)
                            };

                            pedidosLin.Add(pedidoLin);
                        }
                    }
                }
                // Aplicación de filtros
                // Filtro por IdPedido
                var _filtroIdPedido = filters.filtroIdPedido ?? -1;

                var miQuery = pedidosLin.AsQueryable();

                if (_filtroIdPedido >0)
                {
                    miQuery = miQuery.Where(p => p.idPedido == _filtroIdPedido);
                }

                // Filtro por filtroIdProducto
                var _filtroIdProducto = filters.filtroIdProducto ?? -1;
                
                if (_filtroIdProducto > 0)
                {
                    miQuery = miQuery.Where(p => p.idProducto == _filtroIdProducto);
                }

                // Filtro por EstadoActivo
                var _estadoActivoPedidoLin = filters.filtroEstadoActivo;
                
                if (_estadoActivoPedidoLin.HasValue)
                {
                    miQuery = miQuery.Where(p => p.activo == _estadoActivoPedidoLin.Value);
                }

                if (miQuery.Any())
                {
                    pedidosLin = miQuery.ToList();
                }
                
            }

            return pedidosLin;
        }


        //No aplicado filtro de estadoActivo ya que se asume que se quiere obtener la línea del pedido aunque esté inactiva
        public async Task<PedidoLin?> GetByIdAsync(int id)
        {
            PedidoLin? pedidoLin = null;

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = "SELECT idPedido, idProducto, precio, " + 
                "descuento, idTipoIVA, cantidad, totalLinea, activo "+
                "FROM tbPedidosLin WHERE idLineaPedido = @Id";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            pedidoLin = new PedidoLin
                            {
                                idLineaPedido = id,
                                idPedido = reader.GetInt32(0),
                                idProducto = reader.GetInt32(1),
                                precio = reader.GetDouble(2),
                                descuento = reader.GetDouble(3),
                                idTipoIVA = reader.GetInt32(4),
                                cantidad = reader.GetInt32(5),
                                totalLinea = reader.GetDouble(6),
                                activo = reader.GetBoolean(7)
                            };
                        }
                    }
                }
            }

            return pedidoLin;
        }


        //Nos estamos planteando si sería conveniente devolver el id generado al insertar un nuevo medio de pago
        public async Task AddAsync(PedidoLin pedidoLin)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = "INSERT INTO tbPedidosLin (idPedido, idProducto, " +
                                "precio, descuento, idTipoIVA, cantidad, totalLinea, activo) " +
                               "VALUES (@IdPedido, @IdProducto, @Precio, " +
                               "@Descuento, @IdTipoIVA, @Cantidad, @TotalLinea, @Activo)";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdPedido", pedidoLin.idPedido ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@IdProducto", pedidoLin.idProducto ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Precio", pedidoLin.precio ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Descuento", pedidoLin.descuento ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@IdTipoIVA", pedidoLin.idTipoIVA ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Cantidad", pedidoLin.cantidad ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@TotalLinea", pedidoLin.totalLinea ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Activo", pedidoLin.activo);
                    
                    await command.ExecuteNonQueryAsync();
                }
            }
            
        }
        

        public async Task UpdateAsync(PedidoLin pedidoLin)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = "UPDATE tbPedidosLin SET idPedido = @IdPedido, " +
                                "idProducto = @IdProducto, precio = @Precio, " +
                                "descuento = @Descuento, idTipoIVA = @IdTipoIVA, " +
                                "cantidad = @Cantidad, totalLinea = @TotalLinea, " +
                                "activo = @Activo " +
                                "WHERE idLineaPedido = @Id";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdPedido", pedidoLin.idPedido ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@IdProducto", pedidoLin.idProducto ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Precio", pedidoLin.precio ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Descuento", pedidoLin.descuento ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@IdTipoIVA", pedidoLin.idTipoIVA ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Cantidad", pedidoLin.cantidad ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@TotalLinea", pedidoLin.totalLinea ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Activo", pedidoLin.activo);
                    command.Parameters.AddWithValue("@Id", pedidoLin.idLineaPedido);

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

                string query = "UPDATE tbPedidosLin SET activo = 0 WHERE idLineaPedido = @Id";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    await command.ExecuteNonQueryAsync();
                }
            }


        }
    }
}