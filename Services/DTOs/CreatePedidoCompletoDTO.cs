namespace Reto_Desarrollo_Servidor_1ev.Services.DTOs
{
    public class CreatePedidoCompletoDTO
    {
        public int IdCliente { get; set; }
        public int IdMedioPago { get; set; }
        public int? IdTarjetaCredito { get; set; }
        
        public List<CreateLineaPedidoDTO> Lineas { get; set; }
        
        public CreatePedidoCompletoDTO()
        {
            Lineas = new List<CreateLineaPedidoDTO>();
        }
    }
    
    public class CreateLineaPedidoDTO
    {
        public int IdProducto { get; set; }
        public int Cantidad { get; set; }
        public decimal Descuento { get; set; } = 0;
    }
}