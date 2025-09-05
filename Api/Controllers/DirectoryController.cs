using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Api.DTO;
using Api.Services;

namespace Api.Controllers
{
    [ApiController]
    [Route("api")]
    public class DirectoryController : ControllerBase
    {
        private readonly ILogger<DirectoryController> _logger;
        private readonly DirectoryClientService _service;
        private readonly IHttpContextAccessor _http;

        public DirectoryController(
            ILogger<DirectoryController> logger,
            DirectoryClientService service,
            IHttpContextAccessor http)
        {
            _logger = logger;
            _service = service;
            _http = http;
        }

        // GET /api/user?q=&sortBy=&sortDir=&page=&pageSize=
        [HttpGet("user")]
        [ProducesResponseType(typeof(PagedResult<UsuarioDTO>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResult<UsuarioDTO>>> GetUsers(
            [FromQuery] string? q,
            [FromQuery] string? sortBy = "Nombre",
            [FromQuery] string? sortDir = "asc",
            [FromQuery] int page = 0,
            [FromQuery] int pageSize = 10,
            CancellationToken ct = default)
        {
            var traceId = _http.HttpContext?.TraceIdentifier.Split(':')[0] ?? "";
            _logger.LogInformation("[{TraceId}] GET /api/user q={Q} sortBy={SortBy} sortDir={SortDir} page={Page} size={Size}",
                traceId, q, sortBy, sortDir, page, pageSize);

            var result = await _service.GetUsersAsync(q, sortBy, sortDir, page, pageSize, ct);
            return Ok(result);
        }

        // GET /api/user/{name}
        [HttpGet("user/{name}")]
        [ProducesResponseType(typeof(UsuarioDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UsuarioDTO>> GetUserByName(string name, CancellationToken ct = default)
        {
            var traceId = _http.HttpContext?.TraceIdentifier.Split(':')[0] ?? "";
            _logger.LogInformation("[{TraceId}] GET /api/user/{name}", traceId, name);

            var user = await _service.GetUserByNameAsync(name, ct);
            return user is null ? NotFound() : Ok(user);
        }

        // GET /api/group?q=&sortBy=&sortDir=&page=&pageSize=
        [HttpGet("group")]
        [ProducesResponseType(typeof(PagedResult<GrupoDTO>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResult<GrupoDTO>>> GetGroups(
            [FromQuery] string? q,
            [FromQuery] string? sortBy = "Nombre",
            [FromQuery] string? sortDir = "asc",
            [FromQuery] int page = 0,
            [FromQuery] int pageSize = 10,
            CancellationToken ct = default)
        {
            var traceId = _http.HttpContext?.TraceIdentifier.Split(':')[0] ?? "";
            _logger.LogInformation("[{TraceId}] GET /api/group q={Q} sortBy={SortBy} sortDir={SortDir} page={Page} size={Size}",
                traceId, q, sortBy, sortDir, page, pageSize);

            var result = await _service.GetGroupsAsync(q, sortBy, sortDir, page, pageSize, ct);
            return Ok(result);
        }

        // GET /api/group/{name}
        [HttpGet("group/{name}")]
        [ProducesResponseType(typeof(GrupoDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<GrupoDTO>> GetGroupByName(string name, CancellationToken ct = default)
        {
            var traceId = _http.HttpContext?.TraceIdentifier.Split(':')[0] ?? "";
            _logger.LogInformation("[{TraceId}] GET /api/group/{name}", traceId, name);

            var group = await _service.GetGroupByNameAsync(name, ct);
            return group is null ? NotFound() : Ok(group);
        }
    }
}
