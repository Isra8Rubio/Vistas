using System;
using System.Collections.Generic;

namespace Api.DTO
{
    public class GrupoDTO
    {
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public List<MiembroDTO> Miembros { get; set; } = new();
        public DateTime UltimoCambio { get; set; }

        // Útil para la columna y ordenación en el grid
        public int MiembrosCount => Miembros?.Count ?? 0;

        public GrupoDTO(string nombre, string? descripcion, List<MiembroDTO> miembros, DateTime ultimoCambio)
        {
            Nombre = nombre;
            Descripcion = descripcion;
            Miembros = miembros ?? new List<MiembroDTO>();
            UltimoCambio = ultimoCambio;
        }
    }
}
