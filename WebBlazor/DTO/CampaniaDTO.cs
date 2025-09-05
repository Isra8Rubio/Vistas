// WebBlazor/DTO/CampaniaDTO.cs
using System;

namespace WebBlazor.DTO
{
    public class CampaniaDTO
    {
        public string Titulo { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public string Canal { get; set; } = string.Empty;

        public DateTime Inicio { get; set; }
        public DateTime? Fin { get; set; }

        public CampaniaDTO(string titulo, string estado, string canal, DateTime inicio, DateTime? fin)
        {
            Titulo = titulo;
            Estado = estado;
            Canal = canal;
            Inicio = inicio;
            Fin = fin;
        }
    }
}
