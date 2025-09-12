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


    //    [HttpGet("{groupId}/members")]
    //    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GroupMemberDTO>))]
    //    [ProducesResponseType(StatusCodes.Status404NotFound)]
    //    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    //    public async Task<ActionResult<List<GroupMemberDTO>>> GetGroupMembersAsync([FromRoute] string groupId)
    //    {
    //        var traceId = httpContext.HttpContext?.TraceIdentifier.Split(':')[0] ?? "";
    //        try
    //        {
    //            logger.LogInformation("[{TraceId}] Call: GetGroupMembersAsync(groupId={GroupId})", traceId, groupId);

    //            var raw = await callService.Groups_GetGroupsIndividualsAsync(groupId);
    //            var items = GroupMappersDTO.ToMemberList(raw);

    //            logger.LogInformation("[{TraceId}] FinishCall: GetGroupMembersAsync – returned {Count} items",
    //                traceId, items.Count);

    //            if (items.Count == 0)
    //                return NotFound();

    //            return Ok(items);
    //        }
    //        catch (Exception ex)
    //        {
    //            logger.LogError(ex, "[{TraceId}] GetGroupMembersAsync error", traceId);
    //            return StatusCode(500, new { Message = "Error llamando a Genesys Cloud (Group Members)", Detail = ex.Message });
    //        }
    //    }


    //    [HttpGet("list")]
    //    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<GroupDTO>))]
    //    [ProducesResponseType(StatusCodes.Status404NotFound)]
    //    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    //    public async Task<ActionResult<IEnumerable<GroupDTO>>> GetGroupsListAsync(
    //           [FromQuery] int pageNumber = 1,
    //           [FromQuery] int pageSize = 25)
    //    {
    //        var traceId = httpContext.HttpContext?.TraceIdentifier.Split(':')[0] ?? "";
    //        try
    //        {
    //            logger.LogInformation("[{TraceId}] Call: GetGroupsListAsync(pageNumber={PageNumber}, pageSize={PageSize})",
    //                traceId, pageNumber, pageSize);

    //            var response = await callService.Groups_GetGroupsAsync(pageSize, pageNumber);

    //            if (response?.Entities is null || !response.Entities.Any())
    //                return NotFound();

    //            var items = response.Entities.Select(g => GroupMappersDTO.FromRaw(
    //                id: g.Id,
    //                name: g.Name,
    //                memberCountRaw: g.MemberCount,
    //                rolesEnabled: g.RolesEnabled,
    //                dateModified: g.DateModified,
    //                ownersCount: g.Owners?.Count ?? 0
    //            )).ToList();


    //            logger.LogInformation("[{TraceId}] FinishCall: GetGroupsListAsync – mapped {Count} items (page {PageNumber})",
    //                traceId, items.Count, pageNumber);

    //            return Ok(items);
    //        }
    //        catch (Exception ex)
    //        {
    //            logger.LogError(ex, "[{TraceId}] GetGroupsListAsync error", traceId);
    //            return StatusCode(500, new { Message = "Error mapeando Groups a DTO", Detail = ex.Message });
    //        }
    //    }


    //    [HttpGet("list-paged")]
    //    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PagedResultDTO<GroupDTO>))]
    //    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    //    public async Task<ActionResult<PagedResultDTO<GroupDTO>>> GetGroupsListPagedAsync(
    //    [FromQuery] int pageNumber = 1,
    //    [FromQuery] int pageSize = 25)
    //    {
    //        var traceId = httpContext.HttpContext?.TraceIdentifier.Split(':')[0] ?? "";
    //        try
    //        {
    //            logger.LogInformation("[{TraceId}] Call: GetGroupsListPagedAsync(pageNumber={PageNumber}, pageSize={PageSize})",
    //                traceId, pageNumber, pageSize);

    //            var response = await callService.Groups_GetGroupsAsync(pageSize, pageNumber);

    //            var items = response?.Entities?.Select(g => GroupMappersDTO.FromRaw(
    //                id: g.Id,
    //                name: g.Name,
    //                memberCountRaw: g.MemberCount,
    //                rolesEnabled: g.RolesEnabled,
    //                dateModified: g.DateModified,
    //                ownersCount: g.Owners?.Count ?? 0
    //            )).ToList() ?? new List<GroupDTO>();

    //            var total = 0;
    //            var totalRaw = response?.Total ?? 0;
    //            total = totalRaw > int.MaxValue ? int.MaxValue : (int)totalRaw;

    //            var result = new PagedResultDTO<GroupDTO>
    //            {
    //                Items = items,
    //                PageNumber = pageNumber,
    //                PageSize = pageSize,
    //                Total = total
    //            };

    //            logger.LogInformation("[{TraceId}] FinishCall: GetGroupsListPagedAsync – items={Count}, total={Total}, page={PageNumber}",
    //                traceId, items.Count, total, pageNumber);

    //            return Ok(result);
    //        }
    //        catch (Exception ex)
    //        {
    //            logger.LogError(ex, "[{TraceId}] GetGroupsListPagedAsync error", traceId);
    //            return StatusCode(500, new { Message = "Error mapeando Groups a DTO paginado", Detail = ex.Message });
    //        }
    //    }
    //}


