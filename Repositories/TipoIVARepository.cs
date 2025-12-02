using System.Data;
using Microsoft.Data.SqlClient;
using Reto_Desarrollo_Servidor_1ev.Models;

namespace Reto_Desarrollo_Servidor_1ev.Repositories
{
    public class TipoIVARepository : ITipoIVARepository
    {
        private readonly string _connectionString;

        public TipoIVARepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("PedidosBD") ?? "Not found";
        }

        
        //Método asíncrono para obtener todas las tarjetas de la base de datos
        public async Task<List<TipoIVA>> GetAllAsync(
                    QueryParamsFilters? tasaMinima, 
                    QueryParamsFilters? tasaMaxima, 
                    QueryParamsFilters? descripcionTipoIVA, 
                    QueryParamsFilters? fechaCreacionDesde,
                    QueryParamsFilters? fechaCreacionHasta, 
                    QueryParamsFilters? estadoActivo
                    )
        {
            var tiposIVA = new List<TipoIVA>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                
                string query = "SELECT idTipoIVA, descripcion, tasa, fechaCreacion FROM tbTiposIVA";
                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var tipoIVA = new TipoIVA
                            {
                                idTipoIVA = reader.GetInt32(0),
                                descripcion = reader.GetString(1),
                                tasa = reader.GetDouble(2),
                                fechaCreacion = reader.GetDateTime(3)
                            };

                            tiposIVA.Add(tipoIVA);
                        }
                    }
                }
                // Aplicación de filtros
                //Filtro por TasaMinima
                var _filtroTasaMinima = tasaMinima?.filtroTasaMinima;
                if (_filtroTasaMinima.HasValue)
                {
                    tiposIVA = tiposIVA
                                    .Where(t => t.tasa >= _filtroTasaMinima.Value)
                                    .ToList();
                }
                //Filtro por TasaMaxima
                var _filtroTasaMaxima = tasaMaxima?.filtroTasaMaxima;
                if (_filtroTasaMaxima.HasValue)
                {
                    tiposIVA = tiposIVA
                                    .Where(t => t.tasa <= _filtroTasaMaxima.Value)
                                    .ToList();
                }
                //Filtro por DescripcionTipoIVA
                var _filtroDescripcionTipoIVA = descripcionTipoIVA?.filtroDescripcionTipoIVA ?? "";
                _filtroDescripcionTipoIVA.AsQueryable();
                var miQuery = tiposIVA.AsQueryable();
                if (!string.IsNullOrEmpty(_filtroDescripcionTipoIVA))
                {
                    miQuery = miQuery.Where(t => t.descripcion != null &&
                                            t.descripcion.ToString().Contains(_filtroDescripcionTipoIVA, StringComparison.OrdinalIgnoreCase));                    
                }
                // Filtro por FechaCreacionDesde
                var _filtroFechaCreacionDesde = fechaCreacionDesde?.filtroFechaCreacionDesde;
                if (_filtroFechaCreacionDesde.HasValue)
                {
                    miQuery = miQuery.Where(t => t.fechaCreacion >= _filtroFechaCreacionDesde.Value);
                }
                // Filtro por FechaCreacionHasta
                var _filtroFechaCreacionHasta = fechaCreacionHasta?.filtroFechaCreacionHasta;
                if (_filtroFechaCreacionHasta.HasValue)
                {
                    miQuery = miQuery.Where(t => t.fechaCreacion <= _filtroFechaCreacionHasta.Value);
                }
                // Filtro por EstadoActivo
                var _estadoActivoTipoIVA = estadoActivo?.filtroEstadoActivo;
                if (_estadoActivoTipoIVA.HasValue)
                {
                    miQuery = miQuery.Where(t => t.activo == _estadoActivoTipoIVA.Value);
                }
                if (miQuery.Any())
                {
                    tiposIVA = miQuery.ToList();
                }            
                
            }

            return tiposIVA;
        }


        //No aplicado filtro de estadoActivo ya que se asume que se quiere obtener la tarjeta de crédito aunque esté inactiva
        public async Task<TipoIVA?> GetByIdAsync(int id)
        {
            TipoIVA? tipoIVA = null;

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = "SELECT idTipoIVA, descripcion, tasa, fechaCreacion, activo FROM tbTiposIVA WHERE idTipoIVA = @Id";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            tipoIVA = new TipoIVA
                            {
                                idTipoIVA = id,
                                descripcion = reader.GetString(0),
                                tasa = reader.GetDouble(1),
                                fechaCreacion = reader.GetDateTime(2),
                                activo = reader.GetBoolean(3)
                            };
                            
                        }
                    }
                }
            }

            return tipoIVA;
        }


        //Nos estamos planteando si sería conveniente devolver el id generado al insertar un nuevo tipo de IVA
        public async Task AddAsync(TipoIVA tipoIVA)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = "INSERT INTO tbTiposIVA (descripcion, tasa, " +
                                "fechaCreacion, activo) " +
                               "VALUES (@Descripcion, @Tasa, " +
                               "@FechaCreacion, @Activo,)";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Descripcion", tipoIVA.descripcion ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Tasa", tipoIVA.tasa ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@FechaCreacion", tipoIVA.fechaCreacion ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Activo", tipoIVA.activo);
                    
                    await command.ExecuteNonQueryAsync();
                }
            }
            
        }
        

        public async Task UpdateAsync(TipoIVA tipoIVA)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = "UPDATE tbTiposIVA SET descripcion = @Descripcion, " +
                                "tasa = @Tasa, " +
                                "fechaCreacion = @FechaCreacion, activo = @Activo " +
                                "WHERE idTipoIVA = @Id";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Descripcion", tipoIVA.descripcion ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Tasa", tipoIVA.tasa ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@FechaCreacion", tipoIVA.fechaCreacion ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Activo", tipoIVA.activo);
                    command.Parameters.AddWithValue("@Id", tipoIVA.idTipoIVA);

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

                string query = "UPDATE tbTiposIVA SET activo = 0 WHERE idTipoIVA = @Id";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    await command.ExecuteNonQueryAsync();
                }
            }

        }
    }
}