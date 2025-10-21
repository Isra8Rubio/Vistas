using Api.DTO;
using Api.Helpers;
using Api.Service.GenesysAPI;
using Microsoft.AspNetCore.Authorization;
using PureCloudPlatform.Client.V2.Api;
using PureCloudPlatform.Client.V2.Model;

namespace Api.Service
{
    public class CallService : GenesysAPIService
    {
        private readonly RoutingApiService _routingService;
        private readonly GroupApiService _groupService;
        private readonly UserApiService _userService;
        private readonly DivisionApiService _divisionService;
        private readonly ConversationApiService _conversationService;

        public CallService(
            RoutingApiService routingService,
            GroupApiService groupService,
            UserApiService userService,
            DivisionApiService divisionService,
            ConversationApiService conversationService,
        ILogger<GenesysAPIService> logger) : base(logger)
        {
            _routingService = routingService;
            _groupService = groupService;
            _userService = userService;
            _divisionService = divisionService;
            _conversationService = conversationService;
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

        public async Task<UserEntityListing?> Groups_GetGroupMembersAsync(string groupId, int pageSize = 25, int pageNumber = 1)
            => await _groupService.Groups_GetGroupMembersAsync(groupId, pageSize, pageNumber);

        // ===== Routing =====
        public async Task<QueueEntityListing?> Routing_GetRoutingQueuesAsync(int pageNumber = 1, int pageSize = 25)
            => await _routingService.Routing_GetRoutingQueuesAsync(pageNumber, pageSize);

        public async Task<QueueEntityListing?> Routing_GetRoutingQueuesDivisionViewsAsync(
            int pageNumber = 1, int pageSize = 25, List<string>? divisionId = null)
            => await _routingService.Routing_GetRoutingQueuesDivisionViewsAsync(pageNumber, pageSize, divisionId);

        public async Task<Queue?> Routing_GetRoutingQueueByIdAsync(string queueId)
            => await _routingService.Routing_GetRoutingQueueByIdAsync(queueId);

        // ===== Divsions/Auth =====
        public async Task<AuthzDivisionEntityListing?> Divisions_GetDivisionsAsync(int pageNumber = 1, int pageSize = 25)
            => await _divisionService.Divisions_GetDivisionsAsync(pageNumber, pageSize);

        public async Task<AuthzDivision?> Divisions_GetDivisionByIdAsync(string divisionId)
            => await _divisionService.Divisions_GetDivisionByIdAsync(divisionId);

        public static DivisionSummaryDTO? DivisionSummaryFromRaw(AuthzDivision? div)
            => div is null ? null : new DivisionSummaryDTO { Id = div.Id, Name = div.Name };

        // ===== Conversations =====
        public void Conversations_SetIntervalExtract(DateTimeOffset from)
            => _conversationService.SetIntervalExtract(from);

        public Task<string?> Conversations_RequestJobAsync()
            => _conversationService.Analytics_ConversationsDetails_RequestJobAsync();

        public Task<AsyncQueryStatus?> Conversations_GetJobStatusAsync(string jobId)
            => _conversationService.Analytics_ConversationsDetails_GetJobStatusAsync(jobId);

        public Task<List<AnalyticsConversation>> Conversations_GetJobResultsAsync(string jobId)
            => _conversationService.Analytics_ConversationsDetails_GetJobResultsAsync(jobId);

        public async Task<PagedResultDTO<ConversationListItemDTO>> Conversations_GetJobResultsSummaryAsync(
            string jobId, int pageNumber, int pageSize, string? routeBase = null)
        {
            var raw = await Conversations_GetJobResultsAsync(jobId);
                                                                     
            var list = raw ?? new List<AnalyticsConversation>();
            return Mappers.FromRaw(list, pageNumber, pageSize, routeBase);
        }
    }
}
