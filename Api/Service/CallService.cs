using Api.Service.GenesysAPI;
using PureCloudPlatform.Client.V2.Model;

namespace Api.Service
{
    public class CallService : GenesysAPIService
    {
        private readonly RoutingApiService _routingService;
        private readonly GroupApiService _groupService;
        private readonly UserApiService _userService;

        public CallService(
            RoutingApiService routingService,
            GroupApiService groupService,
            UserApiService userService,
            ILogger<GenesysAPIService> logger) : base(logger)
        {
            _routingService = routingService;
            _groupService = groupService;
            _userService = userService;
        }

        // ===== Users =====
        public async Task<UserEntityListing?> Users_GetUsersAsync(int pageSize = 25, int pageNumber = 1)
            => await _userService.Users_GetUsersAsync(pageSize, pageNumber);

        public async Task<User?> Users_GetUserByIdAsync(string userId)
            => await _userService.Users_GetUserByIdAsync(userId);

        // ===== Groups =====
        public async Task<GroupEntityListing?> Groups_GetGroupsAsync(int pageSize = 25, int pageNumber = 1)
            => await _groupService.Groups_GetGroupsAsync(pageSize, pageNumber);

        public async Task<Group?> Groups_GetGroupByIdAsync(string groupId)
            => await _groupService.Groups_GetGroupByIdAsync(groupId);

        // ===== Routing =====
        public async Task<QueueEntityListing?> Routing_GetRoutingQueuesAsync(int pageNumber = 1, int pageSize = 25)
            => await _routingService.Routing_GetRoutingQueuesAsync(pageNumber, pageSize);

        public async Task<QueueEntityListing?> Routing_GetRoutingQueuesDivisionViewsAsync(
            int pageNumber = 1, int pageSize = 25, List<string>? divisionId = null)
            => await _routingService.Routing_GetRoutingQueuesDivisionViewsAsync(pageNumber, pageSize, divisionId);
    }
}
