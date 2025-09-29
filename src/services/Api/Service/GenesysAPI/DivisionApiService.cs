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

        public async Task<AuthzDivisionEntityListing?> Divisions_GetDivisionsAsync(int pageNumber = 1, int pageSize = 25)
        {
            logger.LogDebug("Divisions_GetDivisionsAsync --> pageNumber={pageNumber}, pageSize={pageSize}", pageNumber, pageSize);
            _GenesysAuthService.CheckToken();

            try
            {
                var first = await ExecuteWithRetry(
                    async () => await _authorizationApi.GetAuthorizationDivisionsAsync(pageNumber: pageNumber, pageSize: pageSize),
                    "Divisions_GetDivisionsAsync");

                var count = first?.Entities?.Count ?? 0;
                var total = first?.Total ?? 0;
                logger.LogInformation("Divisions SDK -> total={Total} count={Count}", total, count);

                // Fallback: Total>0 pero página vacía → intentamos la siguiente UNA vez
                if (total > 0 && count == 0)
                {
                    logger.LogWarning("Divisions first page empty (Total={Total}). Trying next page…", total);
                    var next = await ExecuteWithRetry(
                        async () => await _authorizationApi.GetAuthorizationDivisionsAsync(pageNumber: pageNumber + 1, pageSize: pageSize),
                        "Divisions_GetDivisionsAsync(next)");

                    if (next?.Entities?.Count > 0)
                        return next;

                    logger.LogWarning("Next page also empty. Likely permission/scope issue (authorization:division:view).");
                }

                return first;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Divisions_GetDivisionsAsync error");
                // Devolvemos un listado vacío para que el controller pueda responder 200
                return new AuthzDivisionEntityListing
                {
                    Entities = new List<AuthzDivision>(),
                    Total = 0,
                    PageCount = 0,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };
            }
        }


        public async Task<AuthzDivision?> Divisions_GetDivisionByIdAsync(string divisionId)
        {
            logger.LogDebug("Divisions_GetDivisionByIdAsync --> divisionId={divisionId}", divisionId);
            _GenesysAuthService.CheckToken();

            try
            {
                return await ExecuteWithRetry(
                    async () => await _authorizationApi.GetAuthorizationDivisionAsync(divisionId),
                    "Divisions_GetDivisionByIdAsync");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Divisions_GetDivisionByIdAsync error");
                return null;
            }
        }
    }
}
