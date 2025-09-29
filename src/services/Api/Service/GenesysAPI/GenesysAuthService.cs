using PureCloudPlatform.Client.V2.Client;
using PureCloudPlatform.Client.V2.Extensions;
using Configuration = Api.Service.GenesysAPI.Models.Configuration;


namespace Api.Service.GenesysAPI
{
    public class GenesysAuthService
    {
        private readonly object _lock = new object();
        private readonly ILogger<GenesysAuthService> _logger;
        private readonly Configuration _configuration;
        private AuthTokenInfo? _token;
        private DateTimeOffset _tokenCreationDate;
        public GenesysAuthService(ILogger<GenesysAuthService> logger, Configuration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            Authenticate();
        }

        private void Authenticate()
        {
            
            PureCloudRegionHosts region = _configuration.GenesysRegion;
            PureCloudPlatform.Client.V2.Client.Configuration.Default.ApiClient.setBasePath(region);
            lock(_lock)
            {
                _token = PureCloudPlatform.Client.V2.Client.Configuration.Default.ApiClient.PostToken(_configuration.GenesysClient, _configuration.GenesysSecret);
                _tokenCreationDate = DateTimeOffset.UtcNow;
            }
            _logger.LogInformation("Genesys API token acquired: {token}", _token);
        }

        public string GetToken()
        {
            return _token?.AccessToken ?? string.Empty;
        }

        public void CheckToken()
        {
            if (_token != null)
            {
                var tokenExpiredDate = _tokenCreationDate.AddSeconds(_token.ExpiresIn ?? 0);
                if (DateTime.UtcNow <= tokenExpiredDate.AddSeconds(-300)) // 5 minutos antes de que caduque el token
                    return;
            }
            Authenticate();
        }
    }
}
