namespace Reto_Desarrollo_Servidor_1ev.Models.DTOs
{
    public class TarjetaCreditoCreateDTO
    {
        public string Descripcion { get; set; }
        public string NumeroTarjeta { get; set; }
        public DateTime FechaCaducidad { get; set; }
        public int IdCliente { get; set; }
    }
}