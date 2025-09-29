using Api.Service.GenesysAPI;
using PureCloudPlatform.Client.V2.Api;
using PureCloudPlatform.Client.V2.Model;

public class GroupApiService : GenesysAPIService
{
    private readonly GroupsApi _groupsApi = new();
    private readonly GenesysAuthService _genesysAuthService;

    public GroupApiService(GenesysAuthService authService, ILogger<GenesysAPIService> logger) : base(logger)
    {
        _genesysAuthService = authService;
    }

    public async Task<GroupEntityListing?> Groups_GetGroupsAsync(int pageSize = 25, int pageNumber = 1)
    {
        logger.LogDebug("Groups_GetGroupsAsync --> pageSize={pageSize}, pageNumber={pageNumber}", pageSize, pageNumber);
        _genesysAuthService.CheckToken();

        try
        {
            return await ExecuteWithRetry(
                async () => await _groupsApi.GetGroupsAsync(pageSize, pageNumber),
                "Groups_GetGroupsAsync");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Groups_GetGroupsAsync error");
            return null;
        }
    }

    public async Task<Group?> Groups_GetGroupByIdAsync(string groupId)
    {
        logger.LogDebug("Groups_GetGroupByIdAsync --> groupId={groupId}", groupId);
        _genesysAuthService.CheckToken();

        try
        {
            return await ExecuteWithRetry(
                async () => await _groupsApi.GetGroupAsync(groupId),
                "Groups_GetGroupByIdAsync");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Groups_GetGroupByIdAsync error");
            return null;
        }
    }

    // Para obtener los miembros reales del grupo
    public async Task<UserEntityListing?> Groups_GetGroupMembersAsync(string groupId, int pageSize = 25, int pageNumber = 1)
    {
        logger.LogDebug("Groups_GetGroupMembersAsync --> groupId={groupId}, pageSize={pageSize}, pageNumber={pageNumber}", 
            groupId, pageSize, pageNumber);
        _genesysAuthService.CheckToken();

        try
        {
            return await ExecuteWithRetry(
                async () => await _groupsApi.GetGroupMembersAsync(groupId, pageSize, pageNumber),
                "Groups_GetGroupMembersAsync");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Groups_GetGroupMembersAsync error");
            return null;
        }
    }
}
