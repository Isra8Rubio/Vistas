using Api.DTO;
using Api.Helpers;
using Api.Service;
using Api.Service.GenesysAPI;
using Microsoft.AspNetCore.Mvc;
using PureCloudPlatform.Client.V2.Model;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/routing")]
    public class RoutingController : ControllerBase
    {
        private readonly ILogger<RoutingController> _logger;
        private readonly IHttpContextAccessor _httpContext;
        private readonly CallService _callService;

        public RoutingController(
            ILogger<RoutingController> _logger,
            IHttpContextAccessor _httpContext,
            CallService _callService)
        {
            this._logger = _logger;
            this._httpContext = _httpContext;
            this._callService = _callService;
        }

        // GET /api/routing/queues
        [HttpGet("queues")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PagedResultDTO<QueueDTO>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PagedResultDTO<QueueDTO>>> GetQueuesAsync(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 25,
            [FromQuery(Name = "divisionId")] List<string>? divisionIds = null)
        {
            var traceId = _httpContext.HttpContext?.TraceIdentifier.Split(':')[0] ?? "";
            try
            {
                _logger.LogInformation("[{TraceId}] Call: GetQueuesAsync(pageNumber={Page}, pageSize={Size}, divisionIds={Divs})",
                    traceId, pageNumber, pageSize, divisionIds is null ? "-" : string.Join(',', divisionIds));

                QueueEntityListing? response =
                    (divisionIds is not null && divisionIds.Count > 0)
                        ? await _callService.Routing_GetRoutingQueuesDivisionViewsAsync(pageNumber, pageSize, divisionIds)
                        : await _callService.Routing_GetRoutingQueuesAsync(pageNumber, pageSize);

                if (response?.Entities is null || response.Entities.Count == 0)
                {
                    _logger.LogInformation("[{TraceId}] FinishCall: GetQueuesAsync – no data", traceId);
                    return NotFound();
                }

                var dto = Mappers.FromRaw(response);

                // Listado: no devolvemos miembros para evitar payloads grandes (mismo criterio que Groups).
                foreach (var q in dto.Entities)
                    q.ListaUsuarios.Clear();

                _logger.LogInformation("[{TraceId}] FinishCall: GetQueuesAsync – returned {Count} items (page {Page})",
                    traceId, dto.Entities.Count, pageNumber);

                return Ok(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[{TraceId}] GetQueuesAsync error", traceId);
                return StatusCode(500, new { Message = "Error llamando a Genesys Cloud (Queues)", Detail = ex.Message });
            }
        }

        // GET /api/routing/queues/{id}
        [HttpGet("queues/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(QueueDTO))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<QueueDTO>> GetQueueByIdAsync([FromRoute] string id)
        {
            var traceId = _httpContext.HttpContext?.TraceIdentifier.Split(':')[0] ?? "";
            try
            {
                _logger.LogInformation("[{TraceId}] Call: GetQueueByIdAsync(id={Id})", traceId, id);

                var raw = await _callService.Routing_GetRoutingQueueByIdAsync(id);
                if (raw is null)
                {
                    _logger.LogInformation("[{TraceId}] FinishCall: GetQueueByIdAsync – not found ({Id})", traceId, id);
                    return NotFound();
                }

                var dto = Mappers.FromRaw(raw);

                // Detalle (por ahora) sin miembros; se podrá hidratar más adelante.
                dto.ListaUsuarios.Clear();

                _logger.LogInformation("[{TraceId}] FinishCall: GetQueueByIdAsync – OK ({Id})", traceId, id);
                return Ok(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[{TraceId}] GetQueueByIdAsync error ({Id})", traceId, id);
                return StatusCode(500, new { Message = "Error llamando a Genesys Cloud (Queue by Id)", Detail = ex.Message });
            }
        }

        // /api/v2/routing/queues/divisionviews
        [HttpGet("queues/divisionviews")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(QueueEntityListing))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<QueueEntityListing>> GetQueuesDivisionviewsAsync(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 25,
            [FromQuery(Name = "divisionId")] List<string>? divisionIds = null)
        {
            var traceId = _httpContext.HttpContext?.TraceIdentifier.Split(':')[0] ?? "";
            try
            {
                _logger.LogInformation("[{TraceId}] Call: GetQueuesDivisionviewsAsync(pageNumber={PageNumber}, pageSize={PageSize}, divisions={DivCount})",
                    traceId, pageNumber, pageSize, divisionIds?.Count ?? 0);

                var response = await _callService.Routing_GetRoutingQueuesDivisionViewsAsync(pageNumber, pageSize, divisionIds);

                _logger.LogInformation(response != null
                    ? "[{TraceId}] FinishCall: GetQueuesDivisionviewsAsync – returned {Count} items (page {PageNumber})"
                    : "[{TraceId}] FinishCall: GetQueuesDivisionviewsAsync – response null",
                    traceId,
                    response?.Entities?.Count ?? 0,
                    pageNumber);

                if (response == null || response.Entities == null || !response.Entities.Any())
                    return NotFound();

                return Ok(response);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "[{TraceId}] GetQueuesDivisionviewsAsync error", traceId);
                return StatusCode(500, new
                {
                    Message = "Error llamando a Genesys Cloud (Routing/Queues Divisionviews)",
                    Detail = ex.Message
                });
            }
        }
    }
}
