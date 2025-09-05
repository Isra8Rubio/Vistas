using System;

namespace WebBlazor.DTO
{
    public class CampaniaListadoDTO
    {
        public string Titulo { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public string Canal { get; set; } = string.Empty;

        public DateTime Inicio { get; set; }
        public DateTime? Fin { get; set; }

        public string DivisionCodigo { get; set; } = string.Empty;
        public string DivisionNombre { get; set; } = string.Empty;

        public CampaniaListadoDTO(string titulo, string estado, string canal,
                                  DateTime inicio, DateTime? fin,
                                  string divisionCodigo, string divisionNombre)
        {
            Titulo = titulo;
            Estado = estado;
            Canal = canal;
            Inicio = inicio;
            Fin = fin;
            DivisionCodigo = divisionCodigo;
            DivisionNombre = divisionNombre;
        }
    }
}
