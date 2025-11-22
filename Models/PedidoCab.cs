using Models;
namespace Models;


public class PedidoCab{

    public int? idPedido {get; set;}
    public int? idCliente {get; set;}
    public DateTime? fechaPedido {get; set;}
    public int? idMedioPago {get; set;}
    public int? idTarjetaCredito {get; set;}
    public bool activo {get; set;}
    


    public PedidoCab()
    {
        activo = true;
    }

    public PedidoCab(int _idPedido, int _idCliente, DateTime _fechaPedido, int _idMedioPago, int _idTarjetaCredito)
    {
        idPedido = _idPedido;
        idCliente = _idCliente;
        fechaPedido = _fechaPedido;
        idMedioPago = _idMedioPago;
        idTarjetaCredito = _idTarjetaCredito;
        activo = true;        
    }
    
    
}
