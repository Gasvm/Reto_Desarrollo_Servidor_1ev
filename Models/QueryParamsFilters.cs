using Models;

public class QueryParamsFilters {

    //Filtros genéricos para las consultas
    public bool? filtroEstadoActivo { get; set; }

    //Filtros específicos para clientes
    public string? filtroNombreCliente { get; set; }
    




    public string? filtroNombreProducto { get; set; }

    
    public string? filtronombreMedioDePago { get; set; }
    
    

    public QueryParamsFilters(){}

    
}