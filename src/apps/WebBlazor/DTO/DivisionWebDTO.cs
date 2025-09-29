using System;
using System.Collections.Generic;

namespace WebBlazor.DTO
{
    public class DivisionWebDTO
    {
        public string Nombre { get; set; } = string.Empty;
        public string Codigo { get; set; } = string.Empty;
        public string Region { get; set; } = string.Empty;
        public bool Activa { get; set; }
        public DateTime UltimoCambio { get; set; }
        public List<CampaniaDTO> Campanias { get; set; } = new();

        public DivisionWebDTO(string nombre, string codigo, string region,
                           bool activa, List<CampaniaDTO> campanias, DateTime ultimoCambio)
        {
            Nombre = nombre;
            Codigo = codigo;
            Region = region;
            Activa = activa;
            Campanias = campanias ?? new List<CampaniaDTO>();
            UltimoCambio = ultimoCambio;
        }

        public int TotalCampanias => Campanias?.Count ?? 0;
    }
}
