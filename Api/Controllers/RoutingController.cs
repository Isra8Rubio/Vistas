using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Api.Service.GenesysAPI;
using PureCloudPlatform.Client.V2.Model;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/routing")]
    public class RoutingController : ControllerBase
    {
        private readonly ILogger<RoutingController> logger;
        private readonly IHttpContextAccessor httpContext;
        private readonly RoutingApiService routingService;

        public RoutingController(
            ILogger<RoutingController> logger,
            IHttpContextAccessor httpContext,
            RoutingApiService routingService)
        {
            this.logger = logger;
            this.httpContext = httpContext;
            this.routingService = routingService;
        }

        // /api/v2/routing/queues
        [HttpGet("queues")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(QueueEntityListing))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<QueueEntityListing>> GetQueuesAsync(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 25)
        {
            var traceId = httpContext.HttpContext?.TraceIdentifier.Split(':')[0] ?? "";
            try
            {
                logger.LogInformation("[{TraceId}] Call: GetQueuesAsync(pageNumber={PageNumber}, pageSize={PageSize})",
                    traceId, pageNumber, pageSize);

                var response = await routingService.Routing_GetRoutingQueuesAsync(pageNumber, pageSize);

                logger.LogInformation(response != null
                    ? "[{TraceId}] FinishCall: GetQueuesAsync – returned {Count} items (page {PageNumber})"
                    : "[{TraceId}] FinishCall: GetQueuesAsync – response null",
                    traceId,
                    response?.Entities?.Count ?? 0,
                    pageNumber);

                if (response == null || response.Entities == null || !response.Entities.Any())
                    return NotFound();

                return Ok(response);
            }
            catch (System.Exception ex)
            {
                logger.LogError(ex, "[{TraceId}] GetQueuesAsync error", traceId);
                return StatusCode(500, new
                {
                    Message = "Error llamando a Genesys Cloud (Routing/Queues)",
                    Detail = ex.Message
                });
            }
        }

        // Proxy de /api/v2/routing/queues/divisionviews (paginado, filtrable por divisionId)
        [HttpGet("queues/divisionviews")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(QueueEntityListing))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<QueueEntityListing>> GetQueuesDivisionviewsAsync(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 25,
            // admite múltiples ?divisionId=...&divisionId=...
            [FromQuery(Name = "divisionId")] List<string>? divisionIds = null)
        {
            var traceId = httpContext.HttpContext?.TraceIdentifier.Split(':')[0] ?? "";

            try
            {
                logger.LogInformation("[{TraceId}] Call: GetQueuesDivisionviewsAsync(pageNumber={PageNumber}, pageSize={PageSize}, divisions={DivCount})",
                    traceId, pageNumber, pageSize, divisionIds?.Count ?? 0);

                var response = await routingService.Routing_GetRoutingQueuesDivisionViewsAsync(pageNumber, pageSize, divisionIds);

                logger.LogInformation(response != null
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
                logger.LogError(ex, "[{TraceId}] GetQueuesDivisionviewsAsync error", traceId);
                return StatusCode(500, new
                {
                    Message = "Error llamando a Genesys Cloud (Routing/Queues Divisionviews)",
                    Detail = ex.Message
                });
            }
        }
    }
}
