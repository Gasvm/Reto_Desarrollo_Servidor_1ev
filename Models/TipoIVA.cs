using Models;
namespace Models;


public class TipoIVA{

    public int? idTipoIVA {get; set;}
    public string? descripcion {get; set;}
    public double? tasa {get; set;}
    public DateTime? fechaCreacion {get; set;}
    public bool activo {get; set;}
    

    public TipoIVA()
    {
        this.activo = true;
    }

    public TipoIVA(int _idTipoIVA, string _descripcion, double _tasa, DateTime _fechaCreacion)
    {
        this.idTipoIVA = _idTipoIVA;
        this.descripcion = _descripcion;
        this.tasa = _tasa;
        this.fechaCreacion = _fechaCreacion;
        this.activo = true;
        
        
    }
    
    
}
