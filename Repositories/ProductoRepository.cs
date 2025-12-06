using System.Data;
using Microsoft.Data.SqlClient;
using Reto_Desarrollo_Servidor_1ev.Models;

namespace Reto_Desarrollo_Servidor_1ev.Repositories
{
    public class ProductoRepository : IProductoRepository
    {
        private readonly string _connectionString;

        public ProductoRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("SistemaPedidosDB") ?? "Not found";
        }

        
        //Método asíncrono para obtener todos los medios de pago de la base de datos
        public async Task<List<Producto>> GetAllAsync(QueryParamsFilters? filters)
        {
            var productos = new List<Producto>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                
                string query = "SELECT idProducto, descripcion, precio, idTipoIVA, fechaCreacion FROM tbProductos";
                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var producto = new Producto
                            {
                                idProducto = reader.GetInt32(0),
                                descripcion = reader.GetString(1),
                                precio = reader.GetDecimal(2),
                                idTipoIVA = reader.GetInt32(3),
                                fechaCreacion = reader.GetDateTime(4)
                            };

                            productos.Add(producto);
                        }
                    }
                }
                // Aplicación de filtros
                var miQuery = productos.AsQueryable();

                // Filtro por DescripcionProducto
                var _filtroDescripcionProducto = filters.filtroDescripcionProducto ?? "";
                _filtroDescripcionProducto.AsQueryable();
                
                if (!string.IsNullOrEmpty(_filtroDescripcionProducto))
                {
                    miQuery = miQuery.Where(p => p.descripcion != null &&
                                            p.descripcion.ToString().Contains(_filtroDescripcionProducto, StringComparison.OrdinalIgnoreCase));
                    productos = miQuery.ToList();                    
                }
                // Filtro por PrecioMinimo
                var _filtroPrecioMinimo = filters.filtroPrecioMinimo;
                if (_filtroPrecioMinimo.HasValue)
                {
                    miQuery = miQuery.Where(p => p.precio >= _filtroPrecioMinimo.Value);
                    productos = miQuery.ToList();
                }
                // Filtro por PrecioMaximo
                var _filtroPrecioMaximo = filters.filtroPrecioMaximo;
                if (_filtroPrecioMaximo.HasValue)
                {
                    miQuery = miQuery.Where(p => p.precio <= _filtroPrecioMaximo.Value);
                    productos = miQuery.ToList();
                }
                // Filtro por IdTipoIVA
                var _filtroIdTipoIVA = filters.filtroIdTipoIVA;
                
                if (_filtroIdTipoIVA.HasValue)
                {
                    miQuery = miQuery.Where(p => p.idTipoIVA == _filtroIdTipoIVA.Value);
                    productos = miQuery.ToList();
                }
                
                // Filtro por EstadoActivo
                var _estadoActivoMedioDePago = filters.filtroEstadoActivo;
                
                if (_estadoActivoMedioDePago.HasValue)
                {
                    miQuery = miQuery.Where(p => p.activo == _estadoActivoMedioDePago.Value);
                    productos = miQuery.ToList();
                }

                // Ordenamiento
                if (filters != null && !string.IsNullOrEmpty(filters.campoOrden))
                {
                    var esDescendente = filters.direccionOrden?.ToUpper() == "DESC";
                    
                    miQuery = filters.campoOrden.ToLower() switch
                    {
                        "descripcion" => esDescendente ? miQuery.OrderByDescending(p => p.descripcion) : miQuery.OrderBy(p => p.descripcion),
                        "precio" => esDescendente ? miQuery.OrderByDescending(p => p.precio) : miQuery.OrderBy(p => p.precio),
                        "fechacreacion" => esDescendente ? miQuery.OrderByDescending(p => p.fechaCreacion) : miQuery.OrderBy(p => p.fechaCreacion),
                        _ => miQuery.OrderBy(p => p.idProducto)
                    };
                    productos = miQuery.ToList();
                }
                else
                {
                    miQuery = miQuery.OrderBy(p => p.idProducto);
                    productos = miQuery.ToList();
                }
                
            }

            return productos;
        }


        //No aplicado filtro de estadoActivo ya que se asume que se quiere obtener el producto aunque esté inactivo
        public async Task<Producto?> GetByIdAsync(int id)
        {
            Producto? producto = null;

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = "SELECT idProducto, descripcion, precio, idTipoIVA, fechaCreacion, activo FROM tbProductos WHERE idProducto = @Id";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            producto = new Producto
                            {
                                idProducto = reader.GetInt32(0),
                                descripcion = reader.GetString(1),
                                precio = reader.GetDecimal(2),
                                idTipoIVA = reader.GetInt32(3),
                                fechaCreacion = reader.GetDateTime(4),
                                activo = reader.GetBoolean(5)
                            };
                            
                        }
                    }
                }
            }

            return producto;
        }


        //Nos estamos planteando si sería conveniente devolver el id generado al insertar un nuevo medio de pago
        public async Task AddAsync(Producto producto)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = "INSERT INTO tbProductos (descripcion, precio, " +
                                "idTipoIVA, fechaCreacion, activo) " +
                               "VALUES (@Descripcion, @Precio, @IdTipoIVA, " +
                               "@FechaCreacion, @Activo)";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Descripcion", producto.descripcion ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Precio", producto.precio ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@IdTipoIVA", producto.idTipoIVA ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@FechaCreacion", producto.fechaCreacion ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Activo", producto.activo);
                    
                    await command.ExecuteNonQueryAsync();
                }
            }
            
        }
        

        public async Task UpdateAsync(Producto producto)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = "UPDATE tbProductos SET descripcion = @Descripcion, " +
                                "precio = @Precio, idTipoIVA = @IdTipoIVA, " +
                                "fechaCreacion = @FechaCreacion, activo = @Activo " +
                                "WHERE idProducto = @Id";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Descripcion", producto.descripcion ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Precio", producto.precio ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@IdTipoIVA", producto.idTipoIVA ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@FechaCreacion", producto.fechaCreacion ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Activo", producto.activo);
                    command.Parameters.AddWithValue("@Id", producto.idProducto);

                    await command.ExecuteNonQueryAsync();
                }
            }

        }

        //Aplicamos soft delete cambiando el estado del producto a inactivo
        public async Task DeleteAsync(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = "UPDATE tbProductos SET activo = 0 WHERE idProducto = @Id";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    await command.ExecuteNonQueryAsync();
                }
            }

        }
    }
}