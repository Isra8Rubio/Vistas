namespace WebBlazor.DTO
{
    public class MiembroDTO
    {
        public string Nombre { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty;
        public bool Activo { get; set; }

        public MiembroDTO(string nombre, string email, string rol, bool activo)
        {
            Nombre = nombre;
            Email = email;
            Rol = rol;
            Activo = activo;
        }
    }
}
