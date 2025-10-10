using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using API.APIService;

namespace WebBlazor.Components.Pages 
{
    public partial class Usuarios : ComponentBase
    {
        [Inject] private APIClient Api { get; set; } = default!;
        [Inject] private NavigationManager Nav { get; set; } = default!;
        [Inject] private IJSRuntime JS { get; set; } = default!;
        [Inject] private ISnackbar Snackbar { get; set; } = default!;

        // Referencia al grid
        private MudDataGrid<UserDTO>? dataGrid;

        // Selección actual
        private UserDTO? seleccionado;

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

        // Carga de datos para el DataGrid
        private async Task<GridData<UserDTO>> ServerReload(GridState<UserDTO> state)
        {
            try
            {
                // Parámetros de paginación
                var pageNumber = state.Page + 1;
                var pageSize = state.PageSize;

                // Llamada a la API
                var data = await Api.Users_GetUsersAsync(pageNumber, pageSize);

                // Total robusto
                var total = data.Total > 0
                    ? data.Total
                    : data.PageCount > 0 && data.PageSize > 0 ? data.PageCount * data.PageSize : data.Entities.Count;

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

                // Si ya no está el seleccionado en la página, limpiar
                if (seleccionado is not null && !list.Any(u => u.Id == seleccionado.Id))
                    seleccionado = null;

                return new GridData<UserDTO> { TotalItems = (int)total, Items = list };
            }
            catch (ApiException ex)
            {
                Snackbar.Add($"No se pudo cargar usuarios: {ex.StatusCode}", Severity.Error);
                return new GridData<UserDTO> { TotalItems = 0, Items = [] };
            }
            catch (Exception ex)
            {
                Snackbar.Add($"No se pudo cargar usuarios: {ex.Message}", Severity.Error);
                return new GridData<UserDTO> { TotalItems = 0, Items = [] };
            }
        }

        private void GotoUser(DataGridRowClickEventArgs<UserDTO> args)
            => Nav.NavigateTo($"/usuarios/{args.Item.Id}");
    }
}
