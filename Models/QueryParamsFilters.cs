using Models;

public class QueryParamsFilters {

    //Filtros genéricos para las consultas
    public bool? filtroEstadoActivo { get; set; }

    //Filtros específicos para clientes
    public string? filtroNombreCliente { get; set; }
    

    //Filtros específicos para medios de pago
    public string? filtroDescripcionMedioDePago { get; set; }


    //Filtros específicos para Cabeceras de Pedido
    public int? filtroIdCliente { get; set; }
    public DateTime? filtroFechaPedidoDesde { get; set; }
    public DateTime? filtroFechaPedidoHasta { get; set; }
    public int? filtroIdMedioPago { get; set; }

    //Filtros específicos para Líneas de Pedido
    public int? filtroIdPedido { get; set; }
    public int? filtroIdProducto { get; set; }


    //Filtros específicos para productos
    public string? filtroDescripcionProducto { get; set; }
    public double? filtroPrecioMinimo { get; set; }
    public double? filtroPrecioMaximo { get; set; }
    public int? filtroIdTipoIVA { get; set; }

    //Filtros específicos para Tarjetas de Crédito
    public int? filtroIdClienteTarjeta { get; set; }
    public string? filtroDescripcionTarjeta { get; set; }
    public string? filtroNumeroTarjeta { get; set; }
    public DateTime? filtroFechaCaducidadDesde { get; set; }
    public DateTime? filtroFechaCaducidadHasta { get; set; }


    //Filtros específicos para tipos de IVA
    public double? filtroTasaMinima { get; set; }
    public double? filtroTasaMaxima { get; set; }
    public string? filtroDescripcionTipoIVA { get; set; }
    public DateTime? filtroFechaCreacionDesde { get; set; }
    public DateTime? filtroFechaCreacionHasta { get; set; }


    public QueryParamsFilters(){}

    
}