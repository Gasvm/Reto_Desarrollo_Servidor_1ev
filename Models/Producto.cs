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
        this.activo = true;
    }

    public Producto(int _idProducto, string _descripcion, double _precio, int _idTipoIVA, DateTime _fechaCreacion)
    {
        this.idProducto = _idProducto;
        this.descripcion = _descripcion;
        this.precio = _precio;
        this.idTipoIVA = _idTipoIVA;
        this.fechaCreacion = _fechaCreacion;
        this.activo = true; 
        
    }
    
    
}
