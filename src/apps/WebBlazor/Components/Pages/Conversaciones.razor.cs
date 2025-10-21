using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using MudBlazor;
using API.APIService;

namespace WebBlazor.Components.Pages
{
    public partial class Conversaciones : ComponentBase
    {
        [Inject] private APIClient Api { get; set; } = default!;
        [Inject] private NavigationManager Nav { get; set; } = default!;
        [Inject] private ISnackbar Snackbar { get; set; } = default!;

        private MudDataGrid<ConversationListItemDTO>? dataGrid;

        // Filtros
        private string? _search;
        private string? Search
        {
            get => _search;
            set { if (_search == value) return; _search = value; _ = dataGrid?.ReloadServerData(); }
        }

        private DateRange? _range;
        private DateRange? Range
        {
            get => _range;
            set { if (_range == value) return; _range = value; _ = dataGrid?.ReloadServerData(); }
        }

        // jobId y fecha
        private string? JobId;
        private DateTime? FromDate { get; set; } = DateTime.Today;

        protected override void OnInitialized()
        {
            var uri = Nav.ToAbsoluteUri(Nav.Uri);
            if (QueryHelpers.ParseQuery(uri.Query).TryGetValue("jobId", out var v))
                JobId = v.ToString();

            if (string.IsNullOrWhiteSpace(JobId))
                Snackbar.Add("Falta ?jobId=... en la URL /conversaciones. Puedes crear un job arriba.", Severity.Info);
        }

        private Task ClearFilters()
        {
            Search = null;
            Range = null;
            return dataGrid?.ReloadServerData() ?? Task.CompletedTask;
        }

        // Crear job y navegar con ?jobId=...
        private async Task CreateAndLoadJob()
        {
            try
            {
                // Hoy por defecto
                var selected = FromDate ?? DateTime.Today;
                var fromStr = selected.ToString("yyyy-MM-dd");

                // Llamada
                var res = await Api.Conversations_CreateJobAsync(fromStr, default);
                var jobId = res?.JobId;

                if (string.IsNullOrWhiteSpace(jobId))
                {
                    Snackbar.Add("La API no devolvió JobId.", Severity.Warning);
                    return;
                }

                Nav.NavigateTo($"/conversaciones?jobId={jobId}", forceLoad: true);
            }
            catch (ApiException ex)
            {
                Snackbar.Add($"No se pudo crear el job: {ex.StatusCode}", Severity.Error);
            }
            catch (Exception ex)
            {
                Snackbar.Add($"No se pudo crear el job: {ex.Message}", Severity.Error);
            }
        }

        // Carga de datos del grid
        private async Task<GridData<ConversationListItemDTO>> ServerReload(GridState<ConversationListItemDTO> state)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(JobId))
                    return new GridData<ConversationListItemDTO> { TotalItems = 0, Items = [] };

                // MudDataGrid: page base 0 -> API: base 1
                var pageNumber = state.Page + 1;
                var pageSize = state.PageSize <= 0 ? 25 : state.PageSize;

                var data = await Api.Conversations_GetJobResultsSummaryAsync(JobId, pageNumber, pageSize, default);

                // Total robusto
                var total = data.Total > 0
                    ? data.Total
                    : data.PageCount > 0 && data.PageSize > 0 ? data.PageCount * data.PageSize : data.Entities.Count;

                IEnumerable<ConversationListItemDTO> items = data.Entities ?? [];

                // Filtro de búsqueda
                if (!string.IsNullOrWhiteSpace(_search))
                {
                    var q = _search.Trim();
                    items = items.Where(x =>
                        (x.RemoteDisplay?.Contains(q, StringComparison.OrdinalIgnoreCase) ?? false) ||
                        (x.Direction?.Contains(q, StringComparison.OrdinalIgnoreCase) ?? false) ||
                        (x.QueueId?.Contains(q, StringComparison.OrdinalIgnoreCase) ?? false));
                }

                // Filtro por fecha
                if (Range?.Start is DateTime from && Range?.End is DateTime to)
                {
                    var start = from.Date;
                    var endExcl = to.Date.AddDays(1);
                    items = items.Where(x =>
                    {
                        var when = (x.End ?? x.Start).ToLocalTime().DateTime;
                        return when >= start && when < endExcl;
                    });
                }

                var list = items.ToList();
                return new GridData<ConversationListItemDTO> { TotalItems = (int)total, Items = list };
            }
            catch (ApiException ex)
            {
                Snackbar.Add($"No se pudo cargar conversaciones: {ex.StatusCode}", Severity.Error);
                return new GridData<ConversationListItemDTO> { TotalItems = 0, Items = [] };
            }
            catch (Exception ex)
            {
                Snackbar.Add($"No se pudo cargar conversaciones: {ex.Message}", Severity.Error);
                return new GridData<ConversationListItemDTO> { TotalItems = 0, Items = [] };
            }
        }

        private void GotoConversation(DataGridRowClickEventArgs<ConversationListItemDTO> args)
        {
            var id = args.Item?.ConversationId;
            if (!string.IsNullOrWhiteSpace(id))
                Nav.NavigateTo($"/grafico?conversationId={id}");
        }
    }
}
