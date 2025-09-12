using Api.DTO;
using Api.Helpers;
using Api.Service;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly ILogger<GroupsController> logger;
        private readonly IHttpContextAccessor httpContext;
        private readonly CallService callService;

        public UsersController(
            ILogger<GroupsController> logger,
            IHttpContextAccessor httpContext,
            CallService callService)
        {
            this.logger = logger;
            this.httpContext = httpContext;
            this.callService = callService;
        }


        // GET /api/users
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PagedResultDTO<UserDTO>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PagedResultDTO<UserDTO>>> GetUsersAsync(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 25)
        {
            var traceId = httpContext.HttpContext?.TraceIdentifier.Split(':')[0] ?? "";
            try
            {
                logger.LogInformation("[{TraceId}] Call: GetUsersAsync(pageNumber={PageNumber}, pageSize={PageSize})",
                    traceId, pageNumber, pageSize);

                var response = await callService.Users_GetUsersAsync(pageSize, pageNumber);

                logger.LogInformation(response != null
                    ? "[{TraceId}] FinishCall: GetUsersAsync – returned {Count} items (page {PageNumber})"
                    : "[{TraceId}] FinishCall: GetUsersAsync – response null",
                    traceId,
                    response?.Entities?.Count ?? 0,
                    pageNumber);

                if (response?.Entities is null || !response.Entities.Any())
                    return NotFound();

                // 1) Map a DTOs 
                var dto = Mappers.FromRaw(response);

                // 2) Detecta IDs de grupo sin nombre
                var missingIds = dto.Entities
                    .SelectMany(u => u.ListaGrupos ?? new List<GroupDTO>())
                    .Where(g => string.IsNullOrWhiteSpace(g.Name) && !string.IsNullOrWhiteSpace(g.Id))
                    .Select(g => g.Id!)
                    .Distinct()
                    .ToList();

                // 3) Si hay grupos sin nombre, creamos un diccionario id->name
                if (missingIds.Count > 0)
                {
                    // Para no saturar la API
                    var throttler = new SemaphoreSlim(5);
                    var tasks = missingIds.Select(async id =>
                    {
                        await throttler.WaitAsync();
                        try { return await callService.Groups_GetGroupByIdAsync(id); }
                        finally { throttler.Release(); }
                    });

                    var fetched = await Task.WhenAll(tasks);

                    var namesById = fetched
                        .Where(g => g != null && !string.IsNullOrWhiteSpace(g.Id))
                        .GroupBy(g => g!.Id)
                        .Select(g => g.First()!)
                        .ToDictionary(g => g.Id!, g => g.Name ?? g.Id);

                    // 4) Nombre a los grupos
                    foreach (var u in dto.Entities)
                    {
                        foreach (var g in u.ListaGrupos)
                        {
                            if (string.IsNullOrWhiteSpace(g.Name) && !string.IsNullOrWhiteSpace(g.Id)
                                && namesById.TryGetValue(g.Id, out var name))
                            {
                                g.Name = name;
                            }
                        }
                    }
                }
                // 5) DTO completado
                return Ok(dto);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "[{TraceId}] GetUsersAsync error", traceId);
                return StatusCode(500, new { Message = "Error llamando a Genesys Cloud (Users)", Detail = ex.Message });
            }
        }


        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserDTO))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserDTO>> GetUserByIdAsync([FromRoute] string id)
        {
            try
            {
                var raw = await callService.Users_GetUserByIdAsync(id);
                if (raw is null) return NotFound();

                var dto = Mappers.FromRaw(raw);

                // Completar nombres de grupos si vienen vacíos
                var missing = (dto.ListaGrupos ?? []).Where(g => string.IsNullOrWhiteSpace(g.Name) && !string.IsNullOrWhiteSpace(g.Id)).ToList();
                foreach (var g in missing)
                {
                    var full = await callService.Groups_GetGroupByIdAsync(g.Id!);
                    if (!string.IsNullOrWhiteSpace(full?.Name))
                        g.Name = full!.Name!;
                }

                return Ok(dto);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetUserByIdAsync error");
                return StatusCode(500, new { Message = "Error llamando a Genesys Cloud (User by Id)", Detail = ex.Message });
            }
        }

    }
}
