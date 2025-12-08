namespace Reto_Desarrollo_Servidor_1ev.Models.DTOs
{
    public class LineaPedidoDTO
    {
        public int IdLineaPedido { get; set; }
        public int IdProducto { get; set; }
        public string DescripcionProducto { get; set; }
        public decimal PrecioUnitario { get; set; }
        public int Cantidad { get; set; }
        public decimal Descuento { get; set; }
        public int IdTipoIVA { get; set; }
        public string DescripcionTipoIVA { get; set; }
        public decimal TasaIVA { get; set; }
        
        // Campos calculados
        public decimal SubtotalLinea { get; set; }  // precio * cantidad
        public decimal DescuentoAplicado { get; set; }  // subtotal * (descuento/100)
        public decimal BaseImponible { get; set; }  // subtotal - descuento
        public decimal ImporteIVA { get; set; }  // baseImponible * (tasaIVA/100)
        public decimal TotalLinea { get; set; }  // baseImponible + importeIVA
    }
}