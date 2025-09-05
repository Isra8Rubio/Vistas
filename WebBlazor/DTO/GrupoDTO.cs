using System;
using System.Collections.Generic;

namespace WebBlazor.DTO
{
    public class GrupoDTO
    {
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public List<MiembroDTO> Miembros { get; set; } = new();
        public DateTime UltimoCambio { get; set; }

        public GrupoDTO(string nombre, string? descripcion, List<MiembroDTO> miembros, DateTime ultimoCambio)
        {
            Nombre = nombre;
            Descripcion = descripcion;
            Miembros = miembros ?? new List<MiembroDTO>();
            UltimoCambio = ultimoCambio;
        }

        // Útil para el DataGrid (Field="MiembrosCount" y ordenación)
        public int MiembrosCount => Miembros?.Count ?? 0;
    }
}
