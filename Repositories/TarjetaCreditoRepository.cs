using System.Data;
using Microsoft.Data.SqlClient;
using Reto_Desarrollo_Servidor_1ev.Models;

namespace Reto_Desarrollo_Servidor_1ev.Repositories
{
    public class TarjetaCreditoRepository : ITarjetaCreditoRepository
    {
        private readonly string _connectionString;

        public TarjetaCreditoRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("PedidosBD") ?? "Not found";
        }

        
        //Método asíncrono para obtener todas las tarjetas de la base de datos
        public async Task<List<TarjetaCredito>> GetAllAsync(QueryParamsFilters? filters)
        {
            var tarjetasCredito = new List<TarjetaCredito>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                
                string query = "SELECT idTarjetaCredito, descripcion, numeroTarjeta, fechaCaducidad, fechaCreacion FROM tbTarjetasCredito";
                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var tarjetaCredito = new TarjetaCredito
                            {
                                idTarjetaCredito = reader.GetInt32(0),
                                descripcion = reader.GetString(1),
                                numeroTarjeta = reader.GetString(2),
                                fechaCaducidad = reader.GetDateTime(3),
                                fechaCreacion = reader.GetDateTime(4)
                            };

                            tarjetasCredito.Add(tarjetaCredito);
                        }
                    }
                }
                // Aplicación de filtros
                // Nota: El filtro idClienteTarjeta no se aplica porque TarjetaCredito no tiene idCliente
                // Este filtro sería más apropiado aplicarlo a través de la relación Cliente -> TarjetasCredito

                //Filtro por DescripcionTarjeta
                var _filtroDescripcionTarjeta = filters.filtroDescripcionTarjeta ?? "";
                _filtroDescripcionTarjeta.AsQueryable();
                var miQuery = tarjetasCredito.AsQueryable();
                if (!string.IsNullOrEmpty(_filtroDescripcionTarjeta))
                {
                    miQuery = miQuery.Where(t => t.descripcion != null &&
                                            t.descripcion.ToString().Contains(_filtroDescripcionTarjeta, StringComparison.OrdinalIgnoreCase));                    
                }
                // Filtro por NumeroTarjeta
                var _filtroNumeroTarjeta = filters.filtroNumeroTarjeta ?? "";
                if (!string.IsNullOrEmpty(_filtroNumeroTarjeta))
                {
                    miQuery = miQuery.Where(t => t.numeroTarjeta != null &&
                                            t.numeroTarjeta.ToString().Contains(_filtroNumeroTarjeta, StringComparison.OrdinalIgnoreCase));                    
                }
                // Filtro por FechaCaducidadDesde
                var _filtroFechaCaducidadDesde = filters.filtroFechaCaducidadDesde;
                if (_filtroFechaCaducidadDesde.HasValue)
                {
                    miQuery = miQuery.Where(t => t.fechaCaducidad >= _filtroFechaCaducidadDesde.Value);
                }
                // Filtro por FechaCaducidadHasta
                var _filtroFechaCaducidadHasta = filters.filtroFechaCaducidadHasta;
                if (_filtroFechaCaducidadHasta.HasValue)
                {
                    miQuery = miQuery.Where(t => t.fechaCaducidad <= _filtroFechaCaducidadHasta.Value);
                }
                // Filtro por EstadoActivo
                var _estadoActivoTarjeta = filters.filtroEstadoActivo;
                if (_estadoActivoTarjeta.HasValue)
                {
                    miQuery = miQuery.Where(t => t.activo == _estadoActivoTarjeta.Value);
                }
                if (miQuery.Any())
                {
                    tarjetasCredito = miQuery.ToList();
                }            
                
            }

            return tarjetasCredito;
        }


        //No aplicado filtro de estadoActivo ya que se asume que se quiere obtener la tarjeta de crédito aunque esté inactiva
        public async Task<TarjetaCredito?> GetByIdAsync(int id)
        {
            TarjetaCredito? tarjetaCredito = null;

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = "SELECT idTarjetaCredito, descripcion, numeroTarjeta, fechaCaducidad, fechaCreacion, activo FROM tbTarjetasCredito WHERE idTarjetaCredito = @Id";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            tarjetaCredito = new TarjetaCredito
                            {
                                idTarjetaCredito = id,
                                descripcion = reader.GetString(0),
                                numeroTarjeta = reader.GetString(1),
                                fechaCaducidad = reader.GetDateTime(2),
                                fechaCreacion = reader.GetDateTime(3),
                                activo = reader.GetBoolean(4)
                            };
                            
                        }
                    }
                }
            }

            return tarjetaCredito;
        }


        //Nos estamos planteando si sería conveniente devolver el id generado al insertar un nuevo medio de pago
        public async Task AddAsync(TarjetaCredito tarjetaCredito)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = "INSERT INTO tbTarjetasCredito (descripcion, numeroTarjeta, " +
                                "fechaCaducidad, fechaCreacion, activo) " +
                               "VALUES (@Descripcion, @NumeroTarjeta, @FechaCaducidad, " +
                               "@FechaCreacion, @Activo,)";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Descripcion", tarjetaCredito.descripcion ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@NumeroTarjeta", tarjetaCredito.numeroTarjeta ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@FechaCaducidad", tarjetaCredito.fechaCaducidad ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@FechaCreacion", tarjetaCredito.fechaCreacion ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Activo", tarjetaCredito.activo);
                    
                    await command.ExecuteNonQueryAsync();
                }
            }
            
        }
        

        public async Task UpdateAsync(TarjetaCredito tarjetaCredito)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = "UPDATE tbTarjetasCredito SET descripcion = @Descripcion, " +
                                "numeroTarjeta = @NumeroTarjeta, fechaCaducidad = @FechaCaducidad, " +
                                "fechaCreacion = @FechaCreacion, activo = @Activo " +
                                "WHERE idTarjetaCredito = @Id";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Descripcion", tarjetaCredito.descripcion ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@NumeroTarjeta", tarjetaCredito.numeroTarjeta ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@FechaCaducidad", tarjetaCredito.fechaCaducidad ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@FechaCreacion", tarjetaCredito.fechaCreacion ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Activo", tarjetaCredito.activo);
                    command.Parameters.AddWithValue("@Id", tarjetaCredito.idTarjetaCredito);
                    await command.ExecuteNonQueryAsync();
                }
            }

        }

        //Aplicamos soft delete cambiando el estado de la tarjeta de crédito a inactivo
        public async Task DeleteAsync(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = "UPDATE tbTarjetasCredito SET activo = 0 WHERE idTarjetaCredito = @Id";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    await command.ExecuteNonQueryAsync();
                }
            }

        }
    }
}