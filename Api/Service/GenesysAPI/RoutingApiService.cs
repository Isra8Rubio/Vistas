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

        public async Task<QueueEntityListing?> Routing_GetRoutingQueuesAsync(int pageNumber = 1, int pageSize = 25)
        {
            _logger.LogDebug("Routing_GetRoutingQueuesAsync --> pageNumber={pageNumber}, pageSize={pageSize}",
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
                _logger.LogError(ex, "Routing_GetRoutingQueuesAsync error");
                return null;
            }
        }

        public async Task<QueueEntityListing?> Routing_GetRoutingQueuesDivisionViewsAsync(int pageNumber = 1, int pageSize = 25, List<string>? divisionId = null)
        {
            _logger.LogDebug("Routing_GetRoutingQueuesDivisionViewsAsync --> pageNumber={pageNumber}, pageSize={pageSize}, divisionsIds={divisionsIds}", 
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
                _logger.LogError(ex, "Routing_GetRoutingQueuesDivisionViewsAsync error");
                return null;
            }
        }
    }
}
