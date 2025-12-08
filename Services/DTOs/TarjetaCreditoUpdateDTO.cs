namespace Reto_Desarrollo_Servidor_1ev.Models.DTOs
{
    public class TarjetaCreditoUpdateDTO
    {
        public int IdTarjetaCredito { get; set; }
        public string Descripcion { get; set; }
        public string? NumeroTarjeta { get; set; } // Opcional en update
        public DateTime FechaCaducidad { get; set; }
        public int IdCliente { get; set; }
        public bool Activo { get; set; }
    }
}