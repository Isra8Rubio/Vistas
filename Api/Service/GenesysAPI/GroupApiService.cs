using PureCloudPlatform.Client.V2.Api;
using PureCloudPlatform.Client.V2.Model;

namespace Api.Service.GenesysAPI
{
    public class GroupApiService : GenesysAPIService
    {
        private readonly GroupsApi _groupsApi = new GroupsApi();
        private readonly GenesysAuthService _genesysAuthService;

        public GroupApiService(GenesysAuthService authService, ILogger<GenesysAPIService> logger) : base(logger)
        {
            _genesysAuthService = authService;
        }

        public async Task<GroupEntityListing?> Groups_GetGroupsAsync(int pageSize = 25, int pageNumber = 1)
        {
            _logger.LogDebug("Groups_GetGroupsAsync --> pageSize={pageSize}, pageNumber={pageNumer}", pageSize, pageNumber);
            _genesysAuthService.CheckToken();

            try
            {
                var expand = new List<string> { "owners" };
                return await ExecuteWithRetry(async() => await _groupsApi.GetGroupsAsync(pageSize,pageNumber),
                    "Groups_GetGroupsAsync");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Groups_GetGroupsAsync error");
                return null;
            }
        }

        public async Task<Group?> Groups_GetGroupByIdAsync(string groupId)
        {
            _logger.LogDebug("Groups_GetGroupByIdAsync --> groupId={groupId}", groupId);
            _genesysAuthService.CheckToken();
            try
            {
                var expand = new List<string> { "owners" };
                return await ExecuteWithRetry(
                    async () => await _groupsApi.GetGroupAsync(groupId),
                    "Groups_GetGroupByIdAsync");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Groups_GetGroupByIdAsync error");
                return null;
            }
        }
    }
}
