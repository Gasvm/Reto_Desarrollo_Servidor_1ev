namespace Reto_Desarrollo_Servidor_1ev.Models.DTOs
{
    public class PedidoCompletoDTO
    {
        // Información del Pedido
        public int IdPedido { get; set; }
        public DateTime FechaPedido { get; set; }
        public bool Activo { get; set; }
        
        // Información del Cliente
        public int IdCliente { get; set; }
        public string NombreCliente { get; set; }
        public string ApellidosCliente { get; set; }
        public string EmailCliente { get; set; }
        public string TelefonoCliente { get; set; }
        
        // Información del Medio de Pago
        public int IdMedioPago { get; set; }
        public string DescripcionMedioPago { get; set; }
        
        // Información de la Tarjeta (opcional)
        public int? IdTarjetaCredito { get; set; }
        public string DescripcionTarjeta { get; set; }
        public string NumeroTarjetaEnmascarado { get; set; }
        
        // Líneas del Pedido
        public List<LineaPedidoDTO> Lineas { get; set; }
        
        // Totales Calculados
        public decimal SubtotalPedido { get; set; }
        public decimal DescuentoTotal { get; set; }
        public decimal BaseImponibleTotal { get; set; }
        public decimal IVATotal { get; set; }
        public decimal TotalPedido { get; set; }
        
        public PedidoCompletoDTO()
        {
            Lineas = new List<LineaPedidoDTO>();
        }
    }
}