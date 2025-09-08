using PureCloudPlatform.Client.V2.Api;
using PureCloudPlatform.Client.V2.Model;
using System.Net.NetworkInformation;

namespace Api.Service.GenesysAPI
{
    public class DivisionApiService : GenesysAPIService
    {
        private readonly AuthorizationApi _authorizationApi = new AuthorizationApi();
        private readonly GenesysAuthService _GenesysAuthService;

        public DivisionApiService(GenesysAuthService authService, ILogger<GenesysAPIService> logger) : base(logger)
        {
            _GenesysAuthService = authService;
        }

        public async Task<AuthzDivisionEntityListing?> Divisions_GetDivisionsAsync(int pageSize = 25, int pageNumber = 1)
        {
            _logger.LogDebug("Divisions_GetDivisionsAsync --> pageSize={pageSize}, pageNumber={pageNumber}", pageSize, pageNumber);
            _GenesysAuthService.CheckToken();

            try
            {
                return await ExecuteWithRetry(
                    async () => await _authorizationApi.GetAuthorizationDivisionsAsync(pageSize, pageNumber),
                    "Divisions_GetDivisionsAsync");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Divisions_GetDivisionsAsync error");
                return null;
            }
        }
    }
}
