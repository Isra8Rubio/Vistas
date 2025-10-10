using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using API.APIService;

namespace WebBlazor.Components.Pages
{
    public partial class Grupos : ComponentBase
    {
        [Inject] private APIClient Api { get; set; } = default!;
        [Inject] private NavigationManager Nav { get; set; } = default!;

        // Referencia al grid
        private MudDataGrid<GroupDTO>? dataGrid;

        // === Filtros ===
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

        private DateRange? _range;
        private DateRange? Range
        {
            get => _range;
            set
            {
                _range = value;
                _ = dataGrid?.ReloadServerData();
            }
        }

        private Task ClearFilters()
        {
            Search = null;
            Range = null;
            return dataGrid?.ReloadServerData() ?? Task.CompletedTask;
        }

        private async Task<GridData<GroupDTO>> ServerReload(GridState<GroupDTO> state)
        {
            try
            {
                var pageNumber = state.Page + 1;
                var pageSize = state.PageSize;

                var data = await Api.Groups_GetGroupsAsync(pageNumber, pageSize);

                var total = data.Total > 0
                    ? data.Total
                    : data.PageCount > 0 && data.PageSize > 0
                        ? data.PageCount * data.PageSize
                        : data.Entities.Count;

                IEnumerable<GroupDTO> items = data.Entities;

                // Filtro: texto
                if (!string.IsNullOrWhiteSpace(_search))
                {
                    var q = _search.Trim();
                    items = items.Where(x =>
                        (x.Name?.Contains(q, StringComparison.OrdinalIgnoreCase) ?? false) ||
                        (x.ListaUsuarios?.Any(u =>
                            (u.Name?.Contains(q, StringComparison.OrdinalIgnoreCase) ?? false) ||
                            (u.Email?.Contains(q, StringComparison.OrdinalIgnoreCase) ?? false)) ?? false));
                }

                // Filtro: rango de fechas en DateModified
                if (Range?.Start is not null)
                {
                    var start = Range.Start.Value.Date;
                    var end = (Range.End ?? Range.Start)!.Value.Date.AddDays(1).AddTicks(-1);
                    items = items.Where(x => x.DateModified >= start && x.DateModified <= end);
                }

                return new GridData<GroupDTO>
                {
                    TotalItems = (int)total,
                    Items = items.ToList()
                };
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
                return new GridData<GroupDTO> { TotalItems = 0, Items = [] };
            }
        }

        private void GotoGroup(DataGridRowClickEventArgs<GroupDTO> args)
        {
            if (!string.IsNullOrWhiteSpace(args.Item.Id))
                Nav.NavigateTo($"/grupos/{args.Item.Id}");
        }
    }
}
