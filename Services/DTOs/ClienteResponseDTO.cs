namespace Reto_Desarrollo_Servidor_1ev.Models.DTOs
{
    public class ClienteResponseDTO
    {
        public int IdCliente { get; set; }
        public string Nombre { get; set; }
        public string Apellidos { get; set; }
        public string Email { get; set; }
        public string? Telefono { get; set; }
        public DateTime FechaCreacion { get; set; }
        public bool Activo { get; set; }

    }
}