namespace Reto_Desarrollo_Servidor_1ev.Models;

public class TipoIVA{

    public int? idTipoIVA {get; set;}
    public string? descripcion {get; set;}
    public decimal? tasa {get; set;}
    public DateTime? fechaCreacion {get; set;}
    public bool activo {get; set;}
    

    public TipoIVA()
    {
        activo = true;
    }

    public TipoIVA(int _idTipoIVA, string _descripcion, decimal _tasa, DateTime _fechaCreacion)
    {
        idTipoIVA = _idTipoIVA;
        descripcion = _descripcion;
        tasa = _tasa;
        fechaCreacion = _fechaCreacion;
        activo = true;
        
        
    }
    
    
}
