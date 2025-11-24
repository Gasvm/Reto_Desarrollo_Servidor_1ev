using System.Data;
using Micorosoft.Data.SqlClient;

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
        public async Task<List<PedidoLin>> GetAllAsync(
                    QueryParamsFilters? IdPedido, 
                    QueryParamsFilters? IdProducto, 
                    QueryParamsFilters? EstadoActivo
                    )
        {
            var pedidosLin = new List<PedidoLin>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                
                string query = "SELECT idLineaPedido, idPedido, idProducto, precio, descuento, " + 
                "idTipoIVA, cantidad, totalLinea FROM tbPedidoLin";
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
                // Filtro por IdCliente
                var _filtroIdCliente = IdPedido.filtroIdCliente;

                var miQuery = pedidosLin.AsQueryable();

                if (_filtroIdCliente.HasValue)
                {
                    miQuery = miQuery.Where(p => p.idCliente == _filtroIdCliente.Value);
                }

                // Filtro por filtroIdProducto
                var _filtroIdProducto = IdProducto.filtroIdProducto;
                
                if (_filtroIdProducto.HasValue)
                {
                    miQuery = miQuery.Where(p => p.idProducto == _filtroIdProducto.Value);
                }

                // Filtro por EstadoActivo
                var _estadoActivoPedidoLin = EstadoActivo.filtroEstadoActivo;
                
                if (_estadoActivoMedioDePago.HasValue)
                {
                    miQuery = miQuery.Where(p => p.activo == _estadoActivoMedioDePago.Value);
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
                "FROM tbPedidoLin WHERE idLineaPedido = @Id";
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

                string query = "INSERT INTO tbPedidoLin (idCliente, fechaPedido, " +
                                "idMedioPago, idTarjetaCredito, activo) " +
                               "VALUES (@IdCliente, @FechaPedido, @IdMedioPago, " +
                               "@IdTarjetaCredito, @Activo,)";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdCliente", pedidoLin.idCliente ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@FechaPedido", pedidoLin.fechaPedido ?? DateTime.Now);
                    command.Parameters.AddWithValue("@IdMedioPago", pedidoLin.idMedioPago ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@IdTarjetaCredito", pedidoLin.idTarjetaCredito ?? (object)DBNull.Value);
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

                string query = "UPDATE tbPedidoLin SET idPedido = @IdPedido, " +
                                "idProducto = @IdProducto, precio = @Precio, " +
                                "descuento = @Descuento, idTipoIVA = @IdTipoIVA, " +
                                "cantidad = @Cantidad, totalLinea = @TotalLinea " +
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
                    command.Parameters.AddWithValue("@Activo", pedidoLin.activo ?? (object)DBNull.Value);
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

                string query = "UPDATE tbPedidoLin SET activo = 0 WHERE idLineaPedido = @Id";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    await command.ExecuteNonQueryAsync();
                }
            }


        }
    }
}