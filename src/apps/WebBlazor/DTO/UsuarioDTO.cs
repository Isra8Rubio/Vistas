using System;

namespace WebBlazor.DTO
{
    public class UsuarioDTO
    {
        public string Nombre { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Grupo { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty;
        public bool Activo { get; set; }
        public DateTime UltimoAcceso { get; set; }

        public UsuarioDTO(string nombre, string email, string grupo, string rol, bool activo, DateTime ultimoAcceso)
        {
            Nombre = nombre;
            Email = email;
            Grupo = grupo;
            Rol = rol;
            Activo = activo;
            UltimoAcceso = ultimoAcceso;
        }
    }
}
