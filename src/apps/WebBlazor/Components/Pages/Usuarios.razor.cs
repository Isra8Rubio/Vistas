using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.APIService;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;

namespace Web.Components.Pages
{
    public partial class Usuarios : ComponentBase
    {
        // Inyecciones (movidas desde el .razor)
        [Inject] private APIClient Api { get; set; } = default!;
        [Inject] private NavigationManager Nav { get; set; } = default!;
        [Inject] private IJSRuntime JS { get; set; } = default!;
        [Inject] private ISnackbar Snackbar { get; set; } = default!;

        // Estado Grid
        private MudDataGrid<UserDTO>? dataGrid;
        private List<UserDTO> _lastItems = new();
        private UserDTO? seleccionado;
        private int _totalItems;

        // Búsqueda
        private string? _search;
        private string? Search
        {
            get => _search;
            set
            {
                if (_search == value) return;
                _search = value;
                _ = dataGrid?.ReloadServerData();
            }
        }

        private Task ClearFilters()
        {
            Search = null;
            seleccionado = null;
            return dataGrid?.ReloadServerData() ?? Task.CompletedTask;
        }

        // Carga server-side
        private async Task<GridData<UserDTO>> ServerReload(GridState<UserDTO> state)
        {
            try
            {
                // Parámetros de paginación solicitados por el grid
                var pageNumber = state.Page + 1;
                var pageSize = state.PageSize;

                // Llamada a la API
                var data = await Api.Users_GetUsersAsync(pageNumber, pageSize);

                // Cálculo robusto del total de elementos
                var total = data.Total > 0
                    ? data.Total
                    : (data.PageCount > 0 && data.PageSize > 0
                        ? data.PageCount * data.PageSize
                        : data.Entities.Count);

                IEnumerable<UserDTO> items = data.Entities;

                // Filtro de búsqueda
                if (!string.IsNullOrWhiteSpace(_search))
                {
                    var q = _search.Trim();
                    items = items.Where(x =>
                        (x.Name?.Contains(q, StringComparison.OrdinalIgnoreCase) ?? false) ||
                        (x.Email?.Contains(q, StringComparison.OrdinalIgnoreCase) ?? false) ||
                        (x.ListaGrupos?.Any(g => g?.Name?.Contains(q, StringComparison.OrdinalIgnoreCase) ?? false) ?? false));
                }

                var list = items.ToList();
                _lastItems = list;
                _totalItems = (int)total;

                // si no hay selección o ya no existe en esta página, limpia selección
                if (seleccionado is not null && !list.Any(u => u.Id == seleccionado.Id))
                    seleccionado = null;

                return new GridData<UserDTO> { TotalItems = (int)total, Items = list };
            }
            catch (ApiException ex)
            {
                Snackbar.Add($"No se pudo cargar usuarios: {ex.StatusCode}", Severity.Error);
                _lastItems = new();
                return new GridData<UserDTO> { TotalItems = 0, Items = [] };
            }
            catch (Exception ex)
            {
                Snackbar.Add($"No se pudo cargar usuarios: {ex.Message}", Severity.Error);
                _lastItems = new();
                return new GridData<UserDTO> { TotalItems = 0, Items = [] };
            }
        }

        private void GotoUser(DataGridRowClickEventArgs<UserDTO> args)
            => Nav.NavigateTo($"/usuarios/{args.Item.Id}");
    }
}
