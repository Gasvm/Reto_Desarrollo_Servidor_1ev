using Models;
namespace Models;

public class MedioDePago {

    public int? idMedioDePago {get;set;}
    public string? descripcion {get;set;}
    public DateTime? fechaCreacion {get;set;}
    public bool activo {get;set;}


    public MedioDePago()
    {
        this.activo = true;
    }

    public MedioDePago(int _idMedioDePago, string _descripcion, DateTime _fechaCreacion )
    {
        idMedioDePago=_idMedioDePago;
        descripcion=_descripcion;
        fechaCreacion=_fechaCreacion;
        this.activo = true;
    }


    
}