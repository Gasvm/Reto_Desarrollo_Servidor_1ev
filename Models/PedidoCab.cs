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
        this.activo = true;
    }

    public PedidoCab(int _idPedido, int _idCliente, DateTime _fechaPedido, int _idMedioPago, int _idTarjetaCredito)
    {
        this.idPedido = _idPedido;
        this.idCliente = _idCliente;
        this.fechaPedido = _fechaPedido;
        this.idMedioPago = _idMedioPago;
        this.idTarjetaCredito = _idTarjetaCredito;
        this.activo = true;        
    }
    
    
}
