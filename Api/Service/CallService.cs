using Api.Service.GenesysAPI;
using PureCloudPlatform.Client.V2.Model;

namespace Api.Service
{
    public class CallService : GenesysAPIService
    {
        private readonly RoutingApiService _routingService;
        private readonly GroupApiService _groupService;

        public CallService(
            RoutingApiService routingService,
            GroupApiService groupService,
            ILogger<GenesysAPIService> logger) : base(logger)
        {
            _routingService = routingService;
            _groupService = groupService;
        }

        // ===== Routing =====
        public async Task<QueueEntityListing?> Routing_GetRoutingQueuesAsync(int pageNumber = 1, int pageSize = 25)
            => await _routingService.Routing_GetRoutingQueuesAsync(pageNumber, pageSize);

        public async Task<QueueEntityListing?> Routing_GetRoutingQueuesDivisionViewsAsync(
            int pageNumber = 1, int pageSize = 25, List<string>? divisionId = null)
            => await _routingService.Routing_GetRoutingQueuesDivisionViewsAsync(pageNumber, pageSize, divisionId);

        // ===== Groups =====
        public async Task<GroupEntityListing?> Groups_GetGroupsAsync(int pageSize = 25, int pageNumber = 1)
            => await _groupService.Groups_GetGroupsAsync(pageSize, pageNumber);

        public async Task<UserEntityListing?> Groups_GetGroupsIndividualsAsync(string groupId)
            => await _groupService.Groups_GetGroupsIndividualsAsync(groupId);
    }
}
