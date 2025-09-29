namespace WebBlazor.DTO
{
    public class ConversacionDTO
    {
        public string Contacto { get; set; } = string.Empty;

        public string Mensaje { get; set; } = string.Empty;

        public DateTime Fecha { get; set; }

        public int NoLeidos { get; set; }

        public ConversacionDTO(string nombre, string mensaje, DateTime fecha, int noLeidos)
        {
            Contacto = nombre;
            Mensaje = mensaje;
            Fecha = fecha;
            NoLeidos = noLeidos;
        }
    }
}
