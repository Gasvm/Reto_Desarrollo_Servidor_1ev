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
        this.activo = true;
    }

    public PedidoLin(int _idLineaPedido, int _idPedido, int _idProducto, double _precio, double _descuento, int _tipoIVA)
    {
        this.idLineaPedido = _idLineaPedido;
        this.idPedido = _idPedido;
        this.idProducto = _idProducto;
        this.precio = _precio;
        this.descuento = _descuento;
        this.idTipoIVA = _tipoIVA;

        
    }
    
    
}
