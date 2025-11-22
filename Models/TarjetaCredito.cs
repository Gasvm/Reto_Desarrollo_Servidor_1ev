using Models;
namespace Models;

public class TarjetaCredito {

    public int idTarjetaCredito {get;set;}
    public string? descripcion {get;set;}
    public string? numeroTarjeta {get;set;}
    public DateTime? fechaCaducidad {get;set;}
    public DateTime? fechaCreacion {get;set;}
    public bool activo {get;set;}


    public TarjetaCredito()
    {
        activo = true;
    }

    public TarjetaCredito(int _idTarjetaCredito, string _descripcion, string _numeroTarjeta, string _fechaCaducidad, DateTime _fechaCreacion )
    {
        idTarjetaCredito = _idTarjetaCredito;
        descripcion = _descripcion;
        numeroTarjeta = _numeroTarjeta;
        fechaCaducidad = _fechaCaducidad;
        fechaCreacion = _fechaCreacion;
        activo = true;
    }

    
}