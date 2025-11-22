using Models;
namespace Models;


public class Cliente{

    public int? idCliente {get; set;}
    public string? nombre {get; set;}
    public string? apellidos {get; set;}
    public string? email {get; set;}
    public string? password {get; set;}
    public string? telefono {get; set;}
    public DateTime? fechaCreacion {get; set;}
    public bool activo {get; set;}
    public List<TarjetaCredito>? tarjetasCredito {get; set;}


    public Cliente()
    {
        activo = true;
    }

    public Cliente(int _idCliente, string _nombre, string _apellido, string _email, string _password, string _telefono, DateTime? _fechaCreacion, List<TarjetaCredito> _tarjetasCredito)
    {
        idCliente = _idCliente;
        nombre = _nombre;
        apellido = _apellido;
        email = _email;
        password = _password;
        telefono = _telefono;
        fechaCreacion = _fechaCreacion;
        activo = true;
        tarjetasCredito = _tarjetasCredito;
        
    }
    
    
}