using PureCloudPlatform.Client.V2.Client;

namespace Api.Service.GenesysAPI.Models
{
    public class Configuration
    {
        public PureCloudRegionHosts GenesysRegion { get; set; } = PureCloudRegionHosts.eu_west_1;
        public string GenesysClient { get; set; } = string.Empty;
        public string GenesysSecret { get; set; } = string.Empty;
    }
}