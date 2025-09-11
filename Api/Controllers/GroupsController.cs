using Api.Service;
using Microsoft.AspNetCore.Mvc;
using PureCloudPlatform.Client.V2.Model;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/groups")]
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
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GroupEntityListing))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<GroupEntityListing>> GetGroupsAsync(
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

                return Ok(response);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "[{TraceId}] GetGroupsAsync error", traceId);
                return StatusCode(500, new { Message = "Error llamando a Genesys Cloud (Groups)", Detail = ex.Message });
            }
        }

        // GET /api/groups/{groupId}/individuals
        [HttpGet("{groupId}/individuals")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserEntityListing))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<UserEntityListing>> GetGroupIndividualsAsync([FromRoute] string groupId)
        {
            var traceId = httpContext.HttpContext?.TraceIdentifier.Split(':')[0] ?? "";
            try
            {
                logger.LogInformation("[{TraceId}] Call: GetGroupIndividualsAsync(groupId={GroupId})",
                    traceId, groupId);

                var response = await callService.Groups_GetGroupsIndividualsAsync(groupId);

                logger.LogInformation(response != null
                    ? "[{TraceId}] FinishCall: GetGroupIndividualsAsync – returned {Count} items"
                    : "[{TraceId}] FinishCall: GetGroupIndividualsAsync – response null",
                    traceId,
                    response?.Entities?.Count ?? 0);

                if (response?.Entities is null || !response.Entities.Any())
                    return NotFound();

                return Ok(response);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "[{TraceId}] GetGroupIndividualsAsync error", traceId);
                return StatusCode(500, new { Message = "Error llamando a Genesys Cloud (Groups/Individuals)", Detail = ex.Message });
            }
        }
    }
}

