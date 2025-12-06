using System.Data;
using Microsoft.Data.SqlClient;
using Reto_Desarrollo_Servidor_1ev.Models;

namespace Reto_Desarrollo_Servidor_1ev.Repositories
{
    public class MedioDePagoRepository : IMedioDePagoRepository
    {
        private readonly string _connectionString;

        public MedioDePagoRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("SistemaPedidosDB") ?? "Not found";
        }

        
        //Método asíncrono para obtener todos los medios de pago de la base de datos
        public async Task<List<MedioDePago>> GetAllAsync(QueryParamsFilters? filters)
        {
            var mediosDePago = new List<MedioDePago>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                
                string query = "SELECT idMedioDePago, descripcion, fechaCreacion, activo FROM tbMediosDePago";
                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var medioDePago = new MedioDePago
                            {
                                idMedioDePago = reader.GetInt32(0),
                                descripcion = reader.GetString(1),
                                fechaCreacion = reader.GetDateTime(2),
                                activo = reader.GetBoolean(3)
                            };

                            mediosDePago.Add(medioDePago);
                        }
                    }
                }
                
                // Aplicar filtros si se proporcionan
                var miQuery = mediosDePago.AsQueryable();

                // Filtro por descripción
                var _descripcionMedioDePago = filters.filtroDescripcionMedioDePago ?? "";
                
                if (!string.IsNullOrEmpty(_descripcionMedioDePago))
                {
                    miQuery = miQuery.Where(m => m.descripcion != null &&
                                            m.descripcion.Contains(_descripcionMedioDePago));                    
                }

                // Filtro por estadoActivo
                var _estadoActivoMedioDePago = filters.filtroEstadoActivo;
                
                if (_estadoActivoMedioDePago.HasValue)
                {
                    miQuery = miQuery.Where(m => m.activo == _estadoActivoMedioDePago.Value);
                    mediosDePago = miQuery.ToList();
                }

                // Ordenamiento
                if (filters != null && !string.IsNullOrEmpty(filters.campoOrden))
                {
                    var esDescendente = filters.direccionOrden?.ToUpper() == "DESC";
                    
                    miQuery = filters.campoOrden.ToLower() switch
                    {
                        "descripcion" => esDescendente ? miQuery.OrderByDescending(m => m.descripcion) : miQuery.OrderBy(m => m.descripcion),
                        "fechacreacion" => esDescendente ? miQuery.OrderByDescending(m => m.fechaCreacion) : miQuery.OrderBy(m => m.fechaCreacion),
                        _ => miQuery.OrderBy(m => m.idMedioDePago)
                    };
                }
                else
                {
                    miQuery = miQuery.OrderBy(m => m.idMedioDePago);
                    mediosDePago = miQuery.ToList();
                }
                mediosDePago = miQuery.ToList();
            }


            return mediosDePago;
        }


        //No aplicado filtro de estadoActivo ya que se asume que se quiere obtener el medio de pago aunque esté inactivo
        public async Task<MedioDePago?> GetByIdAsync(int id)
        {
            MedioDePago? medioDePago = null;

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = "SELECT idMedioDePago, descripcion, fechaCreacion FROM tbMediosDePago WHERE idMedioDePago = @Id";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            medioDePago = new MedioDePago
                            {
                                idMedioDePago = reader.GetInt32(0),
                                descripcion = reader.GetString(1),
                                fechaCreacion = reader.GetDateTime(2)
                            };
                        }
                    }
                }
            }

            return medioDePago;
        }


        //Nos estamos planteando si sería conveniente devolver el id generado al insertar un nuevo medio de pago
        public async Task AddAsync(MedioDePago medioDePago)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = "INSERT INTO tbMediosDePago (descripcion, fechaCreacion, activo) " +
                               "VALUES (@Descripcion, @FechaCreacion, @Activo)";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Descripcion", medioDePago.descripcion ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@FechaCreacion", medioDePago.fechaCreacion ?? DateTime.Now);
                    command.Parameters.AddWithValue("@Activo", medioDePago.activo);

                    await command.ExecuteNonQueryAsync();
                }
            }
            
        }
        

        public async Task UpdateAsync(MedioDePago medioDePago)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = "UPDATE tbMediosDePago SET descripcion = @Descripcion, fechaCreacion = @FechaCreacion, activo = @Activo " +
                               "WHERE idMedioDePago = @Id";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", medioDePago.idMedioDePago ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Descripcion", medioDePago.descripcion ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@FechaCreacion", medioDePago.fechaCreacion ?? DateTime.Now);
                    command.Parameters.AddWithValue("@Activo", medioDePago.activo);

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

                string query = "UPDATE tbMediosDePago SET activo = 0 WHERE idMedioDePago = @Id";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    await command.ExecuteNonQueryAsync();
                }
            }


        }
    }
}