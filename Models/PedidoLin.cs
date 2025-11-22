using Models;
namespace Models;


public class PedidoLin{

    public int? idLineaPedido {get; set;}
    public int? idPedido {get; set;}
    public int? idProducto {get; set;}
    public double? precio {get; set;}
    public double? descuento {get; set;}
    public int? idTipoIVA {get; set;}
    public int? cantidad {get; set;}
    public double? totalLinea {get; set;}
    public bool activo {get; set;}
    


    public PedidoLin()
    {
        activo = true;
    }

    public PedidoLin(int _idLineaPedido, int _idPedido, int _idProducto, double _precio, double _descuento, int _tipoIVA)
    {
        idLineaPedido = _idLineaPedido;
        idPedido = _idPedido;
        idProducto = _idProducto;
        precio = _precio;
        descuento = _descuento;
        idTipoIVA = _tipoIVA;

        
    }
    
    
}
