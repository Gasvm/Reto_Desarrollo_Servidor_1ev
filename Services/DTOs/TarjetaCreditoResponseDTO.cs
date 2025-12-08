namespace Reto_Desarrollo_Servidor_1ev.Models.DTOs
{
    public class TarjetaCreditoResponseDTO
    {
        public int IdTarjetaCredito { get; set; }
        public string Descripcion { get; set; }
        public string NumeroTarjetaEnmascarado { get; set; }
        public DateTime FechaCaducidad { get; set; }
        public int IdCliente { get; set; }
        public string NombreCliente { get; set; }
        public string ApellidosCliente { get; set; }
        public bool Activo { get; set; }
    }
}