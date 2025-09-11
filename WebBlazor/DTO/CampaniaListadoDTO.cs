namespace WebBlazor.DTO
{
    public class CampaniaListadoDTO
    {
        public string Titulo { get; set; } = "";
        public string Estado { get; set; } = "";
        public string Canal { get; set; } = "";
        public DateTime Inicio { get; set; }
        public DateTime? Fin { get; set; }
        public string DivisionCodigo { get; set; } = "";
        public string DivisionNombre { get; set; } = "";

        public CampaniaListadoDTO() { }

        public CampaniaListadoDTO(
            string titulo,
            string estado,
            string canal,
            DateTime inicio,
            DateTime? fin,
            string divisionCodigo,
            string divisionNombre)
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
