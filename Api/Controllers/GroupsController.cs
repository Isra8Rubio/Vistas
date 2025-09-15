using Api.DTO;
using Api.Helpers;
using Api.Service;
using Microsoft.AspNetCore.Mvc;
using PureCloudPlatform.Client.V2.Model;
using System.Collections.Generic;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GroupsController : ControllerBase
    {
        private readonly ILogger<GroupsController> logger;
        private readonly IHttpContextAccessor httpContext;
        private readonly CallService callService;

        public GroupsController(
            ILogger<GroupsController> logger,
            IHttpContextAccessor httpContext,
            CallService callService)
        {
            this.logger = logger;
            this.httpContext = httpContext;
            this.callService = callService;
        }

        // GET /api/groups
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PagedResultDTO<GroupDTO>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PagedResultDTO<GroupDTO>>> GetGroupsAsync(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 25)
        {
            var traceId = httpContext.HttpContext?.TraceIdentifier.Split(':')[0] ?? "";
            try
            {
                logger.LogInformation("[{TraceId}] Call: GetGroupsAsync(pageNumber={PageNumber}, pageSize={PageSize})",
                    traceId, pageNumber, pageSize);

                var response = await callService.Groups_GetGroupsAsync(pageSize, pageNumber);

                logger.LogInformation(response != null
                    ? "[{TraceId}] FinishCall: GetGroupsAsync – returned {Count} items (page {PageNumber})"
                    : "[{TraceId}] FinishCall: GetGroupsAsync – response null",
                    traceId,
                    response?.Entities?.Count ?? 0,
                    pageNumber);

                if (response?.Entities is null || !response.Entities.Any())
                    return NotFound();

                return Ok(Mappers.FromRaw(response));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "[{TraceId}] GetGroupsAsync error", traceId);
                return StatusCode(500, new { Message = "Error llamando a Genesys Cloud (Groups)", Detail = ex.Message });
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GroupDTO))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<GroupDTO>> GetGroupByIdAsync([FromRoute] string id)
        {
            try
            {
                var raw = await callService.Groups_GetGroupByIdAsync(id);
                if (raw is null) return NotFound();

                var dto = Mappers.FromRaw(raw);

                // --- hidratar nombres/emails si vienen vacíos ---
                var missingIds = (dto.ListaUsuarios ?? [])
                                 .Where(u => (string.IsNullOrWhiteSpace(u.Name) || string.IsNullOrWhiteSpace(u.Email))
                                             && !string.IsNullOrWhiteSpace(u.Id))
                                 .Select(u => u.Id!)
                                 .Distinct()
                                 .ToList();

                if (missingIds.Count > 0)
                {
                    var throttler = new SemaphoreSlim(5);
                    var tasks = missingIds.Select(async uid =>
                    {
                        await throttler.WaitAsync();
                        try { return await callService.Users_GetUserByIdAsync(uid); }
                        finally { throttler.Release(); }
                    });

                    var fetched = await Task.WhenAll(tasks);
                    var byId = fetched.Where(u => u is not null && !string.IsNullOrWhiteSpace(u.Id))
                                      .GroupBy(u => u!.Id)
                                      .Select(g => g.First()!)
                                      .ToDictionary(u => u.Id!, u => u);

                    foreach (var u in dto.ListaUsuarios ?? [])
                        if (u.Id is not null && byId.TryGetValue(u.Id, out var full))
                        {
                            u.Name ??= full.Name;
                            u.Email ??= full.Email;
                        }
                }
                return Ok(dto);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetGroupByIdAsync error");
                return StatusCode(500, new { Message = "Error llamando a Genesys Cloud (Group by Id)", Detail = ex.Message });
            }
        }


    }
}



