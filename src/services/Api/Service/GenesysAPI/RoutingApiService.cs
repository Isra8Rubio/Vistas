using PureCloudPlatform.Client.V2.Api;
using PureCloudPlatform.Client.V2.Model;

namespace Api.Service.GenesysAPI
{
    public class RoutingApiService : GenesysAPIService
    {
        private readonly RoutingApi _routingApi = new();
        private readonly GenesysAuthService _genesysAuthService;

        public RoutingApiService(GenesysAuthService authService, ILogger<GenesysAPIService> logger) : base(logger)
        {
            _genesysAuthService = authService;
        }

        ///api/v2/routing/queues
        public async Task<QueueEntityListing?> Routing_GetRoutingQueuesAsync(int pageNumber = 1, int pageSize = 25)
        {
            logger.LogDebug("Routing_GetRoutingQueuesAsync --> pageNumber={pageNumber}, pageSize={pageSize}",
                pageNumber, pageSize);

            _genesysAuthService.CheckToken();

            try
            {
                return await ExecuteWithRetry(
                    async () => await _routingApi.GetRoutingQueuesAsync(
                        pageNumber: pageNumber,
                        pageSize: pageSize
                    ),
                    "Routing_GetRoutingQueuesAsync"
                );
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Routing_GetRoutingQueuesAsync error");
                return null;
            }
        }

        // Api/Service/GenesysAPI/RoutingApiService.cs
        public async Task<Queue?> Routing_GetRoutingQueueByIdAsync(string queueId)
        {
            logger.LogDebug("Routing_GetRoutingQueueByIdAsync --> queueId={queueId}", queueId);
            _genesysAuthService.CheckToken();

            try
            {
                return await ExecuteWithRetry(
                    async () => await _routingApi.GetRoutingQueueAsync(queueId),
                    "Routing_GetRoutingQueueByIdAsync");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Routing_GetRoutingQueueByIdAsync error");
                return null;
            }
        }

        // Miembros reales de la cola (users)
        public async Task<QueueMemberEntityListing?> Routing_GetRoutingQueueMembersAsync(string queueId, int pageSize = 250, int pageNumber = 1)
        {
            logger.LogDebug("Routing_GetRoutingQueueMembersAsync --> queueId={queueId}, pageSize={pageSize}, pageNumber={pageNumber}",
                queueId, pageSize, pageNumber);
            _genesysAuthService.CheckToken();

            try
            {
                return await ExecuteWithRetry(
                    async () => await _routingApi.GetRoutingQueueMembersAsync(queueId, pageSize, pageNumber),
                    "Routing_GetRoutingQueueMembersAsync");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Routing_GetRoutingQueueMembersAsync error");
                return null;
            }
        }


        ///api/v2/routing/queues/divisionviews
        public async Task<QueueEntityListing?> Routing_GetRoutingQueuesDivisionViewsAsync(int pageNumber = 1, int pageSize = 25, List<string>? divisionId = null)
        {
            logger.LogDebug("Routing_GetRoutingQueuesDivisionViewsAsync --> pageNumber={pageNumber}, pageSize={pageSize}, divisionsIds={divisionsIds}", 
                pageNumber, pageSize, divisionId);

            _genesysAuthService.CheckToken();

            try
            {
                return await ExecuteWithRetry(
                    async () => await _routingApi.GetRoutingQueuesDivisionviewsAsync(
                        pageNumber: pageNumber,
                        pageSize: pageSize,
                        divisionId: divisionId
                    ),
                    "Routing_GetRoutingQueuesDivisionViewsAsync"
                );
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Routing_GetRoutingQueuesDivisionViewsAsync error");
                return null;
            }
        }
    }
}
