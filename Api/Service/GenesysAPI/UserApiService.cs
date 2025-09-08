using PureCloudPlatform.Client.V2.Api;
using PureCloudPlatform.Client.V2.Model;

namespace Api.Service.GenesysAPI
{
    public class UserApiService : GenesysAPIService
    {
        private readonly UsersApi _usersApi = new();
        private readonly GenesysAuthService _genesysAuthService;

        public UserApiService(GenesysAuthService authService, ILogger<GenesysAPIService> logger) : base(logger)
        {
            _genesysAuthService = authService;
        }

        public async Task<UserEntityListing?> Users_GetUsersAsync(int pageSize = 25, int pageNumber = 1, string? state = null)
        {
            _logger.LogDebug("Users_GetUserAsync --> pageSize={pagesize}, pageNumber={pageNumber}, state={state}", pageSize, pageNumber, state);
            _genesysAuthService.CheckToken();

            try
            {
                return await ExecuteWithRetry(async() => await _usersApi.GetUsersAsync(pageSize, pageNumber, state: state), 
                    "Users_GetUsersAsync");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Users_GetUsersAsync error");
                return null;
            }
        }



        //Prueba -> No vale
        public async Task<User?> Users_GetUserByIdAsync(string userId, string? state = null)
        {
            _logger.LogDebug("Users_GetUserByIdAsync --> userId={userId}, state={state}", userId, state);
            _genesysAuthService.CheckToken();

            try
            {
                return await ExecuteWithRetry(async() => await _usersApi.GetUserAsync(userId, state: state),
                    "Users_GetUserByIdAsync");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Users_GetUserByIdAsync error");
                return null;
            }
        }
    }
}
