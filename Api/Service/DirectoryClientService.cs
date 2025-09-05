using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Api.DTO;

namespace Api.Services
{
    /// <summary>
    /// Servicio “fake” que simula una API externa.
    /// Devuelve datos en memoria y soporta búsqueda, orden y paginación.
    /// </summary>
    public class DirectoryClientService
    {
        private readonly ILogger<DirectoryClientService> _logger;

        // ===== Datos falsos =====
        private readonly List<UsuarioDTO> _users;
        private readonly List<GrupoDTO> _groups;

        public DirectoryClientService(ILogger<DirectoryClientService> logger)
        {
            _logger = logger;

            _users = new()
            {
                new("Ana Pérez",       "ana.perez@contoso.com",      "Marketing", "Editor",       true,  DateTime.Now.AddMinutes(-5)),
                new("María López",     "maria.lopez@contoso.com",    "Marketing", "Colaborador", true,  DateTime.Today.AddDays(-1)),
                new("Carlos Freire",   "carlos.freire@contoso.com",  "Ventas",    "Colaborador", true,  DateTime.Now.AddHours(-3)),
                new("Equipo Ventas",   "ventas@contoso.com",         "Ventas",    "Editor",      true,  DateTime.Today.AddDays(-2)),
                new("Jesús Domínguez", "jesus.dominguez@contoso.com","IT",        "Admin",       true,  DateTime.Now.AddHours(-20)),
                new("Soporte N1",      "soporte.n1@contoso.com",     "IT",        "Colaborador", true,  DateTime.Today.AddDays(-4)),
                new("Logística Bot",   "logistica@contoso.com",      "Logística", "Servicio",    false, DateTime.Today.AddDays(-5)),
                new("RRHH Interno",    "rrhh@contoso.com",           "RRHH",      "Editor",      false, DateTime.Today.AddDays(-7)),
            };

            _groups = new()
            {
                new GrupoDTO("Marketing", "Acciones y campañas", new()
                {
                    new MiembroDTO("Ana Pérez",    "ana.perez@contoso.com",   "Editor",       true),
                    new MiembroDTO("María López",  "maria.lopez@contoso.com", "Colaborador",  true),
                }, DateTime.Now.AddHours(-2)),

                new GrupoDTO("Ventas", "Equipo comercial", new()
                {
                    new MiembroDTO("Carlos Freire","carlos.freire@contoso.com","Colaborador", true),
                    new MiembroDTO("Equipo Ventas","ventas@contoso.com",      "Editor",       true),
                }, DateTime.Today.AddDays(-1)),

                new GrupoDTO("IT", "Soporte y administración", new()
                {
                    new MiembroDTO("Jesús Domínguez","jesus.dominguez@contoso.com","Admin",      true),
                    new MiembroDTO("Soporte N1",     "soporte.n1@contoso.com",     "Colaborador",true),
                }, DateTime.Today.AddDays(-3)),

                new GrupoDTO("Logística", "Envíos y almacén", new()
                {
                    new MiembroDTO("Logística Bot","logistica@contoso.com","Servicio", false),
                }, DateTime.Today.AddDays(-5)),

                new GrupoDTO("RRHH", "Gestión de personal", new()
                {
                    new MiembroDTO("RRHH Interno","rrhh@contoso.com","Editor", false),
                }, DateTime.Today.AddDays(-7)),
            };
        }

        // ===== USERS =====
        public Task<PagedResult<UsuarioDTO>> GetUsersAsync(
            string? q, string? sortBy, string? sortDir, int page, int pageSize, CancellationToken ct = default)
        {
            IEnumerable<UsuarioDTO> data = _users;

            if (!string.IsNullOrWhiteSpace(q))
            {
                var s = q.Trim();
                data = data.Where(x =>
                    x.Nombre.Contains(s, StringComparison.OrdinalIgnoreCase) ||
                    x.Email.Contains(s, StringComparison.OrdinalIgnoreCase) ||
                    x.Grupo.Contains(s, StringComparison.OrdinalIgnoreCase) ||
                    x.Rol.Contains(s, StringComparison.OrdinalIgnoreCase));
            }

            data = SortUsers(data, sortBy, sortDir);

            var size = ClampPageSize(pageSize);
            var items = data.Skip(page * size).Take(size).ToList();
            return Task.FromResult(new PagedResult<UsuarioDTO> { TotalItems = data.Count(), Items = items });
        }

        public Task<UsuarioDTO?> GetUserByNameAsync(string name, CancellationToken ct = default)
            => Task.FromResult(_users.FirstOrDefault(u => u.Nombre.Equals(name, StringComparison.OrdinalIgnoreCase)));

        // ===== GROUPS =====
        public Task<PagedResult<GrupoDTO>> GetGroupsAsync(
            string? q, string? sortBy, string? sortDir, int page, int pageSize, CancellationToken ct = default)
        {
            IEnumerable<GrupoDTO> data = _groups;

            if (!string.IsNullOrWhiteSpace(q))
            {
                var s = q.Trim();
                data = data.Where(g =>
                    g.Nombre.Contains(s, StringComparison.OrdinalIgnoreCase) ||
                    (g.Descripcion?.Contains(s, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    g.Miembros.Any(m =>
                        m.Nombre.Contains(s, StringComparison.OrdinalIgnoreCase) ||
                        m.Email.Contains(s, StringComparison.OrdinalIgnoreCase) ||
                        m.Rol.Contains(s, StringComparison.OrdinalIgnoreCase)));
            }

            data = SortGroups(data, sortBy, sortDir);

            var size = ClampPageSize(pageSize);
            var items = data.Skip(page * size).Take(size).ToList();
            return Task.FromResult(new PagedResult<GrupoDTO> { TotalItems = data.Count(), Items = items });
        }

        public Task<GrupoDTO?> GetGroupByNameAsync(string name, CancellationToken ct = default)
            => Task.FromResult(_groups.FirstOrDefault(g => g.Nombre.Equals(name, StringComparison.OrdinalIgnoreCase)));

        // ===== Helpers =====
        private static IEnumerable<UsuarioDTO> SortUsers(IEnumerable<UsuarioDTO> data, string? sortBy, string? sortDir)
        {
            bool desc = string.Equals(sortDir, "desc", StringComparison.OrdinalIgnoreCase);
            return (sortBy ?? "Nombre").ToLowerInvariant() switch
            {
                "email" => desc ? data.OrderByDescending(x => x.Email) : data.OrderBy(x => x.Email),
                "grupo" => desc ? data.OrderByDescending(x => x.Grupo) : data.OrderBy(x => x.Grupo),
                "rol" => desc ? data.OrderByDescending(x => x.Rol) : data.OrderBy(x => x.Rol),
                "activo" => desc ? data.OrderByDescending(x => x.Activo) : data.OrderBy(x => x.Activo),
                "ultimoacceso" => desc ? data.OrderByDescending(x => x.UltimoAcceso) : data.OrderBy(x => x.UltimoAcceso),
                _ => desc ? data.OrderByDescending(x => x.Nombre) : data.OrderBy(x => x.Nombre),
            };
        }

        private static IEnumerable<GrupoDTO> SortGroups(IEnumerable<GrupoDTO> data, string? sortBy, string? sortDir)
        {
            bool desc = string.Equals(sortDir, "desc", StringComparison.OrdinalIgnoreCase);
            return (sortBy ?? "Nombre").ToLowerInvariant() switch
            {
                "ultimocambio" => desc ? data.OrderByDescending(x => x.UltimoCambio) : data.OrderBy(x => x.UltimoCambio),
                "miembroscount" => desc ? data.OrderByDescending(x => x.MiembrosCount) : data.OrderBy(x => x.MiembrosCount),
                _ => desc ? data.OrderByDescending(x => x.Nombre) : data.OrderBy(x => x.Nombre),
            };
        }

        private static int ClampPageSize(int pageSize)
            => pageSize <= 0 ? 10 : Math.Min(pageSize, 100);
    }
}
