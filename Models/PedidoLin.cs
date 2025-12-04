namespace Reto_Desarrollo_Servidor_1ev.Models;

public class PedidoLin{

    public int? idLineaPedido {get; set;}
    public int? idPedido {get; set;}
    public int? idProducto {get; set;}
    public decimal? precio {get; set;}
    public decimal? descuento {get; set;}
    public int? idTipoIVA {get; set;}
    public int? cantidad {get; set;}
    public decimal? totalLinea {get; set;}
    public bool activo {get; set;}
    


    public PedidoLin()
    {
        activo = true;
    }

    public PedidoLin(int _idLineaPedido, int _idPedido, int _idProducto, decimal _precio, decimal _descuento, int _tipoIVA)
    {
        idLineaPedido = _idLineaPedido;
        idPedido = _idPedido;
        idProducto = _idProducto;
        precio = _precio;
        descuento = _descuento;
        idTipoIVA = _tipoIVA;

        
    }
    
    
}
