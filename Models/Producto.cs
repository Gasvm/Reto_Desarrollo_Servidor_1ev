using Models;
namespace Models;


public class Producto{

    public int? idProducto {get; set;}
    public string? descripcion {get; set;}
    public double? precio {get; set;}
    public int? idTipoIVA {get; set;}
    public DateTime? fechaCreacion  {get; set;}
    public bool activo {get; set;}


    public Producto()
    {
        activo = true;
    }

    public Producto(int _idProducto, string _descripcion, double _precio, int _idTipoIVA, DateTime _fechaCreacion)
    {
        idProducto = _idProducto;
        descripcion = _descripcion;
        precio = _precio;
        idTipoIVA = _idTipoIVA;
        fechaCreacion = _fechaCreacion;
        activo = true; 
        
    }
    
    
}
