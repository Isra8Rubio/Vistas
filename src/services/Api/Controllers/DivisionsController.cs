using Api.DTO;
using Api.Helpers;
using Api.Service;
using Microsoft.AspNetCore.Mvc;
using PureCloudPlatform.Client.V2.Model;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DivisionsController : ControllerBase
    {
        private readonly ILogger<DivisionsController> _logger;
        private readonly IHttpContextAccessor _httpContext;
        private readonly CallService _callService;

        public DivisionsController(
            ILogger<DivisionsController> logger,
            IHttpContextAccessor httpContext,
            CallService callService)
        {
            _logger = logger;
            _httpContext = httpContext;
            _callService = callService;
        }

        // Api/Controllers/DivisionsController.cs
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PagedResultDTO<DivisionDTO>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PagedResultDTO<DivisionDTO>>> GetDivisionsAsync(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 25)
        {
            var traceId = _httpContext.HttpContext?.TraceIdentifier.Split(':')[0] ?? "";
            try
            {
                _logger.LogInformation("[{TraceId}] Call: GetDivisionsAsync(pageNumber={Page}, pageSize={Size})",
                    traceId, pageNumber, pageSize);

                var response = await _callService.Divisions_GetDivisionsAsync(pageNumber, pageSize)
                               ?? new AuthzDivisionEntityListing { Entities = new List<AuthzDivision>(), Total = 0, PageNumber = pageNumber, PageSize = pageSize };

                _logger.LogInformation("[{TraceId}] Divisions SDK -> total={Total} count={Count}",
                    traceId, response.Total, response.Entities?.Count ?? 0);

                var dto = Mappers.FromRaw(response);

                _logger.LogInformation("[{TraceId}] FinishCall: GetDivisionsAsync – returned {Count} items (page {Page})",
                    traceId, dto.Entities.Count, pageNumber);

                return Ok(dto); // <- siempre 200 con Entities (posible lista vacía)
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[{TraceId}] GetDivisionsAsync error", traceId);
                return StatusCode(500, new { Message = "Error llamando a Genesys Cloud (Divisions)", Detail = ex.Message });
            }
        }


        // GET /api/divisions/{id}
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DivisionDTO))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<DivisionDTO>> GetDivisionByIdAsync(string id)
        {
            var traceId = _httpContext.HttpContext?.TraceIdentifier.Split(':')[0] ?? "";
            try
            {
                _logger.LogInformation("[{TraceId}] Call: GetDivisionByIdAsync(id={Id})", traceId, id);

                var raw = await _callService.Divisions_GetDivisionByIdAsync(id);
                if (raw is null)
                {
                    _logger.LogInformation("[{TraceId}] FinishCall: GetDivisionByIdAsync – not found ({Id})", traceId, id);
                    return NotFound();
                }

                var dto = Mappers.FromRaw(raw);

                _logger.LogInformation("[{TraceId}] FinishCall: GetDivisionByIdAsync – OK ({Id})", traceId, id);
                return Ok(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[{TraceId}] GetDivisionByIdAsync error ({Id})", traceId, id);
                return StatusCode(500, new { Message = "Error llamando a Genesys Cloud (Division by Id)", Detail = ex.Message });
            }
        }
    }
}
